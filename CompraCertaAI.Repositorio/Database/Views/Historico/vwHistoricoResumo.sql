CREATE OR ALTER VIEW vwHistoricoResumo
AS
SELECT 
    UsuarioId,
    Query,
    LEFT(Query, 200) AS QueryResumo,
    SearchDate
FROM 
    HistoricoPesquisas;