using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using CompraCertaAI.Aplicacao.DTOs.Produto;

namespace CompraCertaAI.Service.Models
{
    public static class AiProductParser
    {
        public static IReadOnlyList<ProdutoDTO> ParseProducts(string? responseContent, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(responseContent) || limit <= 0)
                return Array.Empty<ProdutoDTO>();

            var jsonArray = ExtractJsonArray(responseContent);
            if (string.IsNullOrWhiteSpace(jsonArray))
                return Array.Empty<ProdutoDTO>();

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var items = JsonSerializer.Deserialize<List<AiProductItem>>(jsonArray, options)
                    ?? new List<AiProductItem>();

                var uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var result = new List<ProdutoDTO>();

                foreach (var item in items)
                {
                    var nomeProduto = item.NomeProduto?.Trim() ?? string.Empty;
                    var precoOferta = item.PrecoOferta?.Trim() ?? string.Empty;
                    var descricao = item.Descricao?.Trim() ?? string.Empty;
                    var imagemUrl = ProdutoUrlHelper.NormalizeImageUrl(item.ImagemUrl);
                    var loja = item.Loja?.Trim() ?? string.Empty;
                    var linkProduto = ProdutoUrlHelper.NormalizeProductLink(item.LinkProduto, nomeProduto, loja);

                    if (string.IsNullOrWhiteSpace(nomeProduto)
                        || string.IsNullOrWhiteSpace(loja)
                        || string.IsNullOrWhiteSpace(precoOferta))
                        continue;

                    if (string.IsNullOrWhiteSpace(linkProduto))
                        linkProduto = $"https://www.google.com/search?q={Uri.EscapeDataString(nomeProduto)}+{Uri.EscapeDataString(loja)}";

                    var dedupeKey = string.Concat(nomeProduto, "|", loja, "|", linkProduto);
                    if (!uniqueKeys.Add(dedupeKey))
                        continue;

                    result.Add(new ProdutoDTO
                    {
                        NomeProduto = nomeProduto,
                        PrecoOferta = precoOferta,
                        Descricao = descricao,
                        ImagemUrl = imagemUrl,
                        Loja = loja,
                        LinkProduto = linkProduto,
                        CategoriaNome = string.Empty
                    });

                    if (result.Count >= limit)
                        break;
                }

                return result;
            }
            catch (Exception)
            {
                return Array.Empty<ProdutoDTO>();
            }
        }

        private static string? ExtractJsonArray(string content)
        {
            var start = content.IndexOf('[');
            var end = content.LastIndexOf(']');

            if (start < 0 || end < 0 || end <= start)
                return null;

            var jsonString = content.Substring(start, end - start + 1);
            
            // Cleanup: remover espaços múltiplos fora de strings JSON
            // Isso ajuda quando a IA retorna JSON malformado com muitos espaços
            var cleaned = System.Text.RegularExpressions.Regex.Replace(jsonString, @"""[\s]*,", "\",");
            cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"""[\s]*\}", "\"}");
            cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"""[\s]*\]", "\"]");
            
            return cleaned;
        }

        private sealed class AiProductItem
        {
            public string? NomeProduto { get; set; }
            public string? PrecoOferta { get; set; }
            public string? Descricao { get; set; }
            public string? ImagemUrl { get; set; }
            public string? Loja { get; set; }
            public string? LinkProduto { get; set; }
        }
    }
}