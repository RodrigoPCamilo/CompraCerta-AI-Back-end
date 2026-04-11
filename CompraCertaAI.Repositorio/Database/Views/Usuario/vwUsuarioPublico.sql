CREATE OR ALTER VIEW vwUsuarioPublico
AS
SELECT 
    u.Id,
    u.Nome,
    u.Email,
    u.DataCriacao,
    COUNT(uc.CategoriaId) AS TotalCategoriasFavoritas
FROM
    Usuarios u
LEFT JOIN UsuarioCategorias uc ON uc.UsuarioId = u.Id
GROUP BY
    u.Id,
    u.Nome,
    u.Email,
    u.DataCriacao;
    