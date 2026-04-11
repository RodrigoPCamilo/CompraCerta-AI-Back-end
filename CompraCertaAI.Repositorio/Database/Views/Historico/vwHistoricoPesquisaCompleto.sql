CREATE OR ALTER VIEW vwHistoricoPesquisaCompleto
AS
SELECT 
    h.Id,
    h.UsuarioId,
    u.Nome,
    u.Email,
    h.Query,
    h.SearchDate
FROM 
    HistoricoPesquisas h
INNER JOIN 
    Usuarios u ON u.Id = h.UsuarioId;