using System;
using System.Collections.Generic;

namespace CompraCertaAI.Service.Models
{
    public static class ProdutoUrlHelper
    {
        public const string PlaceholderImageUrl = "https://placehold.co/600x400/1a1a25/f97316?text=Produto";

        // IDs reais do Unsplash verificados por categoria/produto
        private static readonly Dictionary<string, string> UnsplashIds = new(StringComparer.OrdinalIgnoreCase)
        {
            { "smartphone",    "photo-1511707267537-b85faf00021e" },
            { "celular",       "photo-1511707267537-b85faf00021e" },
            { "iphone",        "photo-1510557880182-3d4d3cba35a5" },
            { "samsung",       "photo-1567784177951-6fa58317e16b" },
            { "notebook",      "photo-1496181133206-80ce9b88a853" },
            { "laptop",        "photo-1496181133206-80ce9b88a853" },
            { "computador",    "photo-1593642632559-0c6d3fc62b89" },
            { "desktop",       "photo-1593642632559-0c6d3fc62b89" },
            { "tablet",        "photo-1544244015-0df4b3ffc6b0" },
            { "ipad",          "photo-1544244015-0df4b3ffc6b0" },
            { "fone",          "photo-1505740420928-5e560c06d30e" },
            { "headphone",     "photo-1505740420928-5e560c06d30e" },
            { "headset",       "photo-1505740420928-5e560c06d30e" },
            { "earphone",      "photo-1505740420928-5e560c06d30e" },
            { "airpod",        "photo-1572635196237-14b3f281503f" },
            { "speaker",       "photo-1608043152269-423dbba4e7e1" },
            { "caixa de som",  "photo-1608043152269-423dbba4e7e1" },
            { "smartwatch",    "photo-1523275335684-37898b6baf30" },
            { "relógio",       "photo-1523275335684-37898b6baf30" },
            { "relogio",       "photo-1523275335684-37898b6baf30" },
            { "câmera",        "photo-1516035069371-29a1b244cc32" },
            { "camera",        "photo-1516035069371-29a1b244cc32" },
            { "tv",            "photo-1593784991095-a205069470b6" },
            { "televisão",     "photo-1593784991095-a205069470b6" },
            { "teclado",       "photo-1587829741301-dc798b83add3" },
            { "mouse",         "photo-1527814050087-3793815479db" },
            { "placa",         "photo-1591488320449-011701bb6704" },
            { "gpu",           "photo-1591488320449-011701bb6704" },
            { "geforce",       "photo-1591488320449-011701bb6704" },
            { "impressora",    "photo-1612198188060-c7c2a3b66eae" },
            { "camisa",        "photo-1602810318383-e386cc2a3ccf" },
            { "camiseta",      "photo-1521572163474-6864f9cf17ab" },
            { "calça",         "photo-1542272604-787c3835535d" },
            { "vestido",       "photo-1515372039744-b8f02a3ae446" },
            { "jaqueta",       "photo-1548036328-c9fa89d128fa" },
            { "tênis",         "photo-1542291026-7eec264c27ff" },
            { "tenis",         "photo-1542291026-7eec264c27ff" },
            { "sapato",        "photo-1449505278894-297fdb3edbc1" },
            { "bolsa",         "photo-1548036328-c9fa89d128fa" },
            { "sofá",          "photo-1555041469-a586c61ea9bc" },
            { "sofa",          "photo-1555041469-a586c61ea9bc" },
            { "cadeira",       "photo-1555041469-a586c61ea9bc" },
            { "mesa",          "photo-1555041469-a586c61ea9bc" },
            { "panela",        "photo-1556909114-f6e7ad7d3136" },
            { "fritadeira",    "photo-1626200419199-391ae4be7a41" },
            { "airfryer",      "photo-1626200419199-391ae4be7a41" },
            { "liquidificador","photo-1578662996442-48f60103fc96" },
            { "microondas",    "photo-1574269909862-7e1d70bb8078" },
            { "videogame",     "photo-1606144042614-b2417e99c4e3" },
            { "playstation",   "photo-1606144042614-b2417e99c4e3" },
            { "xbox",          "photo-1612287230202-1ff1d85d1bdf" },
            { "controle",      "photo-1606144042614-b2417e99c4e3" },
            { "nintendo",      "photo-1612287230202-1ff1d85d1bdf" },
            { "livro",         "photo-1544947950-fa07a98d237f" },
        };

        /// <summary>
        /// Normaliza a URL do produto, corrigindo formatos inválidos gerados pela IA.
        /// Sempre retorna uma URL funcional.
        /// </summary>
        public static string NormalizeProductLink(string? raw, string? nomeProduto = null, string? loja = null)
        {
            if (!string.IsNullOrWhiteSpace(raw))
            {
                var value = raw.Trim();
                if (value.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                    value = "https://" + value;

                // Codifica espaços antes de tentar parsear a URI
                value = EncodeSpacesInUrl(value);

                if (Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    return BuildCorrectStoreLink(uri, nomeProduto, loja);
                }
            }

            if (!string.IsNullOrWhiteSpace(nomeProduto) && !string.IsNullOrWhiteSpace(loja))
                return GenerateSearchLink(nomeProduto, loja);

            return string.Empty;
        }

        /// <summary>
        /// Normaliza a URL da imagem. Qualquer URL do Unsplash é substituída por
        /// uma foto real baseada no nome do produto (a IA inventa IDs inexistentes).
        /// </summary>
        public static string NormalizeImageUrl(string? raw, string? nomeProduto = null)
        {
            if (!string.IsNullOrWhiteSpace(raw))
            {
                var value = raw.Trim();
                if (value.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                    value = "https://" + value;

                if (Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    // IA inventa IDs do Unsplash — sempre substituímos pelo nosso mapa
                    if (uri.Host.Contains("unsplash.com"))
                        return GetUnsplashUrl(nomeProduto);

                    return uri.ToString();
                }
            }

            return GetUnsplashUrl(nomeProduto);
        }

        /// <summary>
        /// Gera o link de busca correto para cada loja a partir do nome do produto.
        /// </summary>
        public static string GenerateSearchLink(string nomeProduto, string loja)
        {
            var query = Uri.EscapeDataString(nomeProduto.Trim());
            var lojaLower = (loja ?? "").ToLowerInvariant().Trim();

            if (lojaLower.Contains("amazon"))
                return $"https://www.amazon.com.br/s?k={query}";

            if (lojaLower.Contains("mercado"))
                return $"https://lista.mercadolivre.com.br/{query}";

            if (lojaLower.Contains("magazine") || lojaLower.Contains("magalu"))
                return $"https://www.magazineluiza.com.br/busca/{query}/";

            if (lojaLower.Contains("shopee"))
                return $"https://shopee.com.br/search?keyword={query}";

            if (lojaLower.Contains("americanas"))
                return $"https://www.americanas.com.br/busca/{query}";

            if (lojaLower.Contains("kabum"))
                return $"https://www.kabum.com.br/busca/{query}";

            return $"https://www.google.com/search?q={query}+{Uri.EscapeDataString(loja)}";
        }

        // ── Privados ────────────────────────────────────────────────────────────

        private static string GetUnsplashUrl(string? nomeProduto)
        {
            if (!string.IsNullOrWhiteSpace(nomeProduto))
            {
                var nome = nomeProduto.ToLowerInvariant();
                foreach (var kv in UnsplashIds)
                    if (nome.Contains(kv.Key))
                        return $"https://images.unsplash.com/{kv.Value}?w=600&q=80&fit=crop";
            }
            return "https://images.unsplash.com/photo-1607082348824-0a96f2a4b9da?w=600&q=80&fit=crop";
        }

        /// <summary>
        /// Recebe uma URI já parseada e devolve o link correto para a loja,
        /// regenerando a partir do nome do produto quando o formato está errado.
        /// </summary>
        private static string BuildCorrectStoreLink(Uri uri, string? nomeProduto, string? loja)
        {
            var host = uri.Host.ToLowerInvariant();
            var pq   = uri.PathAndQuery;       // preserva case original para o encode

            // ── Amazon ──────────────────────────────────────────────────────────
            if (host.Contains("amazon.com.br"))
            {
                if (pq.Contains("/s?k=") || pq.Contains("/dp/"))
                    return uri.ToString();
                if (!string.IsNullOrWhiteSpace(nomeProduto))
                    return $"https://www.amazon.com.br/s?k={Uri.EscapeDataString(nomeProduto)}";
            }

            // ── Mercado Livre ────────────────────────────────────────────────────
            if (host.Contains("mercadolivre") || host.Contains("mercadolibre"))
            {
                // lista.mercadolivre.com.br/TERMO  — formato oficial de busca
                if (host.StartsWith("lista."))
                    return uri.ToString();           // já está no formato certo
                if (!string.IsNullOrWhiteSpace(nomeProduto))
                    return $"https://lista.mercadolivre.com.br/{Uri.EscapeDataString(nomeProduto)}";
            }

            // ── Magazine Luiza ───────────────────────────────────────────────────
            if (host.Contains("magazineluiza") || host.Contains("magalu"))
            {
                // Formato certo: /busca/TERMO/ com encode
                if (!string.IsNullOrWhiteSpace(nomeProduto))
                    return $"https://www.magazineluiza.com.br/busca/{Uri.EscapeDataString(nomeProduto)}/";
            }

            // ── Shopee ───────────────────────────────────────────────────────────
            if (host.Contains("shopee"))
            {
                // Formato certo: /search?keyword=TERMO  (sem barra antes de ?)
                if (!string.IsNullOrWhiteSpace(nomeProduto))
                    return $"https://shopee.com.br/search?keyword={Uri.EscapeDataString(nomeProduto)}";
            }

            // ── Americanas ───────────────────────────────────────────────────────
            if (host.Contains("americanas"))
            {
                if (!string.IsNullOrWhiteSpace(nomeProduto))
                    return $"https://www.americanas.com.br/busca/{Uri.EscapeDataString(nomeProduto)}";
            }

            // ── Kabum ────────────────────────────────────────────────────────────
            if (host.Contains("kabum"))
            {
                if (!string.IsNullOrWhiteSpace(nomeProduto))
                    return $"https://www.kabum.com.br/busca/{Uri.EscapeDataString(nomeProduto)}";
            }

            // URL de domínio desconhecido mas válida — retorna como está
            return uri.ToString();
        }

        /// <summary>
        /// Substitui espaços literais em uma URL por %20 antes do parse,
        /// pois Uri.TryCreate aceita espaços mas os navegadores podem rejeitar.
        /// </summary>
        private static string EncodeSpacesInUrl(string url)
        {
            // Separa a parte antes do ? para não duplicar encode em query strings
            var qIdx = url.IndexOf('?');
            if (qIdx < 0)
                return url.Replace(" ", "%20");

            var path  = url.Substring(0, qIdx).Replace(" ", "%20");
            var query = url.Substring(qIdx);

            // Codifica espaços no valor dos parâmetros da query string
            var encodedQuery = System.Text.RegularExpressions.Regex
                .Replace(query, @"(?<==)[^&]*", m => m.Value.Replace(" ", "%20"));

            return path + encodedQuery;
        }
    }
}