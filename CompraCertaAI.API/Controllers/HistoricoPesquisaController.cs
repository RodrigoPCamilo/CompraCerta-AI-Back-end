using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Historico;
using CompraCertaAI.API.Models.Historico.Requisicao;
using CompraCertaAI.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompraCertaAI.API.Controllers
{
    [ApiController]
    [Route("api/historico-pesquisa")]
    [Authorize]
    public class HistoricoPesquisaController : ControllerBase
    {
        private readonly IHistoricoPesquisaService _historicoPesquisaService;

        public HistoricoPesquisaController(IHistoricoPesquisaService historicoPesquisaService)
        {
            _historicoPesquisaService = historicoPesquisaService;
        }

        /// <summary>
        /// Retorna o histórico de pesquisas do usuário autenticado.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Listar()
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

        /// <summary>
        /// Registra manualmente uma nova busca no histórico do usuário autenticado.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RegistrarBuscaRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Query))
                return BadRequest("A query não pode ser vazia.");

            await _historicoPesquisaService.RegistrarPesquisaAsync(int.Parse(userId), request.Query);

            return Ok(new { mensagem = "Busca registrada com sucesso." });
        }
    }
}
