using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompraCertaAI.Aplicacao.DTOs.Login
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public DateTime ExpiraEm { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
    }
}