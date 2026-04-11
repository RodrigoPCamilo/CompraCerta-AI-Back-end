using System.Collections.Generic;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;
using CompraCertaAI.Repositorio.Interfaces;
using CompraCertaAI.Service.Interface;

namespace CompraCertaAI.Service.Services
{
    public class HistoricoPesquisaService : IHistoricoPesquisaService
    {
        private readonly IHistoricoPesquisaRepositorio _historicoPesquisaRepositorio;

        public HistoricoPesquisaService(IHistoricoPesquisaRepositorio historicoPesquisaRepositorio)
        {
            _historicoPesquisaRepositorio = historicoPesquisaRepositorio;
        }

        public Task<int> RegistrarPesquisaAsync(int usuarioId, string query)
        {
            return _historicoPesquisaRepositorio.AdicionarPesquisaAsync(usuarioId, query);
        }

        public Task<IEnumerable<HistoricoPesquisa>> ObterHistoricoPorUsuarioAsync(int usuarioId)
        {
            return _historicoPesquisaRepositorio.ObterHistoricoPorUsuarioAsync(usuarioId);
        }
    }
}
