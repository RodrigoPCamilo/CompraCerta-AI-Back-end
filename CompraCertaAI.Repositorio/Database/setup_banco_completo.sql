-- ============================================================
-- CompraCertaAI – Script completo de setup do banco de dados
-- Execute este script conectado ao SQL Server como administrador
-- ============================================================

-- ============================================================
-- 1. CRIAR BANCO DE DADOS
-- ============================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CompraCertaAIDb')
BEGIN
    CREATE DATABASE CompraCertaAIDb;
    PRINT 'Banco CompraCertaAIDb criado com sucesso.';
END
ELSE
    PRINT 'Banco CompraCertaAIDb ja existe. Continuando...';
GO

USE CompraCertaAIDb;
GO

-- ============================================================
-- 2. TABELAS
-- ============================================================

-- Tabela: Usuarios
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios (
        Id          INT            IDENTITY(1,1) NOT NULL,
        Nome        NVARCHAR(150)  NOT NULL,
        Email       NVARCHAR(150)  NOT NULL,
        SenhaHash   NVARCHAR(150)  NOT NULL,
        DataCriacao DATETIME2      NOT NULL,
        CONSTRAINT PK_Usuarios PRIMARY KEY (Id)
    );
    PRINT 'Tabela Usuarios criada.';
END
ELSE
    PRINT 'Tabela Usuarios ja existe.';
GO

-- Tabela: Categorias
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categorias')
BEGIN
    CREATE TABLE Categorias (
        Id       INT            IDENTITY(1,1) NOT NULL,
        Nome     NVARCHAR(100)  NOT NULL,
        Descricao NVARCHAR(500) NULL,
        Ativa    BIT            NOT NULL CONSTRAINT DF_Categorias_Ativa DEFAULT (1),
        CONSTRAINT PK_Categorias PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_Categorias_Nome ON Categorias (Nome);
    PRINT 'Tabela Categorias criada.';
END
ELSE
    PRINT 'Tabela Categorias ja existe.';
GO

-- Tabela: HistoricoPesquisas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HistoricoPesquisas')
BEGIN
    CREATE TABLE HistoricoPesquisas (
        Id         INT            IDENTITY(1,1) NOT NULL,
        UsuarioId  INT            NOT NULL,
        Query      NVARCHAR(500)  NOT NULL,
        SearchDate DATETIME2      NOT NULL,
        CONSTRAINT PK_HistoricoPesquisas PRIMARY KEY (Id),
        CONSTRAINT FK_HistoricoPesquisas_Usuarios_UsuarioId
            FOREIGN KEY (UsuarioId) REFERENCES Usuarios (Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_HistoricoPesquisas_UsuarioId ON HistoricoPesquisas (UsuarioId);
    PRINT 'Tabela HistoricoPesquisas criada.';
END
ELSE
    PRINT 'Tabela HistoricoPesquisas ja existe.';
GO

-- Tabela: Produtos
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Produtos')
BEGIN
    CREATE TABLE Produtos (
        Id           INT             IDENTITY(1,1) NOT NULL,
        NomeProduto  NVARCHAR(300)   NOT NULL,
        Descricao    NVARCHAR(1000)  NULL,
        ImagemUrl    NVARCHAR(500)   NULL,
        Loja         NVARCHAR(100)   NOT NULL,
        LinkProduto  NVARCHAR(1000)  NOT NULL,
        CategoriaId  INT             NOT NULL,
        Ativo        BIT             NOT NULL CONSTRAINT DF_Produtos_Ativo DEFAULT (1),
        CONSTRAINT PK_Produtos PRIMARY KEY (Id),
        CONSTRAINT FK_Produtos_Categorias_CategoriaId
            FOREIGN KEY (CategoriaId) REFERENCES Categorias (Id) ON DELETE NO ACTION
    );
    CREATE INDEX IX_Produtos_CategoriaId ON Produtos (CategoriaId);
    CREATE INDEX IX_Produtos_NomeProduto  ON Produtos (NomeProduto);
    PRINT 'Tabela Produtos criada.';
END
ELSE
    PRINT 'Tabela Produtos ja existe.';
GO

-- Tabela: UsuarioCategorias
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UsuarioCategorias')
BEGIN
    CREATE TABLE UsuarioCategorias (
        UsuarioId   INT NOT NULL,
        CategoriaId INT NOT NULL,
        CONSTRAINT PK_UsuarioCategorias PRIMARY KEY (UsuarioId, CategoriaId),
        CONSTRAINT FK_UsuarioCategorias_Usuarios_UsuarioId
            FOREIGN KEY (UsuarioId) REFERENCES Usuarios (Id) ON DELETE CASCADE,
        CONSTRAINT FK_UsuarioCategorias_Categorias_CategoriaId
            FOREIGN KEY (CategoriaId) REFERENCES Categorias (Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_UsuarioCategorias_UsuarioId   ON UsuarioCategorias (UsuarioId);
    CREATE INDEX IX_UsuarioCategorias_CategoriaId ON UsuarioCategorias (CategoriaId);
    PRINT 'Tabela UsuarioCategorias criada.';
END
ELSE
    PRINT 'Tabela UsuarioCategorias ja existe.';
GO

-- Tabela de controle de migrations do EF Core
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL,
        ProductVersion NVARCHAR(32)  NOT NULL,
        CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );
    PRINT 'Tabela __EFMigrationsHistory criada.';
END
GO

-- Registrar migrations para que o EF Core não tente recriar tabelas
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260329231626_inicial')
    INSERT INTO __EFMigrationsHistory VALUES ('20260329231626_inicial', '8.0.0');
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260409030609_interesses_e_produtos')
    INSERT INTO __EFMigrationsHistory VALUES ('20260409030609_interesses_e_produtos', '8.0.0');
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260410005657_refatoracao_usuario_historico')
    INSERT INTO __EFMigrationsHistory VALUES ('20260410005657_refatoracao_usuario_historico', '8.0.0');
GO

-- ============================================================
-- 3. SEED – CATEGORIAS (5 categorias base)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Categorias)
BEGIN
    SET IDENTITY_INSERT Categorias ON;
    INSERT INTO Categorias (Id, Nome, Descricao, Ativa) VALUES
        (1, 'Tecnologia', 'Eletronicos, gadgets e acessorios tecnologicos',      1),
        (2, 'Games',      'Jogos, consoles e acessorios para gamers',              1),
        (3, 'Casa',       'Decoracao, moveis e utensilios domesticos',             1),
        (4, 'Livros',     'Livros fisicos e digitais de todos os generos',         1),
        (5, 'Fitness',    'Equipamentos e acessorios para saude e atividade fisica', 1);
    SET IDENTITY_INSERT Categorias OFF;
    PRINT 'Categorias inseridas.';
END
ELSE
    PRINT 'Categorias ja existem. Seed ignorado.';
GO

-- ============================================================
-- 4. SEED – PRODUTOS (2 por categoria = 10 no total)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Produtos)
BEGIN
    SET IDENTITY_INSERT Produtos ON;
    INSERT INTO Produtos (Id, NomeProduto, Descricao, ImagemUrl, Loja, LinkProduto, CategoriaId, Ativo) VALUES
        (1,  'Smart TV Samsung 55" 4K UHD',       'Televisao inteligente com resolucao 4K, HDR e acesso a streaming',                  '', 'Amazon',          'https://www.amazon.com.br/s?k=smart+tv+samsung+55+4k',              1, 1),
        (2,  'Notebook Dell Inspiron 15',          'Notebook com processador Intel Core i5, 8GB RAM e 256GB SSD',                      '', 'Mercado Livre',   'https://lista.mercadolivre.com.br/notebook-dell-inspiron-15',      1, 1),
        (3,  'PlayStation 5 Slim',                 'Console de ultima geracao com SSD ultrarrapido e ray tracing',                     '', 'Magazine Luiza',  'https://www.magazineluiza.com.br/busca/playstation+5+slim/',        2, 1),
        (4,  'Controle Xbox Series S/X',           'Controle sem fio com botao compartilhar e textura antiderrapante',                 '', 'Amazon',          'https://www.amazon.com.br/s?k=controle+xbox+series',               2, 1),
        (5,  'Jogo de Lencol Queen Percal 400 Fios','Conjunto com lencol, fronha e capa de almofada em percal egípcio',               '', 'Shopee',          'https://shopee.com.br/search?keyword=jogo+de+lencol+queen+percal+400', 3, 1),
        (6,  'Liquidificador Britania BL900',       'Liquidificador com 900W, 5 velocidades e jarra de vidro de 2L',                   '', 'Magazine Luiza',  'https://www.magazineluiza.com.br/busca/liquidificador+britania+bl900/', 3, 1),
        (7,  'Clean Code - Robert C. Martin',       'Guia pratico para escrever codigo limpo e de qualidade',                          '', 'Amazon',          'https://www.amazon.com.br/s?k=clean+code+robert+martin',           4, 1),
        (8,  'Dom Casmurro - Machado de Assis',    'Classico da literatura brasileira do periodo realista',                            '', 'Mercado Livre',   'https://lista.mercadolivre.com.br/dom-casmurro-machado-de-assis',  4, 1),
        (9,  'Tenis Nike Air Max 270',             'Tenis esportivo com almofada de ar e palmilha macia para conforto',               '', 'Amazon',          'https://www.amazon.com.br/s?k=tenis+nike+air+max+270',             5, 1),
        (10, 'Kit Halteres Emborrachados 10kg',    'Par de halteres com revestimento emborrachado, ideal para musculacao em casa',    '', 'Shopee',          'https://shopee.com.br/search?keyword=kit+halteres+emborrachados+10kg', 5, 1);
    SET IDENTITY_INSERT Produtos OFF;
    PRINT 'Produtos inseridos.';
END
ELSE
    PRINT 'Produtos ja existem. Seed ignorado.';
GO

-- ============================================================
-- 5. STORED PROCEDURES – USUARIOS
-- ============================================================

CREATE OR ALTER PROCEDURE spUsuarioCriar
(
    @Nome      NVARCHAR(150),
    @Email     NVARCHAR(150),
    @SenhaHash NVARCHAR(150)
)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Usuarios (Nome, Email, SenhaHash, DataCriacao)
    VALUES (@Nome, @Email, @SenhaHash, GETUTCDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END;
GO

CREATE OR ALTER PROCEDURE spUsuarioObterPorId
(
    @Id INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nome, Email, SenhaHash, DataCriacao
    FROM Usuarios
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE spUsuarioObterPorEmail
(
    @Email NVARCHAR(150)
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nome, Email, SenhaHash, DataCriacao
    FROM Usuarios
    WHERE Email = @Email;
END;
GO

CREATE OR ALTER PROCEDURE spUsuarioAtualizar
(
    @Id    INT,
    @Nome  NVARCHAR(150),
    @Email NVARCHAR(150)
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Usuarios
    SET Nome  = @Nome,
        Email = @Email
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE spUsuarioObterComHistorico
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nome, Email, SenhaHash, DataCriacao
    FROM Usuarios
    WHERE Id = @Id;

    SELECT Id, UsuarioId, Query, SearchDate
    FROM HistoricoPesquisas
    WHERE UsuarioId = @Id
    ORDER BY SearchDate DESC;
END;
GO

-- ============================================================
-- 6. STORED PROCEDURES – HISTORICO
-- ============================================================

CREATE OR ALTER PROCEDURE spRegistraHistoricoPesquisa
(
    @UsuarioId INT,
    @Query     NVARCHAR(500)
)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO HistoricoPesquisas (UsuarioId, Query, SearchDate)
    VALUES (@UsuarioId, @Query, GETUTCDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END;
GO

CREATE OR ALTER PROCEDURE spListarHistoricoPorUsuario
(
    @UsuarioId INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UsuarioId, Query, SearchDate
    FROM HistoricoPesquisas
    WHERE UsuarioId = @UsuarioId
    ORDER BY SearchDate DESC;
END;
GO

-- ============================================================
-- 7. STORED PROCEDURES – CATEGORIAS
-- ============================================================

CREATE OR ALTER PROCEDURE spCategoriaListarAtivas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nome, Descricao, Ativa
    FROM Categorias
    WHERE Ativa = 1
    ORDER BY Nome;
END;
GO

CREATE OR ALTER PROCEDURE spCategoriaObterPorId
(
    @Id INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nome, Descricao, Ativa
    FROM Categorias
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE spCategoriaObterPorIds
(
    @Ids NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.Id, c.Nome, c.Descricao, c.Ativa
    FROM Categorias c
    INNER JOIN STRING_SPLIT(@Ids, ',') s ON c.Id = TRY_CAST(s.value AS INT)
    ORDER BY c.Nome;
END;
GO

-- ============================================================
-- 8. STORED PROCEDURES – USUARIO CATEGORIAS
-- ============================================================

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
GO

CREATE OR ALTER PROCEDURE spUsuarioCategoriaAtualizar
(
    @UsuarioId   INT,
    @CategoriaIds NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DELETE FROM UsuarioCategorias WHERE UsuarioId = @UsuarioId;

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
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- ============================================================
-- 9. STORED PROCEDURES – PRODUTOS
-- ============================================================

CREATE OR ALTER PROCEDURE spProdutoBuscar
(
    @Query      NVARCHAR(300) = NULL,
    @CategoriaId INT          = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.Id, p.NomeProduto, p.Descricao, p.ImagemUrl,
           p.Loja, p.LinkProduto, p.CategoriaId, p.Ativo
    FROM Produtos p
    INNER JOIN Categorias c ON c.Id = p.CategoriaId
    WHERE p.Ativo = 1
      AND (@Query      IS NULL OR p.NomeProduto LIKE '%' + @Query + '%'
                                OR c.Nome       LIKE '%' + @Query + '%')
      AND (@CategoriaId IS NULL OR p.CategoriaId = @CategoriaId)
    ORDER BY p.NomeProduto;
END;
GO

CREATE OR ALTER PROCEDURE spProdutoObterRecomendacoes
(
    @CategoriaIds NVARCHAR(MAX),
    @Limite       INT = 10
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (@Limite)
        p.Id, p.NomeProduto, p.Descricao, p.ImagemUrl,
        p.Loja, p.LinkProduto, p.CategoriaId, p.Ativo
    FROM Produtos p
    INNER JOIN STRING_SPLIT(@CategoriaIds, ',') s
        ON p.CategoriaId = TRY_CAST(s.value AS INT)
    WHERE p.Ativo = 1
    ORDER BY NEWID();
END;
GO

-- ============================================================
-- 10. VIEWS
-- ============================================================

CREATE OR ALTER VIEW vwHistoricoPesquisaCompleto
AS
SELECT
    h.Id,
    h.UsuarioId,
    u.Nome,
    u.Email,
    h.Query,
    h.SearchDate
FROM HistoricoPesquisas h
INNER JOIN Usuarios u ON u.Id = h.UsuarioId;
GO

CREATE OR ALTER VIEW vwHistoricoResumo
AS
SELECT
    UsuarioId,
    Query,
    LEFT(Query, 200) AS QueryResumo,
    SearchDate
FROM HistoricoPesquisas;
GO

CREATE OR ALTER VIEW vwUsuarioPublico
AS
SELECT
    u.Id,
    u.Nome,
    u.Email,
    u.DataCriacao,
    COUNT(uc.CategoriaId) AS TotalCategoriasFavoritas
FROM Usuarios u
LEFT JOIN UsuarioCategorias uc ON uc.UsuarioId = u.Id
GROUP BY u.Id, u.Nome, u.Email, u.DataCriacao;
GO

CREATE OR ALTER VIEW vwUsuarioAdmin
AS
SELECT
    u.Id,
    u.Nome,
    u.Email,
    u.DataCriacao,
    COUNT(DISTINCT uc.CategoriaId) AS TotalCategoriasFavoritas,
    COUNT(h.Id)                    AS TotalPesquisas
FROM Usuarios u
LEFT JOIN UsuarioCategorias uc ON uc.UsuarioId = u.Id
LEFT JOIN HistoricoPesquisas h ON h.UsuarioId  = u.Id
GROUP BY u.Id, u.Nome, u.Email, u.DataCriacao;
GO

-- ============================================================
-- 11. FUNCTIONS
-- ============================================================

CREATE OR ALTER FUNCTION fnTotalPerguntaUsuario
(
    @UsuarioId INT
)
RETURNS INT
AS
BEGIN
    DECLARE @Total INT;
    SELECT @Total = COUNT(*)
    FROM HistoricoPesquisas
    WHERE UsuarioId = @UsuarioId;
    RETURN @Total;
END;
GO

CREATE OR ALTER FUNCTION fnUltimaPerguntaUsuario
(
    @UsuarioId INT
)
RETURNS NVARCHAR(500)
AS
BEGIN
    DECLARE @Pergunta NVARCHAR(500);
    SELECT TOP 1 @Pergunta = Query
    FROM HistoricoPesquisas
    WHERE UsuarioId = @UsuarioId
    ORDER BY SearchDate DESC;
    RETURN @Pergunta;
END;
GO

CREATE OR ALTER FUNCTION fnTotalUsuarios()
RETURNS INT
AS
BEGIN
    DECLARE @Total INT;
    SELECT @Total = COUNT(*) FROM Usuarios;
    RETURN @Total;
END;
GO

CREATE OR ALTER FUNCTION fnTicktMedioUsuario()
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Media DECIMAL(10,2);
    ;WITH CategoriasPorUsuario AS
    (
        SELECT u.Id AS UsuarioId, COUNT(uc.CategoriaId) AS QuantidadeCategorias
        FROM Usuarios u
        LEFT JOIN UsuarioCategorias uc ON uc.UsuarioId = u.Id
        GROUP BY u.Id
    )
    SELECT @Media = ISNULL(AVG(CAST(QuantidadeCategorias AS DECIMAL(10,2))), 0)
    FROM CategoriasPorUsuario;
    RETURN @Media;
END;
GO

CREATE OR ALTER FUNCTION fnUsuarioPorCategoria
(
    @Categoria NVARCHAR(100)
)
RETURNS INT
AS
BEGIN
    DECLARE @Total INT;
    SELECT @Total = COUNT(DISTINCT uc.UsuarioId)
    FROM UsuarioCategorias uc
    INNER JOIN Categorias c ON c.Id = uc.CategoriaId
    WHERE c.Nome = @Categoria
      AND c.Ativa = 1;
    RETURN @Total;
END;
GO

-- ============================================================
-- 12. VERIFICAÇÃO FINAL
-- ============================================================
PRINT '';
PRINT '=== SETUP CONCLUIDO ===';
PRINT 'Tabelas: ' + CAST((SELECT COUNT(*) FROM sys.tables WHERE name IN (
    'Usuarios','Categorias','Produtos','HistoricoPesquisas','UsuarioCategorias')) AS NVARCHAR) + '/5';
PRINT 'Stored Procedures: ' + CAST((SELECT COUNT(*) FROM sys.procedures WHERE name IN (
    'spUsuarioCriar','spUsuarioObterPorId','spUsuarioObterPorEmail','spUsuarioAtualizar',
    'spUsuarioObterComHistorico','spRegistraHistoricoPesquisa','spListarHistoricoPorUsuario',
    'spCategoriaListarAtivas','spCategoriaObterPorId','spCategoriaObterPorIds',
    'spUsuarioCategoriaObterPorUsuario','spUsuarioCategoriaAtualizar',
    'spProdutoBuscar','spProdutoObterRecomendacoes')) AS NVARCHAR) + '/14';
PRINT 'Views: ' + CAST((SELECT COUNT(*) FROM sys.views WHERE name IN (
    'vwHistoricoPesquisaCompleto','vwHistoricoResumo','vwUsuarioPublico','vwUsuarioAdmin')) AS NVARCHAR) + '/4';
PRINT 'Functions: ' + CAST((SELECT COUNT(*) FROM sys.objects WHERE type = 'FN' AND name IN (
    'fnTotalPerguntaUsuario','fnUltimaPerguntaUsuario','fnTotalUsuarios',
    'fnTicktMedioUsuario','fnUsuarioPorCategoria')) AS NVARCHAR) + '/5';
PRINT 'Categorias seed: ' + CAST((SELECT COUNT(*) FROM Categorias) AS NVARCHAR) + ' registros';
PRINT 'Produtos seed: '   + CAST((SELECT COUNT(*) FROM Produtos) AS NVARCHAR) + ' registros';
GO
