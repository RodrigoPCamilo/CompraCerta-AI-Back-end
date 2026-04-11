using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;
using CompraCertaAI.Repositorio.Interfaces;
using Dapper;

namespace CompraCertaAI.Repositorio
{
    public class CategoriaRepositorio : BaseRepositorio, ICategoriaRepositorio
    {
        public CategoriaRepositorio(IDbConnection connection) : base(connection) { }

        public async Task<IEnumerable<Categoria>> ListarAtivasAsync()
        {
            return await _connection.QueryAsync<Categoria>(
                "spCategoriaListarAtivas",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Categoria> ObterPorIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<Categoria>(
                "spCategoriaObterPorId",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Categoria>> ObterPorIdsAsync(IEnumerable<int> ids)
        {
            var idsLista = ids?.Distinct().ToList() ?? new List<int>();
            if (!idsLista.Any())
                return new List<Categoria>();

            return await _connection.QueryAsync<Categoria>(
                "spCategoriaObterPorIds",
                new { Ids = string.Join(",", idsLista) },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
