# Banco de dados — GPFA

Scripts SQL Server versionados. A execução recomendada é pelo script único:

```powershell
.\scripts\Setup-Database.ps1 -Recreate
```

## Ordem dos scripts

| Script | Conteúdo | Executa |
|---|---|---|
| `00-create-database.sql` | Cria o banco | `sqlcmd` |
| `01-schema.sql` | Tabelas, FKs, CHECKs, índices filtrados, triggers | `sqlcmd` |
| `02-views.sql` | Views de consulta e relatórios | `sqlcmd` |
| `03-seed.sql` | Dados fictícios de demonstração | `sqlcmd` |
| `04-seed-usuarios.sql` | Usuários com hash PBKDF2 | `sqlcmd` |
| `99-drop-database.sql` | Remove o banco (desenvolvimento) | `sqlcmd` |

## Variável de banco

Os scripts usam `$(DbName)`, definida via `-v DbName="GPFA_Portal"`. O `Setup-Database.ps1` já cuida disso. Para criar outro banco:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -b -f 65001 -i 00-create-database.sql -v DbName="GPFA_Homolog"
sqlcmd -S "(localdb)\MSSQLLocalDB" -b -f 65001 -i 01-schema.sql -v DbName="GPFA_Homolog"
```

## Banco de testes

A suíte `GPFA.Tests` cria `GPFA_Portal_Test` automaticamente, executando estes mesmos scripts e recriando o banco a cada execução. O banco de demonstração não é alterado.

## Cuidados

- `99-drop-database.sql` apaga dados. Use apenas em desenvolvimento.
- Não versione arquivos `.mdf`, `.ldf` ou backups.
- A connection string de desenvolvimento usa `(localdb)\MSSQLLocalDB` e autenticação integrada.
- Em produção, crie um usuário SQL com permissões mínimas e habilite backup.
