CREATE OR ALTER PROCEDURE spCategoriaListarAtivas
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Nome, Descricao, Ativa
    FROM Categorias
    WHERE Ativa = 1
    ORDER BY Nome;
END;
