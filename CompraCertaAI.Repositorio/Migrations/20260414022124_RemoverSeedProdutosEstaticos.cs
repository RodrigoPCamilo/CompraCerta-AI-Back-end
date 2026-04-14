using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompraCertaAI.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class RemoverSeedProdutosEstaticos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Ativo", "CategoriaId", "Descricao", "ImagemUrl", "LinkProduto", "Loja", "NomeProduto" },
                values: new object[,]
                {
                    { 1, true, 1, "Televisão inteligente com resolução 4K, HDR e acesso a streaming", "", "https://www.amazon.com.br/s?k=smart+tv+samsung+55+4k", "Amazon", "Smart TV Samsung 55\" 4K UHD" },
                    { 2, true, 1, "Notebook com processador Intel Core i5, 8GB RAM e 256GB SSD", "", "https://lista.mercadolivre.com.br/notebook-dell-inspiron-15", "Mercado Livre", "Notebook Dell Inspiron 15" },
                    { 3, true, 2, "Console de última geração com SSD ultrarrápido e ray tracing", "", "https://www.magazineluiza.com.br/busca/playstation+5+slim/", "Magazine Luiza", "PlayStation 5 Slim" },
                    { 4, true, 2, "Controle sem fio com botão compartilhar e textura antiderrapante", "", "https://www.amazon.com.br/s?k=controle+xbox+series", "Amazon", "Controle Xbox Series S/X" },
                    { 5, true, 3, "Conjunto com lençol, fronha e capa de almofada em percal egípcio", "", "https://shopee.com.br/search?keyword=jogo+de+lencol+queen+percal+400", "Shopee", "Jogo de Lençol Queen Percal 400 Fios" },
                    { 6, true, 3, "Liquidificador com 900W, 5 velocidades e jarra de vidro de 2L", "", "https://www.magazineluiza.com.br/busca/liquidificador+britania+bl900/", "Magazine Luiza", "Liquidificador Britânia BL900" },
                    { 7, true, 4, "Guia prático para escrever código limpo e de qualidade", "", "https://www.amazon.com.br/s?k=clean+code+robert+martin", "Amazon", "Clean Code - Robert C. Martin" },
                    { 8, true, 4, "Clássico da literatura brasileira do período realista", "", "https://lista.mercadolivre.com.br/dom-casmurro-machado-de-assis", "Mercado Livre", "Dom Casmurro - Machado de Assis" },
                    { 9, true, 5, "Tênis esportivo com almofada de ar e palmilha macia para conforto", "", "https://www.amazon.com.br/s?k=tenis+nike+air+max+270", "Amazon", "Tênis Nike Air Max 270" },
                    { 10, true, 5, "Par de halteres com revestimento emborrachado, ideal para musculação em casa", "", "https://shopee.com.br/search?keyword=kit+halteres+emborrachados+10kg", "Shopee", "Kit Halteres Emborrachados 10kg" }
                });
        }
    }
}
