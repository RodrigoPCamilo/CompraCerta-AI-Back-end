using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Categoria;
using CompraCertaAI.Aplicacao.DTOs.Usuario;
using CompraCertaAI.Aplicacao.Interfaces;
using CompraCertaAI.Dominio.Entidades;
using CompraCertaAI.Repositorio;
using CompraCertaAI.Repositorio.Interfaces;


namespace CompraCertaAI.Aplicacao.Aplicacao
{
    public class UsuarioAplicacao : IUsuarioAplicacao
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IUsuarioCategoriaRepositorio _usuarioCategoriaRepositorio;
        private readonly ICategoriaRepositorio _categoriaRepositorio;

        public UsuarioAplicacao(
            IUsuarioRepositorio repositorio,
            IUsuarioCategoriaRepositorio usuarioCategoriaRepositorio,
            ICategoriaRepositorio categoriaRepositorio)
        {
            _usuarioRepositorio = repositorio;
            _usuarioCategoriaRepositorio = usuarioCategoriaRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
        }

        public async Task<UsuarioDto> CriarAsync(CriarUsuarioDto dto)
        {
            var usuario = new Usuario(
                dto.Nome,
                dto.Email,
                dto.Senha
            );

            await _usuarioRepositorio.CriarAsync(usuario);

            if (dto.CategoriaIds != null)
                await AtualizarCategoriasAsync(usuario.Id, dto.CategoriaIds);

            return await ObterPerfilAsync(usuario.Id);
        }

        public async Task<UsuarioDto> AtualizarAsync(int id, AtualizarUsuarioDto dto)
        {
            var usuario = await _usuarioRepositorio.ObterPorIdAsync(id);

            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            usuario.AtualizarDados(dto.Nome, dto.Email);

            await _usuarioRepositorio.AtualizarAsync(usuario);

            if (dto.CategoriaIds != null)
                await AtualizarCategoriasAsync(id, dto.CategoriaIds);

            return await ObterPerfilAsync(id);
        }

        public async Task<UsuarioDto> ObterPorIdAsync(int id)
        {
            var usuario = await _usuarioRepositorio.ObterPorIdAsync(id);

            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            return MapearBase(usuario);
        }

        public async Task<UsuarioDto> ObterPerfilAsync(int id)
        {
            var usuario = await _usuarioRepositorio.ObterPorIdAsync(id);
            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            var categorias = await _usuarioRepositorio.ObterCategoriasPorUsuarioAsync(id);

            var dto = MapearBase(usuario);
            dto.Categorias = categorias.Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao
            }).ToList();

            return dto;
        }

        public async Task AtualizarCategoriasAsync(int usuarioId, List<int> categoriaIds)
        {
            if (categoriaIds == null || categoriaIds.Count > 5)
                throw new ArgumentException("Máximo de 5 categorias favoritas permitidas.");

            var categoriaIdsNormalizados = categoriaIds.Distinct().ToList();
            if (categoriaIdsNormalizados.Count > 5)
                throw new ArgumentException("Máximo de 5 categorias favoritas permitidas.");

            var usuario = await _usuarioRepositorio.ObterPorIdAsync(usuarioId);
            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            var categoriasAtivas = (await _categoriaRepositorio.ListarAtivasAsync()).ToList();
            var idsCategoriasAtivas = categoriasAtivas.Select(c => c.Id).ToHashSet();
            var idsInvalidos = categoriaIdsNormalizados
                .Where(id => !idsCategoriasAtivas.Contains(id))
                .ToList();

            if (idsInvalidos.Any())
                throw new ArgumentException($"Categoria(s) inválida(s): {string.Join(", ", idsInvalidos)}.");

            var categorias = categoriasAtivas
                .Where(c => categoriaIdsNormalizados.Contains(c.Id))
                .ToList();

            usuario.DefinirCategoriasFavoritas(categorias);
            await _usuarioCategoriaRepositorio.AtualizarCategoriasPorUsuarioAsync(usuarioId, categoriaIdsNormalizados);
        }

        private static UsuarioDto MapearBase(Usuario usuario)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            };
        }
    }
}