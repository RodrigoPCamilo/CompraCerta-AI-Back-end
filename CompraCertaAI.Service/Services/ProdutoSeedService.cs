using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;
using CompraCertaAI.Repositorio.Interfaces;
using CompraCertaAI.Service.Interface;
using CompraCertaAI.Service.Models;
using Microsoft.Extensions.Logging;

namespace CompraCertaAI.Service.Services
{
    public class ProdutoSeedService : IProdutoSeedService
    {
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IIAService _iaService;
        private readonly ILogger<ProdutoSeedService> _logger;

        public ProdutoSeedService(
            IProdutoRepositorio produtoRepositorio,
            ICategoriaRepositorio categoriaRepositorio,
            IIAService iaService,
            ILogger<ProdutoSeedService> logger)
        {
            _produtoRepositorio = produtoRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
            _iaService = iaService;
            _logger = logger;
        }

        public async Task SeedProdutosIniciaisAsync(int minimoPorCategoria = 10)
        {
            if (minimoPorCategoria <= 0)
                minimoPorCategoria = 10;

            var categorias = (await _categoriaRepositorio.ListarAtivasAsync()).ToList();
            if (!categorias.Any())
            {
                _logger.LogWarning("Seed de produtos ignorado: nenhuma categoria ativa encontrada.");
                return;
            }

            _logger.LogInformation("Iniciando seed de produtos por IA. Categorias ativas: {TotalCategorias}", categorias.Count);

            foreach (var categoria in categorias)
            {
                var quantidadeAtual = await _produtoRepositorio.ContarAtivosPorCategoriaAsync(categoria.Id);
                if (quantidadeAtual >= minimoPorCategoria)
                {
                    _logger.LogInformation(
                        "Categoria {CategoriaId} já possui {QuantidadeAtual} produtos ativos. Sem necessidade de seed.",
                        categoria.Id,
                        quantidadeAtual);
                    continue;
                }

                var faltantes = minimoPorCategoria - quantidadeAtual;
                _logger.LogInformation(
                    "Categoria {CategoriaId} ({CategoriaNome}) precisa de {Faltantes} produto(s). Gerando via IA...",
                    categoria.Id,
                    categoria.Nome,
                    faltantes);

                try
                {
                    var prompt = AiPromptTemplates.BuildCategorySeedPrompt(categoria.Nome, faltantes);
                    var aiResponse = await _iaService.GetAiResponseAsync(prompt);
                    var produtosGerados = string.IsNullOrWhiteSpace(aiResponse)
                        ? new System.Collections.Generic.List<CompraCertaAI.Aplicacao.DTOs.Produto.ProdutoDTO>()
                        : AiProductParser.ParseProducts(aiResponse, faltantes);

                    if (produtosGerados.Count == 0)
                    {
                        _logger.LogWarning(
                            "IA não retornou produtos válidos para categoria {CategoriaId} ({CategoriaNome}). Pulando.",
                            categoria.Id, categoria.Nome);
                        continue;
                    }

                    _logger.LogInformation(
                        "IA gerou {Count} produto(s) para categoria {CategoriaId} ({CategoriaNome}).",
                        produtosGerados.Count, categoria.Id, categoria.Nome);

                    var produtosEntidade = new List<Produto>();
                    foreach (var produto in produtosGerados)
                    {
                        try
                        {
                            produtosEntidade.Add(new Produto(
                                produto.NomeProduto,
                                produto.PrecoOferta ?? string.Empty,
                                produto.Descricao ?? string.Empty,
                                produto.ImagemUrl ?? string.Empty,
                                produto.Loja,
                                produto.LinkProduto,
                                categoria.Id));
                        }
                        catch (ArgumentException ex)
                        {
                            _logger.LogWarning(
                                ex,
                                "Produto inválido descartado no seed da categoria {CategoriaId}: {NomeProduto}",
                                categoria.Id,
                                produto.NomeProduto);
                        }
                    }

                    var inseridos = await _produtoRepositorio.InserirNovosAsync(produtosEntidade);
                    _logger.LogInformation(
                        "Seed concluído para categoria {CategoriaId} ({CategoriaNome}). Inseridos: {Inseridos}.",
                        categoria.Id, categoria.Nome, inseridos);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "Falha ao gerar seed para categoria {CategoriaId} ({CategoriaNome}): {Mensagem}",
                        categoria.Id, categoria.Nome, ex.Message);
                }
            }

            _logger.LogInformation("Seed de produtos por IA finalizado.");
        }
    }
}
