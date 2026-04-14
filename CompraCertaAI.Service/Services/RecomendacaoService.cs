using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Produto;
using CompraCertaAI.Repositorio.Interfaces;
using CompraCertaAI.Aplicacao.Interfaces;
using CompraCertaAI.Service.Interface;
using CompraCertaAI.Service.Models;

namespace CompraCertaAI.Service.Services
{
    public class RecomendacaoService : IRecomendacaoService
    {
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IIAService _iaService;

        public RecomendacaoService(
            IProdutoRepositorio produtoRepositorio,
            ICategoriaRepositorio categoriaRepositorio,
            IIAService iaService)
        {
            _produtoRepositorio = produtoRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
            _iaService = iaService;
        }

        public async Task<IEnumerable<ProdutoDTO>> ObterRecomendacoesAsync(IEnumerable<int> categoriaIds, int limite = 10)
        {
            if (limite <= 0)
                return Enumerable.Empty<ProdutoDTO>();

            var ids = categoriaIds.ToList();
            if (!ids.Any())
                return Enumerable.Empty<ProdutoDTO>();

            var categorias = await _categoriaRepositorio.ObterPorIdsAsync(ids);
            var mapeamentoCategorias = categorias.ToDictionary(c => c.Id, c => c.Nome);

            try
            {
                var categoriasNomes = categorias.Select(c => c.Nome);
                var prompt = AiPromptTemplates.BuildRecommendationPrompt(categoriasNomes);
                var aiResponse = await _iaService.GetAiResponseAsync(prompt);
                var aiProducts = AiProductParser.ParseProducts(aiResponse, limite);

                if (aiProducts.Count > 0)
                    return aiProducts;
            }
            catch
            {
                // fallback para o banco
            }

            var produtos = await _produtoRepositorio.ObterRecomendacoesAsync(ids, limite);

            return produtos.Select(p => new ProdutoDTO
            {
                Id            = p.Id,
                NomeProduto   = p.NomeProduto,
                PrecoOferta   = p.PrecoOferta,
                Descricao     = p.Descricao,
                ImagemUrl     = ProdutoUrlHelper.NormalizeImageUrl(p.ImagemUrl, p.NomeProduto),
                Loja          = p.Loja,
                LinkProduto   = ProdutoUrlHelper.NormalizeProductLink(p.LinkProduto, p.NomeProduto, p.Loja),
                CategoriaNome = mapeamentoCategorias.ContainsKey(p.CategoriaId)
                    ? mapeamentoCategorias[p.CategoriaId]
                    : string.Empty
            });
        }
    }
}