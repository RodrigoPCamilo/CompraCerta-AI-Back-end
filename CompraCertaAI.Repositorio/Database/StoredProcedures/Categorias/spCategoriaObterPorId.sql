CREATE OR ALTER PROCEDURE spCategoriaObterPorId
(
    @Id INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Nome, Descricao, Ativa
    FROM Categorias
    WHERE Id = @Id;
END;
