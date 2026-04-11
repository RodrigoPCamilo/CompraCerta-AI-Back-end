using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraCertaAI.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class refatoracao_usuario_historico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoriaFavorita",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "LojaPreferida",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "OrcamentoMedio",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RespostaGerada",
                table: "HistoricoPesquisas");

            migrationBuilder.RenameColumn(
                name: "Pergunta",
                table: "HistoricoPesquisas",
                newName: "Query");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "HistoricoPesquisas",
                newName: "SearchDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SearchDate",
                table: "HistoricoPesquisas",
                newName: "Data");

            migrationBuilder.RenameColumn(
                name: "Query",
                table: "HistoricoPesquisas",
                newName: "Pergunta");

            migrationBuilder.AddColumn<string>(
                name: "CategoriaFavorita",
                table: "Usuarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LojaPreferida",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OrcamentoMedio",
                table: "Usuarios",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RespostaGerada",
                table: "HistoricoPesquisas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
