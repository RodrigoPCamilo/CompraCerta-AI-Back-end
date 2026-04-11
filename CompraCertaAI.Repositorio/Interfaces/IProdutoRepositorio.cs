using System.Collections.Generic;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;

namespace CompraCertaAI.Repositorio.Interfaces
{
    public interface IProdutoRepositorio
    {
        Task<IEnumerable<Produto>> BuscarAsync(string query, int? categoriaId);
        Task<IEnumerable<Produto>> ObterRecomendacoesAsync(IEnumerable<int> categoriaIds, int limite = 10);
    }
}
