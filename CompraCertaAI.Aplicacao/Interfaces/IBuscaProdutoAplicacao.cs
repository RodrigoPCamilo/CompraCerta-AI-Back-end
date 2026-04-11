using System.Collections.Generic;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Produto;

namespace CompraCertaAI.Aplicacao.Interfaces
{
    public interface IBuscaProdutoAplicacao
    {
        Task<IEnumerable<ProdutoDTO>> BuscarAsync(BuscaProdutosDto dto);
    }
}
