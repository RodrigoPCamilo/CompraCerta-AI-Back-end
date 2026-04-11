CREATE OR ALTER PROCEDURE spUsuarioCategoriaAtualizar
(
    @UsuarioId INT,
    @CategoriaIds NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        DELETE FROM UsuarioCategorias
        WHERE UsuarioId = @UsuarioId;

        IF (@CategoriaIds IS NOT NULL AND LTRIM(RTRIM(@CategoriaIds)) <> '')
        BEGIN
            INSERT INTO UsuarioCategorias (UsuarioId, CategoriaId)
            SELECT DISTINCT @UsuarioId, TRY_CAST(value AS INT)
            FROM STRING_SPLIT(@CategoriaIds, ',')
            WHERE TRY_CAST(value AS INT) IS NOT NULL;
        END;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
