CREATE OR ALTER PROCEDURE spCategoriaObterPorIds
(
    @Ids NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.Id, c.Nome, c.Descricao, c.Ativa
    FROM Categorias c
    INNER JOIN STRING_SPLIT(@Ids, ',') s ON c.Id = TRY_CAST(s.value AS INT)
    ORDER BY c.Nome;
END;
