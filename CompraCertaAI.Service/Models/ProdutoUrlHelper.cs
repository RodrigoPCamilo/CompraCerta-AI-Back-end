using System;

namespace CompraCertaAI.Service.Models
{
    public static class ProdutoUrlHelper
    {
        public const string PlaceholderImageUrl = "https://placehold.co/600x400?text=Produto";

        public static string NormalizeProductLink(string? raw, string? nomeProduto = null, string? loja = null)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                if (!string.IsNullOrWhiteSpace(nomeProduto) && !string.IsNullOrWhiteSpace(loja))
                    return GenerateFallbackLink(nomeProduto, loja);
                return string.Empty;
            }

            var value = raw.Trim();

            if (value.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                value = "https://" + value;

            if (Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return uri.ToString();
            }

            if (!string.IsNullOrWhiteSpace(nomeProduto) && !string.IsNullOrWhiteSpace(loja))
                return GenerateFallbackLink(nomeProduto, loja);
                
            return string.Empty;
        }

        public static string NormalizeImageUrl(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return PlaceholderImageUrl;

            var value = raw.Trim();

            if (value.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                value = "https://" + value;

            if (Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return uri.ToString();
            }

            return PlaceholderImageUrl;
        }

        private static string GenerateFallbackLink(string nomeProduto, string loja)
        {
            var lojaLower = (loja ?? "").ToLower().Trim();
            var queryProduto = Uri.EscapeDataString(nomeProduto);

            return lojaLower switch
            {
                "amazon" => $"https://www.amazon.com.br/s?k={queryProduto}",
                "mercado livre" => $"https://lista.mercadolivre.com.br/{queryProduto}",
                "magazine luiza" => $"https://www.magazineluiza.com.br/busca/{queryProduto}/",
                "shopee" => $"https://shopee.com.br/search?keyword={queryProduto}",
                "kabum" => $"https://www.kabum.com.br/busca/{queryProduto}",
                _ => $"https://www.google.com/search?q={queryProduto}+{lojaLower}"
            };
        }
    }
}