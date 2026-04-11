using System.Collections.Generic;
using CompraCertaAI.Aplicacao.DTOs.Categoria;

namespace CompraCertaAI.Aplicacao.DTOs.Usuario
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public List<CategoriaDto> Categorias { get; set; } = new List<CategoriaDto>();
    }
}