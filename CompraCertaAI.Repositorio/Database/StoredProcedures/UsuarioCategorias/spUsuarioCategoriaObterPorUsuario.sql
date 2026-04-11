CREATE OR ALTER PROCEDURE spUsuarioCategoriaObterPorUsuario
(
    @UsuarioId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.Id, c.Nome, c.Descricao, c.Ativa
    FROM UsuarioCategorias uc
    INNER JOIN Categorias c ON c.Id = uc.CategoriaId
    WHERE uc.UsuarioId = @UsuarioId
      AND c.Ativa = 1
    ORDER BY c.Nome;
END;
