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
    public class UsuarioCategoriaRepositorio : BaseRepositorio, IUsuarioCategoriaRepositorio
    {
        public UsuarioCategoriaRepositorio(IDbConnection connection) : base(connection) { }

        public async Task<IEnumerable<Categoria>> ObterCategoriasPorUsuarioAsync(int usuarioId)
        {
            return await _connection.QueryAsync<Categoria>(
                                "spUsuarioCategoriaObterPorUsuario",
                                new { UsuarioId = usuarioId },
                                commandType: CommandType.StoredProcedure
            );
        }

        public async Task AtualizarCategoriasPorUsuarioAsync(int usuarioId, List<int> categoriaIds)
        {
            if (categoriaIds.Count > 5)
                throw new ArgumentException("Máximo de 5 categorias favoritas permitidas.");

            await _connection.ExecuteAsync(
                "spUsuarioCategoriaAtualizar",
                new
                {
                    UsuarioId = usuarioId,
                    CategoriaIds = string.Join(",", categoriaIds.Distinct()),
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
