using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraCertaAI.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class TornarPrecoOfertaObrigatorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE Produtos
SET PrecoOferta = 'R$ 0,00'
WHERE PrecoOferta IS NULL OR LTRIM(RTRIM(PrecoOferta)) = '';
");

            migrationBuilder.AlterColumn<string>(
                name: "PrecoOferta",
                table: "Produtos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "R$ 0,00",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PrecoOferta",
                table: "Produtos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "R$ 0,00");
        }
    }
}
