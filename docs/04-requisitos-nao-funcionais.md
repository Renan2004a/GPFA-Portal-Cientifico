# 04 — Requisitos não funcionais

Os RNFs são mensuráveis ou verificáveis por inspeção, teste ou configuração. Quando aplicável, indica-se a evidência.

## RNF001 — Segurança de autenticação

- **Descrição:** senhas não podem ser armazenadas em texto puro nem recuperadas.
- **Implementação:** PBKDF2-HMAC-SHA256, 120.000 iterações, salt aleatório de 16 bytes, hash de 32 bytes; comparação em tempo constante.
- **Verificação:** `AutenticacaoTests.Senhas_NaoPodemEstarEmTextoPuroNoBanco`; inspeção do `SenhaHash`.
- **Critério:** nenhum valor de senha aparece no banco ou no código.

## RNF002 — Proteção contra SQL Injection

- **Descrição:** todas as consultas que recebem dados externos devem usar parâmetros.
- **Implementação:** `SqlParameter` em todos os repositórios; nenhuma concatenação de entrada.
- **Verificação:** `InfraestruturaTests.ConsultaParametrizada_NaoDevePermitirInjecao` e teste de login com payload.
- **Critério:** payloads são tratados como texto e não alteram o schema.

## RNF003 — Controle de sessão e CSRF

- **Descrição:** sessão expira em 60 minutos; ViewState é vinculado à sessão.
- **Implementação:** Forms Authentication, `Session` InProc, `ViewStateUserKey = Session.SessionID`, cookies `HttpOnly` e `SameSite=Lax`.
- **Verificação:** inspeção de `BasePage.OnPreInit` e `Web.config`.
- **Critério:** requisição com ViewState de outra sessão é rejeitada.

## RNF004 — Autorização por perfil

- **Descrição:** nenhum recurso administrativo pode ser acessado anonimamente; escopo de coordenador é limitado aos próprios projetos.
- **Implementação:** `AdminPageBase`, `ExigirPerfis`, `ExigirGestao`, `ExigirAdministrador`, verificações nos serviços.
- **Verificação:** smoke tests com os três usuários e testes de `ProjetoService`/`PublicacaoService`.
- **Critério:** acessos indevidos resultam em 403 ou exceção de regra.

## RNF005 — Integridade dos dados

- **Descrição:** o banco deve impedir violações das regras críticas mesmo fora da aplicação.
- **Implementação:** PKs, FKs `NO ACTION`, `UNIQUE`, `CHECK`, índices filtrados e triggers.
- **Verificação:** `IntegridadeBancoTests` (11 cenários).
- **Critério:** operações inválidas lançam `SqlException` e não alteram dados.

## RNF006 — Desempenho

- **Descrição:** páginas públicas devem responder em tempo aceitável para o volume acadêmico (dezenas de registros).
- **Implementação:** índices em FKs, ano, situação; views consolidadas; carregamento por página; ausência de consultas em loop nos diretórios públicos.
- **Meta:** < 2 s para carregar uma página pública em LocalDB no ambiente de desenvolvimento.
- **Verificação:** medição local e Lighthouse (sem problemas de boas práticas).

## RNF007 — Usabilidade

- **Descrição:** o sistema deve ser operável por usuários não técnicos, com feedback claro.
- **Implementação:** navegação consistente, labels, ajuda contextual, mensagens de sucesso/erro, confirmação de exclusão, estados vazios.
- **Verificação:** manual do usuário e inspeção das telas.
- **Critério:** todas as ações destrutivas exigem confirmação; erros de validação aparecem junto ao formulário.

## RNF008 — Acessibilidade

- **Descrição:** seguir boas práticas WCAG 2.1 AA no que for aplicável.
- **Implementação:** `lang="pt-BR"`, skip link, foco visível, contraste, textos alternativos, labels associados, cabeçalhos hierárquicos, `aria-live` para mensagens, tabelas com cabeçalhos, navegação por teclado.
- **Verificação:** Lighthouse (acessibilidade ≥ 0,9) e inspeção manual.
- **Critério:** nenhum bloqueio crítico de teclado; contraste adequado no texto principal.

## RNF009 — Responsividade

- **Descrição:** layout funcional de 320 px a 1440+ px.
- **Implementação:** CSS Grid/Flex, breakpoints em 1100/920/640 px, navegação colapsável, tabelas com scroll horizontal, sidebar administrativa off-canvas.
- **Verificação:** inspeção em dispositivos simulados e regras CSS.
- **Critério:** nenhum conteúdo essencial fica inacessível em telas pequenas.

## RNF010 — Compatibilidade

- **Descrição:** executar no ambiente definido da disciplina.
- **Implementação:** .NET Framework 4.8.1, ASP.NET Web Forms, IIS Express, SQL Server LocalDB; VS 2026.
- **Verificação:** build por MSBuild e execução em IIS Express.
- **Critério:** solução abre e compila no Visual Studio sem pacotes externos obrigatórios além dos vendorizados.

## RNF011 — Manutenibilidade

- **Descrição:** código organizado em camadas e com responsabilidades claras.
- **Implementação:** projetos separados, repositórios, serviços, DTOs, validações centralizadas, nomes em português consistentes, comentários nos pontos não óbvios.
- **Verificação:** inspeção estrutural e documento 11.
- **Critério:** a apresentação não contém SQL; a camada de dados não contém regras de UI.

## RNF012 — Tratamento de exceções

- **Descrição:** erros de banco e negócio devem gerar mensagens compreensíveis e log.
- **Implementação:** `SqlErrorTranslator`, `RegraNegocioException`, `IntegridadeDadosException`, `Global.Application_Error`, log em `App_Data/logs`.
- **Verificação:** testes de integridade e inspeção do log.
- **Critério:** nenhuma stack trace é exibida ao usuário final.

## RNF013 — Disponibilidade e recuperação

- **Descrição:** o sistema deve ser reiniciável e o banco recriável por script.
- **Implementação:** scripts idempotentes, `Setup-Database.ps1`, log de erros, sessão com timeout.
- **Verificação:** execução de `-Recreate` e nova abertura do portal.
- **Critério:** ambiente pode ser reconstruído em uma máquina limpa seguindo o manual.

## RNF014 — Organização do código e versionamento

- **Descrição:** histórico compreensível e sem artefatos.
- **Implementação:** Conventional Commits, `.gitignore`, branches `main`/`develop`/`feature`, README e docs.
- **Verificação:** `git log` e estrutura do repositório.
- **Critério:** nenhum `bin/`, `obj/`, `.vs/`, log ou credencial versionado.

## RNF015 — Consistência visual e identidade

- **Descrição:** interface autoral, coerente e institucional.
- **Implementação:** design system em CSS custom properties, tipografia serifada/sans, paleta azul-petróleo/âmbar, cards, ícones SVG, componentes reutilizáveis.
- **Verificação:** inspeção das páginas públicas e administrativas; capturas em `docs/images`.
- **Critério:** mesma identidade em todas as telas, sem aparência de template genérico.

---

## Matriz de verificação

| RNF | Evidência principal | Status |
|---|---|---|
| 001 | Hash PBKDF2 + teste | ✅ |
| 002 | SQL parametrizado + testes | ✅ |
| 003 | BasePage + Web.config | ✅ |
| 004 | BasePage + serviços + smoke | ✅ |
| 005 | Schema + 11 testes de banco | ✅ |
| 006 | Índices/views + Lighthouse | ✅ |
| 007 | Manual + telas | ✅ |
| 008 | Lighthouse 0,93 + revisão | ✅ (melhorias contínuas) |
| 009 | CSS responsivo | ✅ |
| 010 | Build VS/MSBuild + IIS Express | ✅ |
| 011 | Camadas + inspeção | ✅ |
| 012 | Tradutor + log | ✅ |
| 013 | Scripts de setup | ✅ |
| 014 | Git + documentação | ✅ |
| 015 | CSS autoral + screenshots | ✅ |
