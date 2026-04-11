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

            builder.HasData(
                new { Id = 1, NomeProduto = "Smart TV Samsung 55\" 4K UHD", Descricao = "Televisão inteligente com resolução 4K, HDR e acesso a streaming", ImagemUrl = "", Loja = "Amazon", LinkProduto = "https://www.amazon.com.br/s?k=smart+tv+samsung+55+4k", CategoriaId = 1, Ativo = true },
                new { Id = 2, NomeProduto = "Notebook Dell Inspiron 15", Descricao = "Notebook com processador Intel Core i5, 8GB RAM e 256GB SSD", ImagemUrl = "", Loja = "Mercado Livre", LinkProduto = "https://lista.mercadolivre.com.br/notebook-dell-inspiron-15", CategoriaId = 1, Ativo = true },
                new { Id = 3, NomeProduto = "PlayStation 5 Slim", Descricao = "Console de última geração com SSD ultrarrápido e ray tracing", ImagemUrl = "", Loja = "Magazine Luiza", LinkProduto = "https://www.magazineluiza.com.br/busca/playstation+5+slim/", CategoriaId = 2, Ativo = true },
                new { Id = 4, NomeProduto = "Controle Xbox Series S/X", Descricao = "Controle sem fio com botão compartilhar e textura antiderrapante", ImagemUrl = "", Loja = "Amazon", LinkProduto = "https://www.amazon.com.br/s?k=controle+xbox+series", CategoriaId = 2, Ativo = true },
                new { Id = 5, NomeProduto = "Jogo de Lençol Queen Percal 400 Fios", Descricao = "Conjunto com lençol, fronha e capa de almofada em percal egípcio", ImagemUrl = "", Loja = "Shopee", LinkProduto = "https://shopee.com.br/search?keyword=jogo+de+lencol+queen+percal+400", CategoriaId = 3, Ativo = true },
                new { Id = 6, NomeProduto = "Liquidificador Britânia BL900", Descricao = "Liquidificador com 900W, 5 velocidades e jarra de vidro de 2L", ImagemUrl = "", Loja = "Magazine Luiza", LinkProduto = "https://www.magazineluiza.com.br/busca/liquidificador+britania+bl900/", CategoriaId = 3, Ativo = true },
                new { Id = 7, NomeProduto = "Clean Code - Robert C. Martin", Descricao = "Guia prático para escrever código limpo e de qualidade", ImagemUrl = "", Loja = "Amazon", LinkProduto = "https://www.amazon.com.br/s?k=clean+code+robert+martin", CategoriaId = 4, Ativo = true },
                new { Id = 8, NomeProduto = "Dom Casmurro - Machado de Assis", Descricao = "Clássico da literatura brasileira do período realista", ImagemUrl = "", Loja = "Mercado Livre", LinkProduto = "https://lista.mercadolivre.com.br/dom-casmurro-machado-de-assis", CategoriaId = 4, Ativo = true },
                new { Id = 9, NomeProduto = "Tênis Nike Air Max 270", Descricao = "Tênis esportivo com almofada de ar e palmilha macia para conforto", ImagemUrl = "", Loja = "Amazon", LinkProduto = "https://www.amazon.com.br/s?k=tenis+nike+air+max+270", CategoriaId = 5, Ativo = true },
                new { Id = 10, NomeProduto = "Kit Halteres Emborrachados 10kg", Descricao = "Par de halteres com revestimento emborrachado, ideal para musculação em casa", ImagemUrl = "", Loja = "Shopee", LinkProduto = "https://shopee.com.br/search?keyword=kit+halteres+emborrachados+10kg", CategoriaId = 5, Ativo = true }
            );
        }
    }
}
