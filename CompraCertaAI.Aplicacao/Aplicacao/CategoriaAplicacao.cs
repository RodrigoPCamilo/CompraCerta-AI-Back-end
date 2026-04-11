using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Aplicacao.DTOs.Categoria;
using CompraCertaAI.Aplicacao.Interfaces;
using CompraCertaAI.Repositorio.Interfaces;

namespace CompraCertaAI.Aplicacao.Aplicacao
{
    public class CategoriaAplicacao : ICategoriaAplicacao
    {
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IUsuarioCategoriaRepositorio _usuarioCategoriaRepositorio;

        public CategoriaAplicacao(
            ICategoriaRepositorio categoriaRepositorio,
            IUsuarioCategoriaRepositorio usuarioCategoriaRepositorio)
        {
            _categoriaRepositorio = categoriaRepositorio;
            _usuarioCategoriaRepositorio = usuarioCategoriaRepositorio;
        }

        public async Task<IEnumerable<CategoriaDto>> ListarAtivasAsync()
        {
            var categorias = await _categoriaRepositorio.ListarAtivasAsync();
            return categorias.Select(Mapear);
        }

        public async Task<IEnumerable<CategoriaDto>> ObterCategoriasPorUsuarioAsync(int usuarioId)
        {
            var categorias = await _usuarioCategoriaRepositorio.ObterCategoriasPorUsuarioAsync(usuarioId);
            return categorias.Select(Mapear);
        }

        public async Task AtualizarCategoriasPorUsuarioAsync(int usuarioId, List<int> categoriaIds)
        {
            if (categoriaIds.Count > 5)
                throw new ArgumentException("Máximo de 5 categorias favoritas permitidas.");

            if (categoriaIds.Any())
            {
                var categorias = await _categoriaRepositorio.ObterPorIdsAsync(categoriaIds);
                var idsValidos = categorias.Select(c => c.Id).ToHashSet();
                var idInvalido = categoriaIds.FirstOrDefault(id => !idsValidos.Contains(id));
                if (idInvalido != 0)
                    throw new ArgumentException($"Categoria de id {idInvalido} não encontrada.");
            }

            await _usuarioCategoriaRepositorio.AtualizarCategoriasPorUsuarioAsync(usuarioId, categoriaIds);
        }

        private static CategoriaDto Mapear(CompraCertaAI.Dominio.Entidades.Categoria c) =>
            new CategoriaDto { Id = c.Id, Nome = c.Nome, Descricao = c.Descricao };
    }
}
