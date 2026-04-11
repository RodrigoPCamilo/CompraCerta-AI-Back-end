CREATE OR ALTER PROCEDURE spRegistraHistoricoPesquisa
(
    @UsuarioId INT,
    @Query NVARCHAR(500)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO HistoricoPesquisas (UsuarioId, Query, SearchDate)
    VALUES (@UsuarioId, @Query, GETUTCDATE());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END;
