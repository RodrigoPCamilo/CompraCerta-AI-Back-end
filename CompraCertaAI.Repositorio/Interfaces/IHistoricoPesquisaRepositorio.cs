using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;

namespace CompraCertaAI.Repositorio.Interfaces
{
    public interface IHistoricoPesquisaRepositorio
    {
        Task<int> AdicionarPesquisa(int usuarioId, string query);
        Task<IEnumerable<HistoricoPesquisa>> ObterHistoricoPorUsuario(int usuarioId);

        Task<int> AdicionarPesquisaAsync(int usuarioId, string query);
        Task<IEnumerable<HistoricoPesquisa>> ObterHistoricoPorUsuarioAsync(int usuarioId);

        // Compatibilidade com implementações existentes
        Task<int> AdicionarInteracaoAsync(HistoricoPesquisa historico);
        Task<IEnumerable<HistoricoPesquisa>> HistoricoDeUsuarioAsync(int usuarioId);
    }
}