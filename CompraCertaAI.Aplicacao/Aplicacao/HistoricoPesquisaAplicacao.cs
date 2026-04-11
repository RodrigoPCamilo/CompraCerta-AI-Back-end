using CompraCertaAI.Aplicacao.DTOs.Historico;
using CompraCertaAI.Aplicacao.Interfaces;
using CompraCertaAI.Dominio.Entidades;
using CompraCertaAI.Repositorio.Interfaces;


namespace CompraCertaAI.Aplicacao.Aplicacao
{
    public class HistoricoPesquisaAplicacao : IHistoricoPesquisaAplicacao
    {
        private readonly IHistoricoPesquisaRepositorio _historicoPesquisaRepositorio;

        public HistoricoPesquisaAplicacao( IHistoricoPesquisaRepositorio historicoPesquisaRepositorio)
        {
            
            _historicoPesquisaRepositorio = historicoPesquisaRepositorio;
        }
        public async Task<int> PerguntarAsync(int usuarioId, string query)
        {
            return await _historicoPesquisaRepositorio.AdicionarPesquisaAsync(usuarioId, query);
            
        }

        public async Task<IEnumerable<HistoricoPesquisa>> ListarAsync(int usuarioId)
        {
            return await _historicoPesquisaRepositorio.ObterHistoricoPorUsuarioAsync(usuarioId);           
        }
    }
}