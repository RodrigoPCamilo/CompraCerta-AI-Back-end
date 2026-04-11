using System.Security.Claims;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Produto;
using CompraCertaAI.Aplicacao.Interfaces;
using CompraCertaAI.API.Models.Produto.Requisicao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompraCertaAI.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IRecomendacaoService _recomendacaoService;
        private readonly IBuscaProdutoAplicacao _buscaProdutoAplicacao;
        private readonly ICategoriaAplicacao _categoriaAplicacao;

        public ProductsController(
            IRecomendacaoService recomendacaoService,
            IBuscaProdutoAplicacao buscaProdutoAplicacao,
            ICategoriaAplicacao categoriaAplicacao)
        {
            _recomendacaoService = recomendacaoService;
            _buscaProdutoAplicacao = buscaProdutoAplicacao;
            _categoriaAplicacao = categoriaAplicacao;
        }

        /// <summary>
        /// Retorna até 10 produtos recomendados com base nas categorias favoritas do usuário autenticado.
        /// </summary>
        [HttpGet("recommendations")]
        public async Task<IActionResult> Recommendations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var id = int.Parse(userId);

            var categorias = await _categoriaAplicacao.ObterCategoriasPorUsuarioAsync(id);
            var categoriaIds = System.Linq.Enumerable.Select(categorias, c => c.Id);

            var produtos = await _recomendacaoService.ObterRecomendacoesAsync(categoriaIds, 10);

            return Ok(produtos);
        }

        /// <summary>
        /// Busca produtos por nome e/ou categoria. Registra a busca no histórico automaticamente.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] BuscaProdutosRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Query) && !request.CategoriaId.HasValue)
                return BadRequest("Informe ao menos um critério de busca: query ou categoriaId.");

            var dto = new BuscaProdutosDto
            {
                Query = request.Query,
                CategoriaId = request.CategoriaId,
                UsuarioId = int.Parse(userId)
            };

            var resultado = await _buscaProdutoAplicacao.BuscarAsync(dto);
            return Ok(resultado);
        }
    }
}
