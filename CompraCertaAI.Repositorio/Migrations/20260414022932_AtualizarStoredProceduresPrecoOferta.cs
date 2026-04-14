using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraCertaAI.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarStoredProceduresPrecoOferta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE spProdutoBuscar
(
    @Query NVARCHAR(300) = NULL,
    @CategoriaId INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Id,
        p.NomeProduto,
        p.PrecoOferta,
        p.Descricao,
        p.ImagemUrl,
        p.Loja,
        p.LinkProduto,
        p.CategoriaId,
        p.Ativo
    FROM Produtos p
    INNER JOIN Categorias c ON c.Id = p.CategoriaId
    WHERE p.Ativo = 1
      AND (@Query IS NULL OR p.NomeProduto LIKE '%' + @Query + '%' OR c.Nome LIKE '%' + @Query + '%')
      AND (@CategoriaId IS NULL OR p.CategoriaId = @CategoriaId)
    ORDER BY p.NomeProduto;
END;
");

            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE spProdutoObterRecomendacoes
(
    @CategoriaIds NVARCHAR(MAX),
    @Limite INT = 10
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Limite)
        p.Id,
        p.NomeProduto,
        p.PrecoOferta,
        p.Descricao,
        p.ImagemUrl,
        p.Loja,
        p.LinkProduto,
        p.CategoriaId,
        p.Ativo
    FROM Produtos p
    INNER JOIN STRING_SPLIT(@CategoriaIds, ',') s ON p.CategoriaId = TRY_CAST(s.value AS INT)
    WHERE p.Ativo = 1
    ORDER BY NEWID();
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE spProdutoBuscar
(
    @Query NVARCHAR(300) = NULL,
    @CategoriaId INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Id,
        p.NomeProduto,
        p.Descricao,
        p.ImagemUrl,
        p.Loja,
        p.LinkProduto,
        p.CategoriaId,
        p.Ativo
    FROM Produtos p
    INNER JOIN Categorias c ON c.Id = p.CategoriaId
    WHERE p.Ativo = 1
      AND (@Query IS NULL OR p.NomeProduto LIKE '%' + @Query + '%' OR c.Nome LIKE '%' + @Query + '%')
      AND (@CategoriaId IS NULL OR p.CategoriaId = @CategoriaId)
    ORDER BY p.NomeProduto;
END;
");

            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE spProdutoObterRecomendacoes
(
    @CategoriaIds NVARCHAR(MAX),
    @Limite INT = 10
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Limite)
        p.Id,
        p.NomeProduto,
        p.Descricao,
        p.ImagemUrl,
        p.Loja,
        p.LinkProduto,
        p.CategoriaId,
        p.Ativo
    FROM Produtos p
    INNER JOIN STRING_SPLIT(@CategoriaIds, ',') s ON p.CategoriaId = TRY_CAST(s.value AS INT)
    WHERE p.Ativo = 1
    ORDER BY NEWID();
END;
");
        }
    }
}
