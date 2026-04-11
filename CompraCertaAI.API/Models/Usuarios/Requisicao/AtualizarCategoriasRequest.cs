using System.Collections.Generic;

namespace CompraCertaAI.API.Models.Usuarios.Requisicao
{
    public class AtualizarCategoriasRequest
    {
        public List<int> CategoriaIds { get; set; } = new List<int>();
    }
}
