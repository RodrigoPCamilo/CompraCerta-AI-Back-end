using CompraCertaAI.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraCertaAI.Repositorio.Configuracoes
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("Categorias").HasKey(c => c.Id);
            builder.Property(c => c.Nome).HasColumnName("Nome").HasMaxLength(100).IsRequired();
            builder.Property(c => c.Descricao).HasColumnName("Descricao").HasMaxLength(500);
            builder.Property(c => c.Ativa).HasColumnName("Ativa").IsRequired().HasDefaultValue(true);

            builder.HasIndex(c => c.Nome).IsUnique();

            builder.HasData(
                new { Id = 1, Nome = "Tecnologia", Descricao = "Eletrônicos, gadgets e acessórios tecnológicos", Ativa = true },
                new { Id = 2, Nome = "Games", Descricao = "Jogos, consoles e acessórios para gamers", Ativa = true },
                new { Id = 3, Nome = "Casa", Descricao = "Decoração, móveis e utensílios domésticos", Ativa = true },
                new { Id = 4, Nome = "Livros", Descricao = "Livros físicos e digitais de todos os gêneros", Ativa = true },
                new { Id = 5, Nome = "Fitness", Descricao = "Equipamentos e acessórios para saúde e atividade física", Ativa = true }
            );
        }
    }
}
