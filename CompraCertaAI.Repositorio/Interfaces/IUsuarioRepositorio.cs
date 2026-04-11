using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;


namespace CompraCertaAI.Repositorio.Interfaces
{
    public interface IUsuarioRepositorio
    {

        Task CriarAsync(Usuario usuario);
        Task AtualizarAsync(Usuario usuario);
        Task<Usuario> ObterPorIdAsync(int id);
        Task<Usuario> ObterPorEmailAsync(string email);
        Task<Usuario> ObterPorIdComHistoricoAsync(int id);
        Task<IEnumerable<Categoria>> ObterCategoriasPorUsuarioAsync(int id);
    }
}