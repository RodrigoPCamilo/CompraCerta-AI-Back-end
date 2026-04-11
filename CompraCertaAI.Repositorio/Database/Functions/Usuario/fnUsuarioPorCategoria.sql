CREATE OR ALTER FUNCTION fnUsuarioPorCategoria
(
    @Categoria NVARCHAR(100)
)
RETURNS INT
AS
BEGIN
    DECLARE @Total INT;

    SELECT
        @Total = COUNT(DISTINCT uc.UsuarioId)
    FROM UsuarioCategorias uc
    INNER JOIN Categorias c ON c.Id = uc.CategoriaId
    WHERE c.Nome = @Categoria
      AND c.Ativa = 1;

    RETURN @Total;
END;