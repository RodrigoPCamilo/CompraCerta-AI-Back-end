using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompraCertaAI.API.Models.Usuarios.Requisicao
{
    public class AtualizarUsuarioRequest
    {
        public string Nome { get; set; }
        public string CategoriaFavorita { get; set; }
        public decimal OrcamentoMedio { get; set; }
    }
}