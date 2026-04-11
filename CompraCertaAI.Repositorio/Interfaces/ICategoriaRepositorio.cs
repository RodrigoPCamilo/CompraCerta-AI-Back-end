using System.Collections.Generic;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;

namespace CompraCertaAI.Repositorio.Interfaces
{
    public interface ICategoriaRepositorio
    {
        Task<IEnumerable<Categoria>> ListarAtivasAsync();
        Task<Categoria> ObterPorIdAsync(int id);
        Task<IEnumerable<Categoria>> ObterPorIdsAsync(IEnumerable<int> ids);
    }
}
