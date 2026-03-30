using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CortexCommerce.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CategoriaFavorita = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OrcamentoMedio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LojaPreferida = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoPesquisas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Pergunta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RespostaGerada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoPesquisas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoPesquisas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoPesquisas_UsuarioId",
                table: "HistoricoPesquisas",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoPesquisas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
