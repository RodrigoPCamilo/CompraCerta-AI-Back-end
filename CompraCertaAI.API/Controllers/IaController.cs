using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Historico;
using CompraCertaAI.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompraCertaAI.API.Controllers
{
    [ApiController]
    [Route("api/ia")]
    [Authorize]
    public class IaController : ControllerBase
    {
        private readonly IHistoricoPesquisaService _historicoPesquisaService;

        public IaController(IHistoricoPesquisaService historicoPesquisaService)
        {
            _historicoPesquisaService = historicoPesquisaService;
        }

        [HttpGet("historico")]
        public async Task<IActionResult> Historico()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var historico = await _historicoPesquisaService.ObterHistoricoPorUsuarioAsync(int.Parse(userId));

            return Ok(historico.Select(h => new HistoricoPesquisaDto
            {
                Id = h.Id,
                Query = h.Query,
                SearchDate = h.SearchDate
            }));
        }
    }
}