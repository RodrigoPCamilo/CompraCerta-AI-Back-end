CREATE OR ALTER PROCEDURE spUsuarioObterPorId
(
    @Id INT
)    
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        Id,
        Nome,
        Email,
        SenhaHash,
        DataCriacao 
    FROM Usuarios
    WHERE Id = @Id;
END;