using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompraCertaAI.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class interesses_e_produtos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Ativa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeProduto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImagemUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Loja = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LinkProduto = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioCategorias",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioCategorias", x => new { x.UsuarioId, x.CategoriaId });
                    table.ForeignKey(
                        name: "FK_UsuarioCategorias_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCategorias_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Ativa", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, true, "Eletrônicos, gadgets e acessórios tecnológicos", "Tecnologia" },
                    { 2, true, "Jogos, consoles e acessórios para gamers", "Games" },
                    { 3, true, "Decoração, móveis e utensílios domésticos", "Casa" },
                    { 4, true, "Livros físicos e digitais de todos os gêneros", "Livros" },
                    { 5, true, "Equipamentos e acessórios para saúde e atividade física", "Fitness" }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nome",
                table: "Categorias",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_NomeProduto",
                table: "Produtos",
                column: "NomeProduto");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCategorias_CategoriaId",
                table: "UsuarioCategorias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCategorias_UsuarioId",
                table: "UsuarioCategorias",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "UsuarioCategorias");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
