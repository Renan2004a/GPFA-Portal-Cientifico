# 13 — Manual de instalação

## 1. Pré-requisitos

| Item | Versão/teste | Observação |
|---|---|---|
| Windows | 10/11 | Ambiente de desenvolvimento |
| Visual Studio | 2026 Community 18.10 (ou 2022) | Carga “Desenvolvimento Web e ASP.NET” |
| .NET Framework | 4.8.1 | Incluído no VS/Windows |
| SQL Server LocalDB | `MSSQLLocalDB` (SQL 2022) | Instância local |
| `sqlcmd` | 15+ | Incluído com VS/SQL Server |
| Git | 2.40+ | Versionamento |
| Navegador | Edge/Chrome/Firefox atualizados | Interface |

### Verificar o ambiente

```powershell
dotnet --info
& "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -property installationPath
sqllocaldb info MSSQLLocalDB
sqlcmd -?
git --version
```

> **Importante:** ASP.NET Web Forms exige **.NET Framework**, não .NET 8/9/10. O projeto usa `v4.8.1`. O SDK `dotnet` é usado apenas para compilar as bibliotecas e rodar os testes; o site é compilado pelo MSBuild do Visual Studio.

## 2. Obter o código

```powershell
git clone <URL_DO_REPOSITORIO>
cd GPFA-Portal-Cientifico
```

## 3. Criar o banco

O script abaixo remove e recria o banco de desenvolvimento, executa schema, views, dados e usuários:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Setup-Database.ps1 -Recreate
```

Saída esperada:

```text
Banco 'GPFA_Portal' pronto.
```

### O que o script faz

1. Inicia a instância LocalDB (se necessário).
2. Remove `GPFA_Portal` (se `-Recreate`).
3. Cria o banco.
4. Executa `01-schema.sql` (tabelas, constraints, índices, triggers).
5. Executa `02-views.sql`.
6. Insere dados de demonstração e usuários.
7. Retorna erro não zero se qualquer script falhar.

### Execução manual (alternativa)

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -b -f 65001 -i .\database\00-create-database.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -b -f 65001 -i .\database\01-schema.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -b -f 65001 -i .\database\02-views.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -b -f 65001 -i .\database\03-seed.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -b -f 65001 -i .\database\04-seed-usuarios.sql
```

## 4. Configurar a conexão

`src/GPFA.Portal/Web.config`:

```xml
<connectionStrings>
  <add name="GPFA"
       connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GPFA_Portal;Integrated Security=True;MultipleActiveResultSets=True;Application Name=GPFA-Portal"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Para outro servidor:

```text
Data Source=.\SQLEXPRESS;Initial Catalog=GPFA_Portal;Integrated Security=True;
```

**Nunca** versione senhas reais. Em produção, use transformação de configuração ou variável de ambiente.

## 5. Abrir no Visual Studio

1. Abra `GPFA.sln`.
2. Confirme que os projetos `GPFA.Models`, `GPFA.Data`, `GPFA.Business` e `GPFA.Portal` foram carregados.
3. Clique com o botão direito em `GPFA.Portal` → **Definir como projeto de inicialização**.
4. Verifique em **Propriedades → Web** a URL do IIS Express (ex.: `http://localhost:54321/`).
5. Pressione **F5**.
6. O navegador abre em `http://localhost:54321/`.

### Build por linha de comando

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" `
  .\src\GPFA.Portal\GPFA.Portal.csproj /p:Configuration=Debug
```

## 6. Executar

### Portal público

`http://localhost:54321/Default.aspx`

### Área restrita

`http://localhost:54321/Login.aspx`

| Perfil | Login | Senha |
|---|---|---|
| Administrador | `admin` | `Gpfa@2026` |
| Coordenador | `coordenador` | `Coord@2026` |
| Pesquisador | `pesquisador` | `Pesq@2026` |

As credenciais de demonstração só aparecem na tela quando `DemoMode=true`.

### IIS Express manual

```powershell
& "C:\Program Files\IIS Express\iisexpress.exe" `
  /path:"$PWD\src\GPFA.Portal" /port:54321 /systray:false
```

## 7. Testes

```powershell
dotnet test .\tests\GPFA.Tests\GPFA.Tests.csproj
```

O teste cria/recria `GPFA_Portal_Test`. Ele **não** altera o banco de demonstração `GPFA_Portal`.

Requisitos:

- Acesso ao LocalDB.
- Permissão para criar/dropar `GPFA_Portal_Test`.
- Pacotes NuGet restaurados (MSTest).

## 8. Solução de problemas

| Sintoma | Causa provável | Solução |
|---|---|---|
| “A cadeia de conexão 'GPFA' não foi configurada” | `Web.config` ausente/errado | Verifique a seção `connectionStrings` |
| “Cannot open database GPFA_Portal” | Banco não criado | Execute `Setup-Database.ps1` |
| “Login failed for user” | Autenticação integrada | Rode como o mesmo usuário do Windows que criou o LocalDB |
| Página de erro genérica | Exceção logada | Consulte `src/GPFA.Portal/App_Data/logs/erros-*.log` |
| CSS antigo após alterações | Cache do navegador | Ctrl+F5 ou aumente `?v=` no `Site.Master` |
| `sqllocaldb` não encontrado | LocalDB não instalado | Instale “SQL Server Express LocalDB” pelo instalador do VS |
| `sqlcmd` não encontrado | Ferramenta fora do PATH | Use o caminho do VS ou instale SQL Server Command Line Utilities |
| Porta 54321 ocupada | Outra instância | Altere `/port:` e a URL do projeto |
| Erro ao restaurar NuGet | Sem internet/proxy | Restaure no VS ou configure o feed |
| `CheckBoxList` não renderiza | Layout incompatível | Use `RepeatLayout="Flow"` (já configurado) |

### Limpar cache de compilação

```powershell
Get-Process iisexpress -ErrorAction SilentlyContinue | Stop-Process -Force
Remove-Item .\src\GPFA.Portal\bin -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item .\src\GPFA.Portal\obj -Recurse -Force -ErrorAction SilentlyContinue
```

## 9. Colaboração

### Clonar e configurar

```powershell
git clone <URL>
cd GPFA-Portal-Cientifico
git config user.name "Seu Nome"
git config user.email "seu@email"
```

### Fluxo de branches

```powershell
git checkout develop
git pull origin develop
git checkout -b feature/minha-funcionalidade

# implementar e testar
git add .
git commit -m "feat: descreve a funcionalidade"
git push -u origin feature/minha-funcionalidade
```

1. Abra um Pull Request para `develop`.
2. Solicite revisão de outro integrante.
3. Após aprovação e testes, faça merge.
4. `develop` é promovida para `main` nas entregas.

### Convenções de commit

- `feat:` nova funcionalidade
- `fix:` correção
- `docs:` documentação
- `test:` testes
- `refactor:` refatoração sem mudança de comportamento
- `chore:` configuração/infraestrutura

### O que não versionar

`bin/`, `obj/`, `.vs/`, `packages/`, `*.user`, `TestResults/`, `App_Data/logs/`, arquivos temporários e credenciais.

## 10. Publicação (opcional)

Para IIS completo:

1. Publique o projeto `GPFA.Portal` (Release) em uma pasta.
2. Crie um site/aplicativo no IIS apontando para a pasta.
3. Habilite **Autenticação Anônima** e **Forms Authentication**.
4. Instale o .NET Framework 4.8.1 no servidor.
5. Ajuste a connection string e `customErrors mode="On"`.
6. Configure HTTPS e `requireSSL="true"` no elemento `<forms>`.

Para SQL Server completo, execute os scripts no servidor e crie um usuário de aplicação com permissões mínimas (`SELECT`, `INSERT`, `UPDATE`, `DELETE`, `EXECUTE`).

## 11. Checklist de instalação

- [ ] VS com carga Web instalado
- [ ] .NET Framework 4.8.1 presente
- [ ] LocalDB iniciado
- [ ] Banco criado sem erros
- [ ] Solution abre e compila
- [ ] Portal abre no IIS Express
- [ ] Login `admin` funciona
- [ ] Dashboard mostra 6 pesquisadores, 13 estudantes, 6 projetos
- [ ] `dotnet test` retorna 83 aprovados
- [ ] Logs em `App_Data/logs` sem erros inesperados
