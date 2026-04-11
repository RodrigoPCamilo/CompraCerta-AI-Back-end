CREATE OR ALTER PROCEDURE spUsuarioCriar
(
    @Nome NVARCHAR(150),
    @Email NVARCHAR(150),
    @SenhaHash NVARCHAR(150)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Usuarios 
    (Nome, Email, SenhaHash, DataCriacao)
    VALUES 
    (@Nome, @Email, @SenhaHash, GETUTCDATE());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END;