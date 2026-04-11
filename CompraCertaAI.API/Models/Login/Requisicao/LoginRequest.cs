using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompraCertaAI.API.Models.Login.Requisicao
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}