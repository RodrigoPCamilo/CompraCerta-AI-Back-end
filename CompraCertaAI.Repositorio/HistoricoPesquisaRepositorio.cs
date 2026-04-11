using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;
using CompraCertaAI.Repositorio.Interfaces;
using Dapper;

namespace CompraCertaAI.Repositorio
{
    public class HistoricoPesquisaRepositorio : BaseRepositorio, IHistoricoPesquisaRepositorio
    {
        public HistoricoPesquisaRepositorio(IDbConnection connection)
            : base(connection)
        {
        }

        public async Task<int> AdicionarPesquisaAsync(int usuarioId, string query)
        {
            return await _connection.QuerySingleAsync<int>(
                "spRegistraHistoricoPesquisa",
                new
                {
                    UsuarioId = usuarioId,
                    Query = query,
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public Task<int> AdicionarPesquisa(int usuarioId, string query)
        {
            return AdicionarPesquisaAsync(usuarioId, query);
        }

        public async Task<IEnumerable<HistoricoPesquisa>> ObterHistoricoPorUsuarioAsync(int usuarioId)
        {
            return await _connection.QueryAsync<HistoricoPesquisa>(
                "spListarHistoricoPorUsuario",
                new { UsuarioId = usuarioId },
                commandType: CommandType.StoredProcedure
            );
        }

        public Task<IEnumerable<HistoricoPesquisa>> ObterHistoricoPorUsuario(int usuarioId)
        {
            return ObterHistoricoPorUsuarioAsync(usuarioId);
        }

        public Task<int> AdicionarInteracaoAsync(HistoricoPesquisa historico)
        {
            return AdicionarPesquisaAsync(historico.UsuarioId, historico.Query);
        }

        public Task<IEnumerable<HistoricoPesquisa>> HistoricoDeUsuarioAsync(int usuarioId)
        {
            return ObterHistoricoPorUsuarioAsync(usuarioId);
        }
    }
}