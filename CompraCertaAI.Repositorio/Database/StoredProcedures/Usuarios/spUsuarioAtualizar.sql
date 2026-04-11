CREATE OR ALTER PROCEDURE spUsuarioAtualizar
(
    @Id INT,
    @Nome NVARCHAR(150),
    @Email NVARCHAR(150)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuarios
    SET
        Nome = @Nome,
        Email = @Email
    WHERE Id = @Id;
END;
