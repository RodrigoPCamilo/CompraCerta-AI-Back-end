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
    public class UsuarioRepositorio : BaseRepositorio, IUsuarioRepositorio
    {
        public UsuarioRepositorio(IDbConnection connection) : base(connection)
        {
        }

        public async Task CriarAsync(Usuario usuario)
        {
            var id = await _connection.QuerySingleAsync<int>(
                "spUsuarioCriar",
                new
                {
                    usuario.Nome,
                    usuario.Email,
                    usuario.SenhaHash,
                },
                commandType: CommandType.StoredProcedure
            );

            usuario.Id = id;
        }

        public async Task AtualizarAsync(Usuario usuario)
        {
            await _connection.ExecuteAsync(
                "spUsuarioAtualizar",
                new
                {
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email,
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Usuario> ObterPorIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<Usuario>(
                "spUsuarioObterPorId",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Usuario> ObterPorEmailAsync(string email)
        {
            return await _connection.QueryFirstOrDefaultAsync<Usuario>(
                "spUsuarioObterPorEmail",
                new { Email = email },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Usuario> ObterPorIdComHistoricoAsync(int id)
        {
            using var multi = await _connection.QueryMultipleAsync(
                "spUsuarioObterComHistorico",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            var usuario = await multi.ReadFirstOrDefaultAsync<Usuario>();
            if (usuario == null)
                return null;

            usuario.HistoricoPesquisas = (await multi.ReadAsync<HistoricoPesquisa>()).ToList();
            return usuario;
        }

        public async Task<IEnumerable<Categoria>> ObterCategoriasPorUsuarioAsync(int id)
        {
            return await _connection.QueryAsync<Categoria>(
                @"SELECT c.Id, c.Nome, c.Descricao, c.Ativa
                  FROM UsuarioCategorias uc
                  INNER JOIN Categorias c ON c.Id = uc.CategoriaId
                  WHERE uc.UsuarioId = @Id AND c.Ativa = 1
                  ORDER BY c.Nome",
                new { Id = id }
            );
        }
    }
}