using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;
using CompraCertaAI.Repositorio.Configuracoes;
using Microsoft.EntityFrameworkCore;

namespace CompraCertaAI.Repositorio.Contexto
{
    public class CompraCertaAIContexto : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<HistoricoPesquisa> HistoricoPesquisas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<UsuarioCategoria> UsuarioCategorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        public CompraCertaAIContexto(DbContextOptions<CompraCertaAIContexto> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new HistoricoPesquisaConfiguration());
            modelBuilder.ApplyConfiguration(new CategoriaConfiguration());
            modelBuilder.ApplyConfiguration(new UsuarioCategoriaConfiguration());
            modelBuilder.ApplyConfiguration(new ProdutoConfiguration());
        }
    }
}                                                                                       