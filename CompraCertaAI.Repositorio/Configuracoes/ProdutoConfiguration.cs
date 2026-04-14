using CompraCertaAI.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraCertaAI.Repositorio.Configuracoes
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos").HasKey(p => p.Id);
            builder.Property(p => p.NomeProduto).HasColumnName("NomeProduto").HasMaxLength(300).IsRequired();
            builder.Property(p => p.PrecoOferta).HasColumnName("PrecoOferta").HasMaxLength(50);
            builder.Property(p => p.Descricao).HasColumnName("Descricao").HasMaxLength(1000);
            builder.Property(p => p.ImagemUrl).HasColumnName("ImagemUrl").HasMaxLength(500);
            builder.Property(p => p.Loja).HasColumnName("Loja").HasMaxLength(100).IsRequired();
            builder.Property(p => p.LinkProduto).HasColumnName("LinkProduto").HasMaxLength(1000).IsRequired();
            builder.Property(p => p.Ativo).HasColumnName("Ativo").IsRequired().HasDefaultValue(true);

            builder.HasOne(p => p.Categoria)
                   .WithMany(c => c.Produtos)
                   .HasForeignKey(p => p.CategoriaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.CategoriaId);
            builder.HasIndex(p => p.NomeProduto);
        }
    }
}
