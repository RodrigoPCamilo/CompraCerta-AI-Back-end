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
    public class BuscaProdutoService : IBuscaProdutoService
    {
        private readonly IProdutoRepositorio           _produtoRepositorio;
        private readonly ICategoriaRepositorio         _categoriaRepositorio;
        private readonly IHistoricoPesquisaRepositorio _historicoPesquisaRepositorio;
        private readonly IIAService                    _iaService;
        private readonly ILogger<BuscaProdutoService>  _logger;

        public BuscaProdutoService(
            IProdutoRepositorio           produtoRepositorio,
            ICategoriaRepositorio         categoriaRepositorio,
            IHistoricoPesquisaRepositorio historicoPesquisaRepositorio,
            IIAService                    iaService,
            ILogger<BuscaProdutoService>  logger)
        {
            _produtoRepositorio           = produtoRepositorio;
            _categoriaRepositorio         = categoriaRepositorio;
            _historicoPesquisaRepositorio = historicoPesquisaRepositorio;
            _iaService                    = iaService;
            _logger                       = logger;
        }

        public async Task<IEnumerable<ProdutoDTO>> BuscarAsync(
            string query, int? categoriaId, int usuarioId)
        {
            IEnumerable<ProdutoDTO> resultado;

            if (!string.IsNullOrWhiteSpace(query))
            {
                _logger.LogInformation("Busca IA: '{Query}'", query);
                try
                {
                    var prompt     = AiPromptTemplates.BuildSearchPrompt(query);
                    var aiResponse = await _iaService.GetAiResponseAsync(prompt);

                    // Passa a query original para que os links redirecionem
                    // para a busca exata do que o usuário digitou na loja
                    var aiProducts = AiProductParser.ParseProducts(
                        aiResponse, 12, queryOriginal: query);

                    _logger.LogInformation("Busca '{Query}': {Count} produtos", query, aiProducts.Count);

                    resultado = aiProducts.Count > 0
                        ? aiProducts
                        : await BuscarNoBancoAsync(query, categoriaId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro IA busca '{Query}'", query);
                    resultado = await BuscarNoBancoAsync(query, categoriaId);
                }
            }
            else
            {
                resultado = await BuscarNoBancoAsync(query, categoriaId);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                try { await _historicoPesquisaRepositorio.AdicionarPesquisaAsync(usuarioId, query); }
                catch (Exception ex) { _logger.LogWarning(ex, "Falha histórico '{Query}'", query); }
            }

            return resultado;
        }

        private async Task<IEnumerable<ProdutoDTO>> BuscarNoBancoAsync(
            string query, int? categoriaId)
        {
            var categorias = await _categoriaRepositorio.ListarAtivasAsync();
            var mapeamento = categorias.ToDictionary(c => c.Id, c => c.Nome);
            var produtos   = await _produtoRepositorio.BuscarAsync(query, categoriaId);

            return produtos.Take(12).Select(p => new ProdutoDTO
            {
                Id            = p.Id,
                NomeProduto   = p.NomeProduto,
                PrecoOferta   = string.IsNullOrWhiteSpace(p.PrecoOferta)
                    ? "Consulte na loja" : p.PrecoOferta,
                PrecoOriginal = string.Empty,
                Desconto      = string.Empty,
                Descricao     = p.Descricao,
                ImagemUrl     = string.Empty,
                Loja          = p.Loja,
                LinkProduto   = ProdutoUrlHelper.NormalizeProductLink(
                    p.LinkProduto, p.NomeProduto, p.Loja),
                CategoriaNome = mapeamento.ContainsKey(p.CategoriaId)
                    ? mapeamento[p.CategoriaId] : string.Empty
            });
        }
    }
}