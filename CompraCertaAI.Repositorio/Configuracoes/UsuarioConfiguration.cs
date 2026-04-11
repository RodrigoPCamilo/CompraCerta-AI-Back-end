using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraCertaAI.Repositorio.Configuracoes
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios").HasKey(u => u.Id);
            builder.Property(u => u.Nome).HasColumnName("Nome").HasMaxLength(150).IsRequired();
            builder.Property(u => u.Email).HasColumnName("Email").HasMaxLength(150).IsRequired();
            builder.Property(u => u.SenhaHash).HasColumnName("SenhaHash").HasMaxLength(150).IsRequired();
            builder.Property(u => u.DataCriacao).HasColumnName("DataCriacao").IsRequired();

            builder.HasMany(u => u.CategoriasFavoritas)
                .WithMany(c => c.UsuariosFavoritos)
                .UsingEntity<UsuarioCategoria>(
                    j => j
                        .HasOne(uc => uc.Categoria)
                        .WithMany(c => c.UsuarioCategorias)
                        .HasForeignKey(uc => uc.CategoriaId),
                    j => j
                        .HasOne(uc => uc.Usuario)
                        .WithMany(u => u.UsuarioCategorias)
                        .HasForeignKey(uc => uc.UsuarioId),
                    j =>
                    {
                        j.ToTable("UsuarioCategorias");
                        j.HasKey(uc => new { uc.UsuarioId, uc.CategoriaId });
                    });

            builder.HasMany(u => u.HistoricoPesquisas)
                   .WithOne(h => h.Usuario)
                   .HasForeignKey(h => h.UsuarioId);

        }
    }
}