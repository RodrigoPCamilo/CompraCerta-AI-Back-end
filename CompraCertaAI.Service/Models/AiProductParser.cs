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

                    // Corrige descrição se ela claramente não descreve o produto
                    var descricaoBruta = item.Descricao?.Trim() ?? string.Empty;
                    var descricaoCorrigida = CorrigirDescricao(nome, descricaoBruta);

                    // Link sempre usa o nome do produto do card
                    var termoLink = SimplificarTermo(nome);

                    result.Add(new ProdutoDTO
                    {
                        NomeProduto   = nome,
                        PrecoOferta   = Preco(item.PrecoOferta)   ?? EstimarPreco(nome),
                        PrecoOriginal = Preco(item.PrecoOriginal) ?? string.Empty,
                        Desconto      = item.Desconto?.Trim()     ?? string.Empty,
                        Descricao     = descricaoCorrigida,
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

            // Sem limite de palavras — títulos como "Harry Potter e a Pedra Filosofal"
            // precisam do nome completo para gerar links de busca corretos nas lojas.
            // As limpezas acima (cores, specs técnicas) já simplificam o suficiente.
            return nome.Trim();
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

        /// <summary>
        /// Verifica se a descrição é compatível com o nome do produto.
        /// Se claramente incompatível (ex: "Bicicleta ergométrica" para "Xiaomi Mi Band 4"),
        /// gera uma descrição genérica baseada no nome — melhor que uma errada.
        /// </summary>
        private static string CorrigirDescricao(string nome, string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao)) return GerarDescricaoGenerica(nome);
            if (string.IsNullOrWhiteSpace(nome))      return descricao;

            var n = nome.ToLowerInvariant();
            var d = descricao.ToLowerInvariant();

            // Estratégia: verifica se a descrição contém pelo menos UMA palavra
            // significativa do nome do produto (ignora artigos/preposições).
            // Ex: "Adidas Powerlift 4" → descrição deve conter "adidas" ou "powerlift"
            //     Se contém só "halteres para treinamento" → não bate → gera descrição correta.
            var stopWords = new HashSet<string> {
                "de","da","do","das","dos","e","a","o","as","os","para","com","em",
                "um","uma","no","na","nos","nas","por","se","ao","aos",
                "the","for","and","of","in","to","with","4","5","6","7","8","9","1","2","3"
            };

            // Palavras significativas do nome (≥ 3 chars, não stopword)
            var palavrasNome = nome
                .ToLowerInvariant()
                .Split(new[]{ ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => p.Length >= 3 && !stopWords.Contains(p))
                .ToList();

            // Se nenhuma palavra do nome aparece na descrição → descrição errada
            if (palavrasNome.Any())
            {
                var descContemNome = palavrasNome.Any(p => d.Contains(p));
                if (!descContemNome)
                    return GerarDescricaoGenerica(nome);
            }

            return descricao;
        }

        /// <summary>
        /// Gera uma descrição genérica mas coerente baseada no nome do produto.
        /// Usada quando a IA retornou uma descrição claramente errada.
        /// </summary>
        private static string GerarDescricaoGenerica(string nome)
        {
            var n = nome.ToLowerInvariant();

            // ── Tênis / calçados (por modelo específico primeiro) ─────────────
            if (n.Contains("powerlift"))
                return "Tênis Adidas para levantamento de peso e musculação";
            if (n.Contains("crossfit nano") || n.Contains("nano crossfit"))
                return "Tênis Reebok para CrossFit e treinos funcionais";
            if (n.Contains("metcon"))
                return "Tênis Nike para CrossFit e treinos de alta intensidade";
            if (n.Contains("lifter") || n.Contains("weightlifting"))
                return "Tênis específico para levantamento de peso olímpico";
            if (n.Contains("ultraboost") || n.Contains("ultra boost"))
                return "Tênis Adidas com tecnologia Boost para corrida";
            if (n.Contains("air max 270"))
                return "Tênis Nike Air Max 270 com amortecimento avançado";
            if (n.Contains("air max"))
                return "Tênis Nike Air Max com amortecimento de ar";
            if (n.Contains("air force 1") || n.Contains("air force one"))
                return "Tênis Nike Air Force 1 clássico e versátil";
            if (n.Contains("dunk low") || n.Contains("dunk high"))
                return "Tênis Nike Dunk com design icônico e confortável";
            if (n.Contains("stan smith"))
                return "Tênis Adidas Stan Smith clássico e minimalista";
            if (n.Contains("superstar"))
                return "Tênis Adidas Superstar com design clássico e atemporal";
            if (n.Contains("samba"))
                return "Tênis Adidas Samba com estilo retrô e confortável";
            if (n.Contains("campus"))
                return "Tênis Adidas Campus casual e estiloso";
            if (n.Contains("gazelle"))
                return "Tênis Adidas Gazelle com design vintage e leve";
            if (n.Contains("yeezy"))
                return "Tênis Adidas Yeezy com design exclusivo de Kanye West";
            if (n.Contains("jordan 1") || n.Contains("air jordan 1"))
                return "Tênis Nike Air Jordan 1 com visual icônico do basquete";
            if (n.Contains("jordan"))
                return "Tênis Nike Air Jordan com tecnologia de alta performance";
            if (n.Contains("vans old skool"))
                return "Tênis Vans Old Skool clássico para skate e streetwear";
            if (n.Contains("converse") || n.Contains("all star"))
                return "Tênis Converse All Star atemporal e versátil";
            if (n.Contains("new balance"))
                return "Tênis New Balance para corrida e uso casual";
            if (n.Contains("asics gel"))
                return "Tênis Asics Gel para corrida com amortecimento avançado";
            if (n.Contains("mizuno wave"))
                return "Tênis Mizuno Wave para corrida de alto desempenho";
            if (n.Contains("adidas") && (n.Contains("tênis") || n.Contains("tenis") || n.Contains("calçado") || n.Contains("shoe")))
                return "Tênis Adidas para esporte e uso casual";
            if (n.Contains("nike") && (n.Contains("tênis") || n.Contains("tenis") || n.Contains("calçado") || n.Contains("shoe")))
                return "Tênis Nike para esporte e uso casual";
            if (n.Contains("puma") && (n.Contains("tênis") || n.Contains("tenis")))
                return "Tênis Puma confortável para esporte e lazer";
            if (n.Contains("tênis") || n.Contains("tenis"))
                return "Tênis esportivo de alta performance e conforto";

            // ── Suplementos / nutrição ────────────────────────────────────────
            if (n.Contains("whey protein") || n.Contains("whey isolado") || n.Contains("whey concentrado"))
                return "Suplemento de proteína whey para ganho muscular";
            if (n.Contains("whey"))
                return "Suplemento proteico para treino e recuperação muscular";
            if (n.Contains("creatina monohidratada") || n.Contains("creatine"))
                return "Creatina monohidratada para força e performance";
            if (n.Contains("creatina"))
                return "Suplemento de creatina para aumento de força";
            if (n.Contains("bcaa"))
                return "Aminoácidos essenciais BCAA para recuperação muscular";
            if (n.Contains("pré-treino") || n.Contains("pre-treino") || n.Contains("pre workout"))
                return "Suplemento pré-treino para energia e foco";
            if (n.Contains("proteína") || n.Contains("proteina"))
                return "Suplemento proteico para ganho de massa muscular";
            if (n.Contains("glutamina"))
                return "Suplemento de glutamina para recuperação e imunidade";
            if (n.Contains("omega") || n.Contains("ômega"))
                return "Suplemento de ômega para saúde cardiovascular";
            if (n.Contains("vitamina c"))
                return "Vitamina C para imunidade e antioxidação";
            if (n.Contains("colágeno"))
                return "Suplemento de colágeno para pele, cabelo e articulações";

            // ── Equipamentos fitness ──────────────────────────────────────────
            if (n.Contains("kettlebell"))
                return "Kettlebell para treinos funcionais e condicionamento físico";
            if (n.Contains("halter") || n.Contains("haltere"))
                return "Halteres para musculação e treinamento de força";
            if (n.Contains("esteira"))
                return "Esteira elétrica para corrida e caminhada em casa";
            if (n.Contains("bicicleta ergométrica") || n.Contains("bicicleta spinning"))
                return "Bicicleta ergométrica para treino cardiovascular em casa";
            if (n.Contains("tapete yoga") || n.Contains("yoga mat"))
                return "Tapete yoga antiderrapante para meditação e pilates";
            if (n.Contains("elástico") || n.Contains("faixa elástica"))
                return "Faixa elástica para exercícios de resistência";
            if (n.Contains("corda pular") || n.Contains("speed rope"))
                return "Corda de pular para treino cardiovascular e agilidade";
            if (n.Contains("pull up") || n.Contains("barra fixa"))
                return "Barra para treino de tração e força do core";
            if (n.Contains("luva academia") || n.Contains("luva musculação"))
                return "Luva de academia para proteção e aderência nos treinos";

            // ── Roupas esportivas ─────────────────────────────────────────────
            if (n.Contains("lululemon") || n.Contains("align pants"))
                return "Calça legging Lululemon confortável para yoga e treino";
            if (n.Contains("legging") || n.Contains("calça de treino"))
                return "Calça legging para treinos de alta performance";
            if (n.Contains("top esportivo") || n.Contains("sports bra"))
                return "Top esportivo para treinos e atividades físicas";
            if (n.Contains("roupa esportiva") || n.Contains("roupas esportivas"))
                return "Roupas esportivas para treinos e atividades físicas";
            if (n.Contains("moletom"))
                return "Moletom confortável para uso casual e treinos";
            if (n.Contains("camiseta"))
                return "Camiseta esportiva leve e respirável para treinos";

            // ── Smartphones ───────────────────────────────────────────────────
            if (n.Contains("iphone 15 pro max")) return "iPhone 15 Pro Max com chip A17 Pro e câmera avançada";
            if (n.Contains("iphone 15 pro"))     return "iPhone 15 Pro com titânio e câmera profissional";
            if (n.Contains("iphone 15"))         return "iPhone 15 com chip A16 e Dynamic Island";
            if (n.Contains("iphone 14"))         return "iPhone 14 com câmera de 48MP e chip A15 Bionic";
            if (n.Contains("iphone"))            return "Smartphone Apple com iOS e alta performance";
            if (n.Contains("galaxy s24 ultra"))  return "Samsung Galaxy S24 Ultra com S Pen e câmera de 200MP";
            if (n.Contains("galaxy s24"))        return "Samsung Galaxy S24 com chip Snapdragon e câmera avançada";
            if (n.Contains("galaxy a"))          return "Smartphone Samsung Galaxy com excelente custo-benefício";
            if (n.Contains("moto g"))            return "Smartphone Motorola Moto G com bateria de longa duração";
            if (n.Contains("redmi"))             return "Smartphone Xiaomi Redmi com ótimo custo-benefício";
            if (n.Contains("smartphone") || n.Contains("celular"))
                return "Smartphone com câmera avançada e alta performance";

            // ── Notebooks ─────────────────────────────────────────────────────
            if (n.Contains("macbook pro"))  return "MacBook Pro com chip Apple Silicon e display Liquid Retina";
            if (n.Contains("macbook air"))  return "MacBook Air ultrafino com chip Apple Silicon";
            if (n.Contains("asus rog"))     return "Notebook gamer Asus ROG para alta performance";
            if (n.Contains("asus tuf"))     return "Notebook gamer Asus TUF robusto e acessível";
            if (n.Contains("dell xps"))     return "Notebook Dell XPS premium com display OLED";
            if (n.Contains("lenovo legion"))return "Notebook gamer Lenovo Legion para jogos exigentes";
            if (n.Contains("notebook gamer") || n.Contains("laptop gamer"))
                return "Notebook gamer com placa dedicada para jogos";
            if (n.Contains("notebook") || n.Contains("laptop"))
                return "Notebook para trabalho, estudos e entretenimento";

            // ── Fones / áudio ─────────────────────────────────────────────────
            if (n.Contains("airpods pro"))   return "Apple AirPods Pro com cancelamento ativo de ruído";
            if (n.Contains("airpods max"))   return "Apple AirPods Max com áudio espacial e ANC premium";
            if (n.Contains("airpods"))       return "Apple AirPods sem fio com chip H1 e áudio imersivo";
            if (n.Contains("sony wh-1000"))  return "Fone Sony WH-1000 com cancelamento de ruído líder do mercado";
            if (n.Contains("bose quietcomfort")) return "Fone Bose QuietComfort com ANC e conforto premium";
            if (n.Contains("jbl flip"))      return "Caixa de som JBL Flip portátil e à prova d'água";
            if (n.Contains("jbl charge"))    return "Caixa de som JBL Charge com bateria de longa duração";
            if (n.Contains("jbl"))           return "Produto JBL com qualidade de áudio premium";
            if (n.Contains("headset gamer")) return "Headset gamer com som surround e microfone integrado";
            if (n.Contains("fone") || n.Contains("headphone") || n.Contains("earbuds"))
                return "Fone de ouvido com qualidade de som premium";

            // ── Smartwatches ──────────────────────────────────────────────────
            if (n.Contains("apple watch ultra"))  return "Apple Watch Ultra com GPS de precisão para esportes extremos";
            if (n.Contains("apple watch se"))     return "Apple Watch SE com monitoramento de saúde e fitness";
            if (n.Contains("apple watch"))        return "Apple Watch com monitoramento de saúde e apps nativos";
            if (n.Contains("galaxy watch"))       return "Samsung Galaxy Watch com saúde avançada e apps";
            if (n.Contains("garmin forerunner"))  return "Smartwatch Garmin para corredores com GPS avançado";
            if (n.Contains("garmin"))             return "Smartwatch Garmin para atletas e aventureiros";
            if (n.Contains("amazfit"))            return "Smartwatch Amazfit com bateria duradoura e esportes";
            if (n.Contains("mi band"))            return "Xiaomi Mi Band para monitoramento de saúde e fitness";
            if (n.Contains("smartwatch"))         return "Smartwatch com monitoramento de saúde e notificações";

            // ── Games / consoles ──────────────────────────────────────────────
            if (n.Contains("ps5") || n.Contains("playstation 5"))
                return "Console PlayStation 5 com SSD ultrarrápido e DualSense";
            if (n.Contains("ps4") || n.Contains("playstation 4"))
                return "Console PlayStation 4 com grande biblioteca de jogos";
            if (n.Contains("xbox series x"))
                return "Console Xbox Series X com 4K nativo e SSD NVMe";
            if (n.Contains("xbox series s"))
                return "Console Xbox Series S compacto para jogos digitais";
            if (n.Contains("nintendo switch oled"))
                return "Nintendo Switch OLED com tela vibrante e jogo portátil";
            if (n.Contains("nintendo switch"))
                return "Nintendo Switch híbrido para jogar em casa e portátil";
            if (n.Contains("dualsense") || n.Contains("controle ps5"))
                return "Controle DualSense com feedback háptico e gatilhos adaptáveis";
            if (n.Contains("controle xbox"))
                return "Controle Xbox sem fio com design ergonômico";
            if (n.Contains("playstation") || n.Contains("xbox") || n.Contains("nintendo"))
                return "Console de videogame para jogos de alta performance";

            // ── Livros ────────────────────────────────────────────────────────
            if (n.Contains("harry potter"))    return "Livro da série Harry Potter por J.K. Rowling";
            if (n.Contains("senhor dos anéis") || n.Contains("tolkien"))
                return "Livro épico de fantasia por J.R.R. Tolkien";
            if (n.Contains("mangá") || n.Contains("manga"))
                return "Mangá com arte e narrativa envolventes";
            if (n.Contains("hq") || n.Contains("quadrinhos"))
                return "HQ com histórias em quadrinhos de alto impacto visual";
            if (n.Contains("autoajuda") || n.Contains("hábitos") || n.Contains("poder do"))
                return "Livro de autoajuda para desenvolvimento pessoal";
            if (n.Contains("livro"))
                return "Livro com conteúdo envolvente e de alta qualidade";

            // ── Eletrodomésticos / casa ───────────────────────────────────────
            if (n.Contains("airfryer") || n.Contains("fritadeira sem óleo"))
                return "Fritadeira sem óleo para preparo de refeições mais saudáveis";
            if (n.Contains("microondas"))    return "Micro-ondas eficiente para aquecimento rápido";
            if (n.Contains("geladeira") || n.Contains("refrigerador"))
                return "Geladeira com eficiência energética e grande capacidade";
            if (n.Contains("lavadora") || n.Contains("máquina de lavar"))
                return "Lavadora de roupas eficiente com múltiplos programas";
            if (n.Contains("cafeteira"))     return "Cafeteira para preparo de café cremoso e saboroso";
            if (n.Contains("dyson airwrap")) return "Dyson Airwrap modelador de cabelo com tecnologia Airwrap";
            if (n.Contains("dyson"))         return "Produto Dyson com tecnologia de alta performance";
            if (n.Contains("aspirador"))     return "Aspirador de pó potente para limpeza eficiente";

            // ── TV ────────────────────────────────────────────────────────────
            if (n.Contains("oled"))          return "Smart TV OLED com qualidade de imagem excepcional";
            if (n.Contains("qled"))          return "Smart TV QLED com cores vibrantes e brilho intenso";
            if (n.Contains("smart tv"))      return "Smart TV com conectividade e apps de streaming";

            // ── Câmeras ───────────────────────────────────────────────────────
            if (n.Contains("gopro hero"))    return "Câmera GoPro Hero para aventuras e esportes radicais";
            if (n.Contains("gopro"))         return "Câmera de ação GoPro para fotos e vídeos extremos";
            if (n.Contains("sony alpha"))    return "Câmera mirrorless Sony Alpha para fotografia profissional";
            if (n.Contains("canon eos"))     return "Câmera Canon EOS para fotografia e vídeo de alta qualidade";
            if (n.Contains("câmera") || n.Contains("camera"))
                return "Câmera para fotos e vídeos de alta qualidade";

            // ── Perfume / beleza ──────────────────────────────────────────────
            if (n.Contains("eau de parfum")) return "Eau de Parfum com fragrância intensa e duradoura";
            if (n.Contains("eau de toilette"))return "Eau de Toilette com fragrância fresca e sofisticada";
            if (n.Contains("perfume"))       return "Perfume com fragrância exclusiva e duradoura";
            if (n.Contains("sérum") || n.Contains("serum"))
                return "Sérum facial com ativos para rejuvenescimento da pele";
            if (n.Contains("protetor solar"))
                return "Protetor solar com alta proteção contra raios UV";
            if (n.Contains("hidratante"))    return "Hidratante corporal para pele macia e nutrida";
            if (n.Contains("shampoo"))       return "Shampoo para cabelos saudáveis e com brilho";

            // ── Bolsas / acessórios ───────────────────────────────────────────
            if (n.Contains("mochila"))       return "Mochila espaçosa e resistente para uso diário";
            if (n.Contains("bolsa"))         return "Bolsa elegante e funcional para o dia a dia";

            // ── Fallback: extrai palavras-chave do próprio nome ───────────────
            var palavras = nome.Split(new[]{ ' ', '-' }, StringSplitOptions.RemoveEmptyEntries)
                               .Where(p => p.Length >= 3)
                               .Take(4)
                               .ToArray();
            var resumo = palavras.Length > 0 ? string.Join(" ", palavras) : nome;
            return $"Produto {resumo} de alta qualidade";
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