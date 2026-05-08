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

            var ids = categoriaIds?.ToList() ?? new List<int>();

            List<string> categoriasNomes;
            if (ids.Any())
            {
                var cats = await _categoriaRepositorio.ObterPorIdsAsync(ids);
                categoriasNomes = cats
                    .Select(c => c.Nome)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .ToList();
            }
            else
            {
                categoriasNomes = new List<string> { "Tecnologia" };
            }

            var temaLog = string.Join(", ", categoriasNomes);
            _logger.LogInformation("Recomendações IA para categorias: '{Tema}'", temaLog);

            var resultado = new List<ProdutoDTO>();

            try
            {
                var prompt     = AiPromptTemplates.BuildRecommendationPrompt(categoriasNomes);
                var aiResponse = await _iaService.GetAiResponseAsync(prompt);
                var aiProducts = AiProductParser.ParseProducts(aiResponse, limite);

                var filtrados  = FiltrarPorCategorias(aiProducts, categoriasNomes);

                _logger.LogInformation(
                    "IA retornou {Total}, {Filtrados} na categoria '{Tema}'",
                    aiProducts.Count, filtrados.Count, temaLog);

                resultado.AddRange(filtrados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha IA para categorias '{Tema}'", temaLog);
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

            _logger.LogInformation("Total recomendações retornadas: {Count}", resultado.Count);
            return resultado;
        }
        private static readonly Dictionary<string, string[]> AllowlistPorCategoria =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Livros"] = new[] {
                "livro", "romance", "ficção", "fantasia", "autoajuda", "biografia",
                "mangá", "manga", "hq ", "quadrinhos", "thriller", "poesia", "literatura",
                "harry potter", "rowling", "tolkien", "stephen king", "agatha christie",
                "sapiens", "hábitos atômicos", "clean code", "código limpo",
                "vol.", "volume ", "edição", "editora", "kindle", "e-reader",
                "1984", "george orwell", "aldous huxley", "kafka", "cervantes",
                "mark manson", "brené brown", "eckhart tolle", "yuval harari",
                "james clear", "charles duhigg", "simon sinek", "malcolm gladwell",
                "ryan holiday", "nassim taleb", "cal newport",
                "dom quixote", "o alquimista", "o pequeno príncipe", "dom casmurro",
                "sherlock holmes", "hercule poirot", "arsène lupin",
                "senhor dos anéis", "hobbit", "duna", "fundação",
                "jogos vorazes", "divergente", "maze runner",
                "pedro bandeira", "monteiro lobato", "érico veríssimo",
                "a arte da guerra", "o príncipe", "tao te ching",
                "a cabana", "veronika decide morrer", "o médico de família",
                "attack on titan", "demon slayer", "one piece", "naruto",
                "dragon ball", "jujutsu", "death note", "fullmetal", "berserk",
            },
        };

        private static readonly Dictionary<string, string[]> BlocklistPorCategoria =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Games"] = new[] {
                "livro","romance","mangá","hq","whey protein","creatina",
                "kettlebell","halter","esteira","airfryer","geladeira","lavadora",
                "camiseta","vestido","calça jeans","perfume",
                "smartphone","celular","moto g","redmi note","galaxy a",
            },
            ["Tecnologia"] = new[] {
                "livro","romance","mangá","hq","quadrinhos",
                "kettlebell","halter","whey protein","creatina","esteira",
                "airfryer","geladeira","lavadora","microondas",
                "vestido","calça jeans","camiseta","moletom",
                "perfume","eau de parfum","maquiagem",
            },
            ["Fitness"] = new[] {
                "livro","romance","mangá","hq",
                "playstation","ps5","xbox","nintendo switch","console","controle gamer",
                "airfryer","geladeira","lavadora","microondas","cafeteira",
                "smartphone","notebook","tablet","smart tv","airpods",
                "vestido","calça jeans","perfume",
            },
            ["Casa"] = new[] {
                "livro","romance","mangá","hq",
                "playstation","xbox","nintendo","console de jogos","controle gamer",
                "whey protein","creatina","kettlebell","halter","esteira",
                "smartphone","notebook","tablet","airpods",
            },
            ["Moda"] = new[] {
                "livro","romance","mangá","hq",
                "playstation","xbox","nintendo","console","controle gamer",
                "whey protein","creatina","kettlebell","halter","esteira",
                "airfryer","geladeira","lavadora","microondas",
                "smartphone","notebook","tablet","airpods","smart tv",
            },
            ["Beleza"] = new[] {
                "livro","romance","mangá","hq",
                "playstation","xbox","nintendo","console","controle gamer",
                "whey protein","creatina","kettlebell","halter","esteira",
                "airfryer","geladeira","lavadora","microondas",
                "smartphone","notebook","tablet","airpods","smart tv",
            },
            ["Esportes"] = new[] {
                "livro","romance","mangá","hq",
                "playstation","xbox","nintendo","console de jogos",
                "airfryer","geladeira","lavadora","microondas",
                "smartphone","notebook","tablet","smart tv","airpods",
                "perfume","maquiagem",
            },
        };

        private IReadOnlyList<ProdutoDTO> FiltrarPorCategorias(
            IReadOnlyList<ProdutoDTO> produtos, List<string> categorias)
        {
            if (!produtos.Any() || !categorias.Any()) return produtos;

            var filtrados = new List<ProdutoDTO>();

            foreach (var p in produtos)
            {
                var texto = $"{p.NomeProduto} {p.Descricao}".ToLowerInvariant();
                bool aceito = true;

                foreach (var cat in categorias)
                {
                    if (AllowlistPorCategoria.TryGetValue(cat, out var allowed))
                    {
                        if (!allowed.Any(t => texto.Contains(t.ToLowerInvariant())))
                        {
                            aceito = false;
                            _logger.LogInformation(
                                "Produto '{Nome}' rejeitado — não é da categoria {Cat}",
                                p.NomeProduto, cat);
                            break;
                        }
                    }
                    else if (BlocklistPorCategoria.TryGetValue(cat, out var blocked))
                    {
                        if (blocked.Any(t => texto.Contains(t.ToLowerInvariant())))
                        {
                            aceito = false;
                            _logger.LogInformation(
                                "Produto '{Nome}' rejeitado — fora da categoria {Cat}",
                                p.NomeProduto, cat);
                            break;
                        }
                    }
                }

                if (aceito) filtrados.Add(p);
            }

            if (!filtrados.Any())
                _logger.LogWarning(
                    "FiltrarPorCategorias: todos os {Count} produtos rejeitados para '{Cats}'.",
                    produtos.Count, string.Join(", ", categorias));

            return filtrados;
        }

    }
}