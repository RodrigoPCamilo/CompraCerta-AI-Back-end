using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CompraCertaAI.API.Models.Usuarios.Requisicao;
using CompraCertaAI.API.Models.Usuarios.Resposta;
using CompraCertaAI.Aplicacao.DTOs.Usuario;
using CompraCertaAI.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompraCertaAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioAplicacao _usuarioService;
        private readonly ICategoriaAplicacao _categoriaAplicacao;

        public UsuarioController(IUsuarioAplicacao usuarioService, ICategoriaAplicacao categoriaAplicacao)
        {
            _usuarioService = usuarioService;
            _categoriaAplicacao = categoriaAplicacao;
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Criar([FromBody] CriarUsuarioDto dto)
        {
            var usuario = await _usuarioService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);
            return Ok(usuario);
        }

            [Authorize]
            [HttpGet("perfil")]
            public async Task<IActionResult> Perfil()
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var usuario = await _usuarioService.ObterPerfilAsync(int.Parse(userId));
                return Ok(usuario);
            }

            [Authorize]
            [HttpPut]
            public async Task<IActionResult> Atualizar([FromBody] AtualizarUsuarioDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var usuarioAtualizado = await _usuarioService.AtualizarAsync(
                int.Parse(userId),
                dto
            );

            return Ok(usuarioAtualizado);
        }

            [Authorize]
            [HttpPut("categorias")]
            public async Task<IActionResult> AtualizarCategorias([FromBody] AtualizarCategoriasRequest request)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                if (request.CategoriaIds == null || request.CategoriaIds.Count > 5)
                    return BadRequest("Informe entre 0 e 5 categorias favoritas.");

                await _categoriaAplicacao.AtualizarCategoriasPorUsuarioAsync(int.Parse(userId), request.CategoriaIds);
                return Ok(new { mensagem = "Categorias atualizadas com sucesso." });
            }

            [HttpGet("categorias/disponiveis")]
            public async Task<IActionResult> CategoriasDisponiveis()
            {
                var categorias = await _categoriaAplicacao.ListarAtivasAsync();
                return Ok(categorias);
            }
    }
}