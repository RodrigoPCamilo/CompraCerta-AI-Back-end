using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CompraCertaAI.API.Models.Login.Requisicao;
using CompraCertaAI.Aplicacao.DTOs.Login;
using CompraCertaAI.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace CompraCertaAI.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthAplicacao _authAplicacao;
        public AuthController(IAuthAplicacao authAplicacao)
        {
            _authAplicacao = authAplicacao;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authAplicacao.LoginAsync(new LoginDto
            {
                Email = request.Email,
                Senha = request.Senha
            });

            return Ok( response );
        }
    }
}
    