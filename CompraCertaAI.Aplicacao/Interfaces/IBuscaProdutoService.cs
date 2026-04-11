using System.Collections.Generic;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Produto;

namespace CompraCertaAI.Aplicacao.Interfaces
{
    public interface IBuscaProdutoService
    {
        Task<IEnumerable<ProdutoDTO>> BuscarAsync(string query, int? categoriaId, int usuarioId);
    }
}
