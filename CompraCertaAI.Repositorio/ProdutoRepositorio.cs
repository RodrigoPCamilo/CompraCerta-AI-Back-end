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
