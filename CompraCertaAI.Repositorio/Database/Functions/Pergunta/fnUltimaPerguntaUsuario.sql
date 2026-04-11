CREATE OR ALTER FUNCTION fnUltimaPerguntaUsuario
(
    @UsuarioId INT
)
RETURNS NVARCHAR(500)
AS
BEGIN
    DECLARE @Pergunta NVARCHAR(500);

    SELECT 
        TOP 1 @Pergunta = Query
    FROM 
        HistoricoPesquisas
    WHERE 
        UsuarioId = @UsuarioId
    ORDER BY 
        SearchDate DESC;

    RETURN @Pergunta;
END;