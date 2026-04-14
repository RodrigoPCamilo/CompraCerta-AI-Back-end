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
