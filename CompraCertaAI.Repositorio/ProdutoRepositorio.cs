using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;
using CompraCertaAI.Repositorio.Interfaces;
using Dapper;

namespace CompraCertaAI.Repositorio
{
    public class ProdutoRepositorio : BaseRepositorio, IProdutoRepositorio
    {
        public ProdutoRepositorio(IDbConnection connection) : base(connection) { }

        public async Task<int> ContarAtivosPorCategoriaAsync(int categoriaId)
        {
            const string sql = @"
SELECT COUNT(1)
FROM Produtos
WHERE CategoriaId = @CategoriaId
  AND Ativo = 1;";

            return await _connection.ExecuteScalarAsync<int>(sql, new { CategoriaId = categoriaId });
        }

        public async Task<int> InserirNovosAsync(IEnumerable<Produto> produtos)
        {
            var lista = produtos?.ToList() ?? new List<Produto>();
            if (!lista.Any())
                return 0;

            const string sql = @"
INSERT INTO Produtos (NomeProduto, PrecoOferta, Descricao, ImagemUrl, Loja, LinkProduto, CategoriaId, Ativo)
SELECT @NomeProduto, @PrecoOferta, @Descricao, @ImagemUrl, @Loja, @LinkProduto, @CategoriaId, 1
WHERE NOT EXISTS
(
    SELECT 1
    FROM Produtos
    WHERE NomeProduto = @NomeProduto
      AND Loja = @Loja
      AND LinkProduto = @LinkProduto
);";

            return await _connection.ExecuteAsync(sql, lista.Select(p => new
            {
                p.NomeProduto,
                p.PrecoOferta,
                p.Descricao,
                p.ImagemUrl,
                p.Loja,
                p.LinkProduto,
                p.CategoriaId
            }));
        }

        public async Task<IEnumerable<Produto>> BuscarAsync(string query, int? categoriaId)
        {
            return await _connection.QueryAsync<Produto>(
                "spProdutoBuscar",
                new
                {
                    Query = string.IsNullOrWhiteSpace(query) ? null : query,
                    CategoriaId = categoriaId,
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Produto>> ObterRecomendacoesAsync(IEnumerable<int> categoriaIds, int limite = 10)
        {
            var ids = categoriaIds.ToList();
            if (!ids.Any())
                return Enumerable.Empty<Produto>();

            return await _connection.QueryAsync<Produto>(
                "spProdutoObterRecomendacoes",
                new
                {
                    CategoriaIds = string.Join(",", ids.Distinct()),
                    Limite = limite,
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
