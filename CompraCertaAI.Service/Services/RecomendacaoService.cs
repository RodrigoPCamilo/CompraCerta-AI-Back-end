using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Produto;
using CompraCertaAI.Repositorio.Interfaces;
using CompraCertaAI.Aplicacao.Interfaces;
using CompraCertaAI.Service.Interface;
using CompraCertaAI.Service.Models;
using Microsoft.Extensions.Logging;

namespace CompraCertaAI.Service.Services
{
    public class RecomendacaoService : IRecomendacaoService
    {
        private readonly IProdutoRepositorio          _produtoRepositorio;
        private readonly ICategoriaRepositorio        _categoriaRepositorio;
        private readonly IIAService                   _iaService;
        private readonly ILogger<RecomendacaoService> _logger;

        public RecomendacaoService(
            IProdutoRepositorio          produtoRepositorio,
            ICategoriaRepositorio        categoriaRepositorio,
            IIAService                   iaService,
            ILogger<RecomendacaoService> logger)
        {
            _produtoRepositorio   = produtoRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
            _iaService            = iaService;
            _logger               = logger;
        }

        public async Task<IEnumerable<ProdutoDTO>> ObterRecomendacoesAsync(
            IEnumerable<int> categoriaIds, int limite = 12)
        {
            if (limite <= 0) limite = 12;

            string tema;
            var ids = categoriaIds?.ToList() ?? new List<int>();

            if (ids.Any())
            {
                var categorias = await _categoriaRepositorio.ObterPorIdsAsync(ids);
                var nomes = categorias
                    .Select(c => c.Nome)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .ToList();
                tema = nomes.Any() ? string.Join(", ", nomes) : "Eletrônicos";
            }
            else
            {
                tema = "Eletrônicos";
            }

            _logger.LogInformation("Recomendações IA para tema: '{Tema}'", tema);

            var resultado = new List<ProdutoDTO>();

            try
            {
                var prompt     = AiPromptTemplates.BuildRecommendationPrompt(new[] { tema });
                var aiResponse = await _iaService.GetAiResponseAsync(prompt);
                var aiProducts = AiProductParser.ParseProducts(aiResponse, limite);

                _logger.LogInformation("IA retornou {Count}/{Limite} produtos para '{Tema}'",
                    aiProducts.Count, limite, tema);

                resultado.AddRange(aiProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha IA para tema '{Tema}'", tema);
            }

            // Se IA retornou menos de 12, complementa com produtos do banco
            if (resultado.Count < limite)
            {
                var faltam = limite - resultado.Count;
                _logger.LogWarning(
                    "IA retornou apenas {Count}. Complementando com {Faltam} do banco.",
                    resultado.Count, faltam);

                var mapeamento = ids.Any()
                    ? (await _categoriaRepositorio.ObterPorIdsAsync(ids))
                        .ToDictionary(c => c.Id, c => c.Nome)
                    : new Dictionary<int, string>();

                var dbProds = await _produtoRepositorio.ObterRecomendacoesAsync(ids, faltam);
                var nomesExistentes = resultado
                    .Select(p => $"{p.NomeProduto}|{p.Loja}")
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                foreach (var p in dbProds)
                {
                    var chave = $"{p.NomeProduto}|{p.Loja}";
                    if (nomesExistentes.Contains(chave)) continue;
                    if (resultado.Count >= limite) break;

                    resultado.Add(new ProdutoDTO
                    {
                        Id            = p.Id,
                        NomeProduto   = p.NomeProduto,
                        PrecoOferta   = string.IsNullOrWhiteSpace(p.PrecoOferta)
                            ? "Consulte na loja" : p.PrecoOferta,
                        PrecoOriginal = string.Empty,
                        Desconto      = string.Empty,
                        Descricao     = p.Descricao,
                        ImagemUrl     = string.Empty, // front-end usa getProductImage
                        Loja          = p.Loja,
                        LinkProduto   = ProdutoUrlHelper.NormalizeProductLink(
                            p.LinkProduto, p.NomeProduto, p.Loja),
                        CategoriaNome = mapeamento.ContainsKey(p.CategoriaId)
                            ? mapeamento[p.CategoriaId] : string.Empty
                    });
                }
            }

            _logger.LogInformation("Total recomendações: {Count}", resultado.Count);
            return resultado;
        }
    }
}
