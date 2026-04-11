using System.Collections.Generic;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;

namespace CompraCertaAI.Service.Interface
{
    public interface IHistoricoPesquisaService
    {
        Task<int> RegistrarPesquisaAsync(int usuarioId, string query);
        Task<IEnumerable<HistoricoPesquisa>> ObterHistoricoPorUsuarioAsync(int usuarioId);
    }
}
