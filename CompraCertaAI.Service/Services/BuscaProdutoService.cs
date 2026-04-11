using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Produto;
using CompraCertaAI.Dominio.Entidades;
using CompraCertaAI.Repositorio.Interfaces;
using CompraCertaAI.Aplicacao.Interfaces;
using CompraCertaAI.Service.Interface;
using CompraCertaAI.Service.Models;

namespace CompraCertaAI.Service.Services
{
    public class BuscaProdutoService : IBuscaProdutoService
    {
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IHistoricoPesquisaRepositorio _historicoPesquisaRepositorio;
        private readonly IIAService _iaService;

        public BuscaProdutoService(
            IProdutoRepositorio produtoRepositorio,
            ICategoriaRepositorio categoriaRepositorio,
            IHistoricoPesquisaRepositorio historicoPesquisaRepositorio,
            IIAService iaService)
        {
            _produtoRepositorio = produtoRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
            _historicoPesquisaRepositorio = historicoPesquisaRepositorio;
            _iaService = iaService;
        }

        public async Task<IEnumerable<ProdutoDTO>> BuscarAsync(string query, int? categoriaId, int usuarioId)
        {
            var categorias = await _categoriaRepositorio.ListarAtivasAsync();
            var mapeamentoCategorias = categorias.ToDictionary(c => c.Id, c => c.Nome);

            IEnumerable<ProdutoDTO> resultado;

            try
            {
                if (!string.IsNullOrWhiteSpace(query))
                {
                    var prompt = AiPromptTemplates.BuildSearchPrompt(query);
                    var aiResponse = await _iaService.GetAiResponseAsync(prompt);
                    var aiProducts = AiProductParser.ParseProducts(aiResponse, 10);

                    if (aiProducts.Count > 0)
                    {
                        resultado = aiProducts;
                    }
                    else
                    {
                        var produtosFallback = await _produtoRepositorio.BuscarAsync(query, categoriaId);
                        resultado = produtosFallback
                            .Take(10)
                            .Select(p => new ProdutoDTO
                            {
                                Id = p.Id,
                                NomeProduto = p.NomeProduto,
                                Descricao = p.Descricao,
                                ImagemUrl = p.ImagemUrl,
                                Loja = p.Loja,
                                LinkProduto = p.LinkProduto,
                                CategoriaNome = mapeamentoCategorias.ContainsKey(p.CategoriaId)
                                    ? mapeamentoCategorias[p.CategoriaId]
                                    : string.Empty
                            });
                    }
                }
                else
                {
                    var produtosFallback = await _produtoRepositorio.BuscarAsync(query, categoriaId);
                    resultado = produtosFallback
                        .Take(10)
                        .Select(p => new ProdutoDTO
                        {
                            Id = p.Id,
                            NomeProduto = p.NomeProduto,
                            Descricao = p.Descricao,
                            ImagemUrl = p.ImagemUrl,
                            Loja = p.Loja,
                            LinkProduto = p.LinkProduto,
                            CategoriaNome = mapeamentoCategorias.ContainsKey(p.CategoriaId)
                                ? mapeamentoCategorias[p.CategoriaId]
                                : string.Empty
                        });
                }
            }
            catch
            {
                var produtosFallback = await _produtoRepositorio.BuscarAsync(query, categoriaId);
                resultado = produtosFallback
                    .Take(10)
                    .Select(p => new ProdutoDTO
                    {
                        Id = p.Id,
                        NomeProduto = p.NomeProduto,
                        Descricao = p.Descricao,
                        ImagemUrl = p.ImagemUrl,
                        Loja = p.Loja,
                        LinkProduto = p.LinkProduto,
                        CategoriaNome = mapeamentoCategorias.ContainsKey(p.CategoriaId)
                            ? mapeamentoCategorias[p.CategoriaId]
                            : string.Empty
                    });
            }

            // Registra busca no histórico automaticamente
            if (!string.IsNullOrWhiteSpace(query))
            {
                await _historicoPesquisaRepositorio.AdicionarPesquisaAsync(usuarioId, query);
            }

            return resultado;
        }
    }
}
