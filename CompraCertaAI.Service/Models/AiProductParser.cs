using System;
using System.Collections.Generic;
using System.Text.Json;
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
                    var precoOferta  = item.PrecoOferta?.Trim() ?? string.Empty;
                    var descricao    = item.Descricao?.Trim() ?? string.Empty;
                    var loja         = item.Loja?.Trim() ?? string.Empty;

                    // Campos mínimos obrigatórios
                    if (string.IsNullOrWhiteSpace(nomeProduto) || string.IsNullOrWhiteSpace(loja))
                        continue;

                    if (string.IsNullOrWhiteSpace(precoOferta))
                        precoOferta = "Consulte na loja";

                    // Passa nomeProduto para que o helper escolha imagem e link corretos
                    var imagemUrl   = ProdutoUrlHelper.NormalizeImageUrl(item.ImagemUrl, nomeProduto);
                    var linkProduto = ProdutoUrlHelper.NormalizeProductLink(item.LinkProduto, nomeProduto, loja);

                    if (string.IsNullOrWhiteSpace(linkProduto))
                        linkProduto = ProdutoUrlHelper.GenerateSearchLink(nomeProduto, loja);

                    var dedupeKey = $"{nomeProduto}|{loja}";
                    if (!uniqueKeys.Add(dedupeKey))
                        continue;

                    result.Add(new ProdutoDTO
                    {
                        // id = 0 para produtos gerados pela IA (não estão no banco)
                        // O front deve aceitar id = 0 como válido
                        NomeProduto  = nomeProduto,
                        PrecoOferta  = precoOferta,
                        Descricao    = descricao,
                        ImagemUrl    = imagemUrl,
                        Loja         = loja,
                        LinkProduto  = linkProduto,
                        CategoriaNome = string.Empty
                    });

                    if (result.Count >= limit)
                        break;
                }

                return result;
            }
            catch
            {
                return Array.Empty<ProdutoDTO>();
            }
        }

        private static string? ExtractJsonArray(string content)
        {
            content = content.Trim();

            // Caso 1: já é um array JSON
            if (content.StartsWith("["))
            {
                var end = content.LastIndexOf(']');
                return end > 0 ? content.Substring(0, end + 1) : null;
            }

            // Caso 2: objeto JSON com uma propriedade de array (ex: { "produtos": [...] })
            if (content.StartsWith("{"))
            {
                try
                {
                    using var doc = JsonDocument.Parse(content);
                    foreach (var prop in doc.RootElement.EnumerateObject())
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                            return prop.Value.GetRawText();
                }
                catch { }
            }

            // Caso 3: texto misto / markdown com JSON embutido
            var start = content.IndexOf('[');
            var lastEnd = content.LastIndexOf(']');
            if (start >= 0 && lastEnd > start)
                return content.Substring(start, lastEnd - start + 1);

            return null;
        }

        private sealed class AiProductItem
        {
            public string? NomeProduto  { get; set; }
            public string? PrecoOferta  { get; set; }
            public string? Descricao    { get; set; }
            public string? ImagemUrl    { get; set; }
            public string? Loja         { get; set; }
            public string? LinkProduto  { get; set; }
        }
    }
}