CREATE OR ALTER VIEW vwUsuarioAdmin
AS
SELECT 
    u.Id,
    u.Nome,
    u.Email,
    u.DataCriacao,
    COUNT(DISTINCT uc.CategoriaId) AS TotalCategoriasFavoritas,
    COUNT(h.Id) AS TotalPesquisas
FROM Usuarios u
LEFT JOIN UsuarioCategorias uc ON uc.UsuarioId = u.Id
LEFT JOIN HistoricoPesquisas h ON h.UsuarioId = u.Id
GROUP BY
    u.Id,
    u.Nome,
    u.Email,
    u.DataCriacao;