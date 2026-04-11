CREATE OR ALTER PROCEDURE spUsuarioObterComHistorico
    @Id INT
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
  
    SELECT
        Id,
        UsuarioId,
        Query,
        SearchDate
    FROM HistoricoPesquisas
    WHERE UsuarioId = @Id
    ORDER BY SearchDate DESC;
END