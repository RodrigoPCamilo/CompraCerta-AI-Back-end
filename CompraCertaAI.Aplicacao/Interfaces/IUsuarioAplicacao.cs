using System.Collections.Generic;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Usuario;
using CompraCertaAI.Dominio.Entidades;

namespace CompraCertaAI.Aplicacao.Interfaces
{
    public interface IUsuarioAplicacao
    {
        Task<UsuarioDto> CriarAsync(CriarUsuarioDto dto);
        Task<UsuarioDto> AtualizarAsync(int id, AtualizarUsuarioDto dto);
        Task<UsuarioDto> ObterPorIdAsync(int id);
        Task<UsuarioDto> ObterPerfilAsync(int id);
        Task AtualizarCategoriasAsync(int usuarioId, List<int> categoriaIds);
    }
}