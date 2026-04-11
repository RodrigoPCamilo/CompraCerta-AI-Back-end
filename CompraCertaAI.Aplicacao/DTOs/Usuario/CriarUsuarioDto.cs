using System.Collections.Generic;

namespace CompraCertaAI.Aplicacao.DTOs.Usuario
{
    public class CriarUsuarioDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public List<int> CategoriaIds { get; set; } = new List<int>();
    }

}