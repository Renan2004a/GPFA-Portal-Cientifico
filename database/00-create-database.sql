/* ============================================================================
   GPFA - Portal de Visibilidade Científica e Gestão de Projetos
   Script 00 - Criação do banco de dados
   Uso: sqlcmd -S "(localdb)\MSSQLLocalDB" -i 00-create-database.sql -v DbName="GPFA_Portal"
   ============================================================================ */
:setvar DbName "GPFA_Portal"

IF DB_ID(N'$(DbName)') IS NULL
BEGIN
    PRINT N'Criando banco de dados $(DbName)...';
    CREATE DATABASE [$(DbName)];
END
ELSE
BEGIN
    PRINT N'Banco de dados $(DbName) já existe.';
END
GO

ALTER DATABASE [$(DbName)] SET RECOVERY SIMPLE;
GO
