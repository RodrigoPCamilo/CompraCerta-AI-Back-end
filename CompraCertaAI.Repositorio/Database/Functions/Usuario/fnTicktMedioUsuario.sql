CREATE OR ALTER FUNCTION fnTicktMedioUsuario()
RETURNS DECIMAL(10,2)
AS
BEGIN 
    DECLARE @Media DECIMAL(10,2);

    ;WITH CategoriasPorUsuario AS
    (
        SELECT
            u.Id AS UsuarioId,
            COUNT(uc.CategoriaId) AS QuantidadeCategorias
        FROM Usuarios u
        LEFT JOIN UsuarioCategorias uc ON uc.UsuarioId = u.Id
        GROUP BY u.Id
    )
    SELECT
        @Media = ISNULL(AVG(CAST(QuantidadeCategorias AS DECIMAL(10,2))), 0)
    FROM CategoriasPorUsuario;

    RETURN @Media;
END;