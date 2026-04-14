using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraCertaAI.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarPrecoOfertaProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrecoOferta",
                table: "Produtos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoOferta",
                table: "Produtos");
        }
    }
}
