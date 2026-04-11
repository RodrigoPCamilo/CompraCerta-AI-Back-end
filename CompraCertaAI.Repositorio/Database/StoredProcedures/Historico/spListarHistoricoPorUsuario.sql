CREATE OR ALTER PROCEDURE spListarHistoricoPorUsuario
(
    @UsuarioId INT
)
AS
BEGIN 
    SET NOCOUNT ON;

    SELECT 
        Id,
        UsuarioId,
        Query,
        SearchDate
    FROM 
        HistoricoPesquisas
    WHERE 
        UsuarioId = @UsuarioId
    ORDER BY 
        SearchDate DESC;
END;