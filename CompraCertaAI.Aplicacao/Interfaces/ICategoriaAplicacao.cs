using System.Collections.Generic;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Categoria;

namespace CompraCertaAI.Aplicacao.Interfaces
{
    public interface ICategoriaAplicacao
    {
        Task<IEnumerable<CategoriaDto>> ListarAtivasAsync();
        Task<IEnumerable<CategoriaDto>> ObterCategoriasPorUsuarioAsync(int usuarioId);
        Task AtualizarCategoriasPorUsuarioAsync(int usuarioId, List<int> categoriaIds);
    }
}
