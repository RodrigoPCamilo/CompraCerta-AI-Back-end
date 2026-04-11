using System.Collections.Generic;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Produto;
using CompraCertaAI.Aplicacao.Interfaces;

namespace CompraCertaAI.Aplicacao.Aplicacao
{
    public class BuscaProdutoAplicacao : IBuscaProdutoAplicacao
    {
        private readonly IBuscaProdutoService _buscaProdutoService;

        public BuscaProdutoAplicacao(IBuscaProdutoService buscaProdutoService)
        {
            _buscaProdutoService = buscaProdutoService;
        }

        public async Task<IEnumerable<ProdutoDTO>> BuscarAsync(BuscaProdutosDto dto)
        {
            return await _buscaProdutoService.BuscarAsync(dto.Query, dto.CategoriaId, dto.UsuarioId);
        }
    }
}
