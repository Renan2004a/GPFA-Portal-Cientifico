/* ============================================================================
   GPFA - Script 99 - Remoção controlada do banco de dados
   ATENÇÃO: apaga todos os dados. Use apenas em ambiente de desenvolvimento.
   Uso: sqlcmd -S "(localdb)\MSSQLLocalDB" -i 99-drop-database.sql -v DbName="GPFA_Portal"
   ============================================================================ */
:setvar DbName "GPFA_Portal"

IF DB_ID(N'$(DbName)') IS NOT NULL
BEGIN
    PRINT N'Removendo banco $(DbName)...';
    ALTER DATABASE [$(DbName)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(DbName)];
END
GO
