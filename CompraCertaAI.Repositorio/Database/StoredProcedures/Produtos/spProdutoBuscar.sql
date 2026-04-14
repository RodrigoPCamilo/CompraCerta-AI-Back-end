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
