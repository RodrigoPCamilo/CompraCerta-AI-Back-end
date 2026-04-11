using CompraCertaAI.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraCertaAI.Repositorio.Configuracoes
{
    public class UsuarioCategoriaConfiguration : IEntityTypeConfiguration<UsuarioCategoria>
    {
        public void Configure(EntityTypeBuilder<UsuarioCategoria> builder)
        {
            builder.ToTable("UsuarioCategorias")
                   .HasKey(uc => new { uc.UsuarioId, uc.CategoriaId });

            builder.HasIndex(uc => uc.UsuarioId);
        }
    }
}
