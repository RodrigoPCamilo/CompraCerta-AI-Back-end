using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Login;

namespace CompraCertaAI.Aplicacao.Interfaces
{
    public interface IAuthAplicacao
    {
       Task<AuthResponseDTO> LoginAsync(LoginDto dto);

    }
}