using System.Collections.Generic;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;

namespace CompraCertaAI.Repositorio.Interfaces
{
    public interface IUsuarioCategoriaRepositorio
    {
        Task<IEnumerable<Categoria>> ObterCategoriasPorUsuarioAsync(int usuarioId);
        Task AtualizarCategoriasPorUsuarioAsync(int usuarioId, System.Collections.Generic.List<int> categoriaIds);
    }
}
