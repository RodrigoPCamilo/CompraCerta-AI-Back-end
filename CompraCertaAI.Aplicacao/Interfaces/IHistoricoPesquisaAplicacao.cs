using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Historico;
using CompraCertaAI.Dominio.Entidades;

namespace CompraCertaAI.Aplicacao.Interfaces
{
    public interface IHistoricoPesquisaAplicacao
    {
        Task<int> PerguntarAsync(int usuarioId, string query);
        Task<IEnumerable<HistoricoPesquisa>> ListarAsync(int usuarioId);
    }
}