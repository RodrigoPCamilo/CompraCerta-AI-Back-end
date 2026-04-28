using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using CompraCertaAI.Aplicacao.DTOs.Produto;

namespace CompraCertaAI.Service.Models
{
    public static class AiProductParser
    {
        public static IReadOnlyList<ProdutoDTO> ParseProducts(
            string? raw, int limit = 12, string? queryOriginal = null)
        {
            if (string.IsNullOrWhiteSpace(raw) || limit <= 0)
                return Array.Empty<ProdutoDTO>();

            var json = ExtractJsonArray(raw);
            if (string.IsNullOrWhiteSpace(json))
                return Array.Empty<ProdutoDTO>();

            try
            {
                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas         = true,
                    ReadCommentHandling         = JsonCommentHandling.Skip
                };

                var items = JsonSerializer.Deserialize<List<AiItem>>(json, opts)
                    ?? new List<AiItem>();

                var seen   = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var result = new List<ProdutoDTO>();

                foreach (var item in items)
                {
                    var nome = LimparNome(item.NomeProduto?.Trim() ?? string.Empty);
                    var loja = item.Loja?.Trim() ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(loja))
                        continue;

                    if (!seen.Add($"{nome}|{loja}")) continue;

                    // Link sempre usa o nome do produto do card
                    // (SimplificarTermo remove specs técnicas para busca mais ampla)
                    var termoLink = SimplificarTermo(nome);

                    result.Add(new ProdutoDTO
                    {
                        NomeProduto   = nome,
                        PrecoOferta   = Preco(item.PrecoOferta)   ?? EstimarPreco(nome),
                        PrecoOriginal = Preco(item.PrecoOriginal) ?? string.Empty,
                        Desconto      = item.Desconto?.Trim()     ?? string.Empty,
                        Descricao     = item.Descricao?.Trim()    ?? string.Empty,
                        ImagemUrl     = string.Empty,
                        Loja          = loja,
                        LinkProduto   = GerarLink(termoLink, loja),
                        CategoriaNome = string.Empty,
                    });

                    if (result.Count >= limit) break;
                }

                return result;
            }
            catch { return Array.Empty<ProdutoDTO>(); }
        }

        private static string GerarLink(string termo, string loja)
        {
            var q = Uri.EscapeDataString(termo.Trim());
            var l = loja.ToLowerInvariant();

            if (l.Contains("amazon"))
                return $"https://www.amazon.com.br/s?k={q}";
            if (l.Contains("mercado"))
                return $"https://lista.mercadolivre.com.br/{q}";
            if (l.Contains("magazine") || l.Contains("magalu"))
                return $"https://www.magazineluiza.com.br/busca/{q}/";
            if (l.Contains("shopee"))
                return $"https://shopee.com.br/search?keyword={q}";
            if (l.Contains("americanas"))
                return $"https://www.americanas.com.br/busca/{q}";
            if (l.Contains("kabum"))
                return $"https://www.kabum.com.br/busca/{q}";

            return $"https://www.google.com/search?q={q}+{Uri.EscapeDataString(loja)}";
        }

        // Remove specs técnicas para não restringir demais a busca
        // "Adidas Superstar 2022 Preto 42" → "Adidas Superstar 2022"
        // "Adidas Ultraboost 22 Feminino" → "Adidas Ultraboost 22"
        private static string SimplificarTermo(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return nome;

            nome = Regex.Replace(nome,
                @"\b\d+\s*(GB|TB|MB|mAh|Hz|W|MP|DPI|RPM|cores?)\b",
                "", RegexOptions.IgnoreCase);
            nome = Regex.Replace(nome,
                @"\b\d+[,.]?\d*\s*(pol\.?|polegadas?|"")",
                "", RegexOptions.IgnoreCase);
            nome = Regex.Replace(nome,
                @"\b(Preto|Preta|Branco|Branca|Azul|Vermelho|Vermelha|Verde|Amarelo|Amarela|" +
                @"Cinza|Prata|Dourado|Dourada|Rosa|Roxo|Roxa|Laranja|Bege|Grafite|Prateado|" +
                @"Masculino|Feminino|Infantil|Unissex|Junior)\b",
                "", RegexOptions.IgnoreCase);
            nome = Regex.Replace(nome, @"\s+", " ").Trim();

            // Máximo 4 palavras — preserva marca + modelo + versão
            // "Adidas Superstar 2022" = 3 palavras ✓
            // "Nike Air Max 270" = 4 palavras ✓
            var palavras = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", palavras.Take(4));
        }

        private static string LimparNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return nome;
            nome = Regex.Replace(nome, @"\s+[Mm]arca\s+\S+(\s+[Mm]odelo\s+.+)?$", "").Trim();
            nome = Regex.Replace(nome, @"\s+[Mm]odelo\s+.+$", "").Trim();
            var p = nome.Split(' ');
            if (p.Length >= 4)
            {
                var m  = p.Length / 2;
                var p1 = string.Join(" ", p[..m]);
                var p2 = string.Join(" ", p[m..]);
                if (string.Equals(p1, p2, StringComparison.OrdinalIgnoreCase))
                    nome = p1;
            }
            return nome.Trim();
        }

        private static string? Preco(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            var t = raw.Trim();

            // Rejeitar valores zero ou placeholder
            if (t == "0" || t == "0,00" || t == "R$ 0,00" || t == "R$0,00" ||
                t == "0.00" || t == "R$ 0" || t.ToLower() == "null")
                return null;

            if (t.StartsWith("R$"))
            {
                // Verifica se o valor após R$ é zero
                var num = Regex.Replace(t[2..].Trim(), @"[^\d,.]", "");
                if (decimal.TryParse(num.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var check)
                    && check <= 0)
                    return null;
                return t;
            }

            var c = Regex.Replace(t, @"[^\d,.]", "");
            if (decimal.TryParse(c.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var v) && v > 0)
                return v.ToString("C", new System.Globalization.CultureInfo("pt-BR"));

            return null;
        }


        /// <summary>
        /// Estima um preço plausível pelo nome do produto quando a IA retorna zero.
        /// Usa faixas de preço reais do mercado brasileiro como referência.
        /// </summary>
        private static string EstimarPreco(string nome)
        {
            var n = nome.ToLowerInvariant();

            // Games — jogos vs consoles
            if (n.Contains("playstation 5") || n.Contains("ps5") || n.Contains("xbox series x"))
                return "R$ 3.999,90";
            if (n.Contains("xbox series s") || n.Contains("nintendo switch oled"))
                return "R$ 2.499,90";
            if (n.Contains("nintendo switch") || n.Contains("ps4") || n.Contains("playstation 4"))
                return "R$ 1.999,90";
            if (n.Contains("the last of us") || n.Contains("god of war") ||
                n.Contains("red dead") || n.Contains("cyberpunk") || n.Contains("gta"))
                return "R$ 249,90";

            // Smartphones
            if (n.Contains("iphone 15") || n.Contains("galaxy s24"))  return "R$ 5.999,90";
            if (n.Contains("iphone 14") || n.Contains("galaxy s23"))  return "R$ 4.499,90";
            if (n.Contains("iphone 13") || n.Contains("galaxy s22"))  return "R$ 3.299,90";
            if (n.Contains("iphone") || n.Contains("galaxy s"))       return "R$ 2.999,90";
            if (n.Contains("galaxy a") || n.Contains("redmi note"))   return "R$ 1.499,90";
            if (n.Contains("moto g") || n.Contains("redmi"))          return "R$ 999,90";

            // Notebooks
            if (n.Contains("macbook pro"))   return "R$ 11.999,90";
            if (n.Contains("macbook air"))   return "R$ 8.499,90";
            if (n.Contains("rog") || n.Contains("predator") || n.Contains("legion"))
                return "R$ 6.999,90";
            if (n.Contains("notebook") || n.Contains("laptop"))       return "R$ 2.999,90";

            // Fones
            if (n.Contains("airpods pro"))   return "R$ 1.799,90";
            if (n.Contains("airpods"))       return "R$ 1.299,90";
            if (n.Contains("sony wh") || n.Contains("wh-1000"))      return "R$ 1.499,90";
            if (n.Contains("jbl"))           return "R$ 299,90";
            if (n.Contains("fone") || n.Contains("headphone"))        return "R$ 199,90";

            // TV
            if (n.Contains("oled") || n.Contains("qled"))  return "R$ 3.999,90";
            if (n.Contains("smart tv") || n.Contains("televisão"))    return "R$ 1.799,90";

            // Tênis
            if (n.Contains("yeezy") || n.Contains("jordan"))         return "R$ 999,90";
            if (n.Contains("ultraboost") || n.Contains("air max"))   return "R$ 699,90";
            if (n.Contains("adidas") || n.Contains("nike"))          return "R$ 499,90";
            if (n.Contains("tênis") || n.Contains("tenis"))          return "R$ 299,90";

            // Eletrodomésticos
            if (n.Contains("geladeira") || n.Contains("lavadora"))   return "R$ 2.499,90";
            if (n.Contains("airfryer") || n.Contains("fritadeira"))  return "R$ 399,90";
            if (n.Contains("microondas"))                             return "R$ 499,90";

            // Padrão
            return "R$ 299,90";
        }

        private static string? ExtractJsonArray(string s)
        {
            s = s.Trim();
            if (s.StartsWith("```"))
            {
                s = Regex.Replace(s, @"^```[a-zA-Z]*\n?", "").TrimStart();
                s = Regex.Replace(s, @"\n?```$", "").TrimEnd();
                s = s.Trim();
            }
            if (s.StartsWith("[")) { var e = s.LastIndexOf(']'); return e > 0 ? s[..(e + 1)] : null; }
            if (s.StartsWith("{"))
            {
                try
                {
                    using var d = JsonDocument.Parse(s);
                    foreach (var p in d.RootElement.EnumerateObject())
                        if (p.Value.ValueKind == JsonValueKind.Array)
                            return p.Value.GetRawText();
                }
                catch { }
            }
            var st = s.IndexOf('['); var en = s.LastIndexOf(']');
            return st >= 0 && en > st ? s[st..(en + 1)] : null;
        }

        private sealed class AiItem
        {
            public string? NomeProduto   { get; set; }
            public string? PrecoOferta   { get; set; }
            public string? PrecoOriginal { get; set; }
            public string? Desconto      { get; set; }
            public string? Descricao     { get; set; }
            public string? ImagemUrl     { get; set; }
            public string? Loja          { get; set; }
            public string? LinkProduto   { get; set; }
        }
    }
}
