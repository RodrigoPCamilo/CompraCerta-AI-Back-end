CREATE OR ALTER FUNCTION fnTotalPerguntaUsuario
(
    @UsuarioId INT
)
RETURNS INT
AS
BEGIN
    DECLARE @Total INT;

    SELECT 
        @Total = COUNT(*)
    FROM 
        HistoricoPesquisas
    WHERE 
        UsuarioId = @UsuarioId;

    RETURN @Total;
END;