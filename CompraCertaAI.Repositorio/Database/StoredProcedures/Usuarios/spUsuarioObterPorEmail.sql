CREATE OR ALTER PROCEDURE spUsuarioObterPorEmail
(
    @Email NVARCHAR(150)
)
AS
BEGIN
    SET
    NOCOUNT ON;

    SELECT
        Id,
        Nome,
        Email,
        SenhaHash,
        DataCriacao
     FROM Usuarios
    WHERE Email = @Email;
END;