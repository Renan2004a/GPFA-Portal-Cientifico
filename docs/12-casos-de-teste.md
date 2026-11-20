# 12 — Casos de teste

## 1. Estratégia

| Nível | Ferramenta | Escopo |
|---|---|---|
| Integração de negócio | MSTest | Serviços, validações, autorização e transações |
| Integração de banco | MSTest + SQL direto | Constraints, índices filtrados e triggers |
| Smoke de interface | Navegador/HTTP | Login, permissões, criação/edição/exclusão via Web Forms |
| Manual exploratório | Navegador | Navegação, responsividade, acessibilidade e estados vazios |

**Banco de teste:** `GPFA_Portal_Test`, recriado do zero a cada execução a partir dos mesmos scripts versionados.

**Comando:**

```powershell
dotnet test .\tests\GPFA.Tests\GPFA.Tests.csproj
```

**Resultado da última execução:**

```text
Aprovado! – Com falha: 0, Aprovado: 83, Ignorado: 0, Total: 83
Duração: ~8 s
```

## 2. Casos exigidos pela disciplina

| # | Caso | Objetivo | Dados | Resultado esperado | Obtido | Status |
|---|---|---|---|---|---|---|
| CT01 | Cadastro de pesquisador | Criar perfil completo | Nome, e-mail único, formação, área, URLs | Registro criado e visível no diretório | Criado; teste `Pesquisador_DevePermitirCrudCompleto` | ✅ |
| CT02 | Edição de pesquisador | Alterar nome/biografia | Registro do CT01 | Dados atualizados no banco e no público | Atualizado; mesmo teste | ✅ |
| CT03 | Exclusão de pesquisador | Excluir sem vínculos | Registro do CT01 | Registro removido | Removido; mesmo teste | ✅ |
| CT04 | Cadastro de estudante | Criar com orientador e projeto | Nome, e-mail, nível, curso, orientador ativo, projeto | Estudante + orientação + vínculo criados | Criado; `Estudante_DeveSerCriado...` | ✅ |
| CT05 | Obrigatoriedade de orientador | Impedir sem orientador | Lista vazia/ID 0 | Erro de validação | Bloqueado; `Estudante_SemOrientador...` | ✅ |
| CT06 | Obrigatoriedade de projeto | Impedir sem projeto | Lista vazia | Erro de validação | Bloqueado; `Estudante_SemProjeto...` | ✅ |
| CT07 | Coorientadores externos | Registrar texto livre | “Prof. Externo” | Texto persistido e exibido | Persistido; `Estudante_DeveSerCriado...` | ✅ |
| CT08 | Cadastro de projeto | Criar projeto com coordenador | Título, resumo, datas, coordenador | Projeto criado | Criado; `Projeto_DevePermitirCrudCompleto` | ✅ |
| CT09 | Cadastro de coordenador | Garantir coordenador único e válido | Pesquisador ativo | Um coordenador por projeto | Garantido por FK/serviço | ✅ |
| CT10 | Múltiplos pesquisadores | Adicionar dois participantes | Projeto + 2 pesquisadores | Total de 3 pessoas (coord.+2) | 3; `Projeto_AdministradorPodeAdicionar...` | ✅ |
| CT11 | Cadastro de publicações | Criar com autores e DOI | Título, veículo, ano, DOI, 2 autores | Publicação e autores persistidos | Criado; `Publicacao_DevePermitir...` | ✅ |
| CT12 | Limite de cinco destaques | Impedir sexto | 6 publicações do mesmo autor | Sexto bloqueado | Bloqueado; `Destaques_NaoPodemUltrapassarCinco` | ✅ |
| CT13 | Cadastro de financiamentos | Criar valores por fonte | Projeto, fonte, valor, datas | Registro criado | Criado; `Financiamento_DeveSomar...` | ✅ |
| CT14 | Cálculo do total captado | Somar sem duplicidade | 10.000,50 + 25.000,25 | 35.000,75 | 35.000,75 | ✅ |
| CT15 | Relacionamentos N:N | Validar associativas | Participações, autores, vínculos | Registros nas tabelas corretas | Validado nos testes e no schema | ✅ |
| CT16 | Integridade referencial | Bloquear exclusões inválidas | Pesquisador/projeto/fonte com vínculos | Erro compreensível | Bloqueado; `IntegridadeBancoTests` | ✅ |
| CT17 | Autenticação | Login válido/inválido/bloqueio | Credenciais de demo | Só válidas entram; 5 falhas bloqueiam | Validado; `AutenticacaoTests` | ✅ |
| CT18 | Controle de permissões | Restringir por perfil | Admin/coord/pesq. | 403/escopo correto | Validado; smoke + serviços | ✅ |
| CT19 | Operações CRUD | Criar/ler/atualizar/excluir | Cada módulo | CRUD completo | 83 testes + smoke | ✅ |
| CT20 | Relatórios | Conferir totais e agrupamentos | Dados de demo | Números coerentes com views | Validado; `RelatoriosTests` | ✅ |

## 3. Casos adicionais de banco

| Caso | Entrada | Esperado | Status |
|---|---|---|---|
| Financiamento negativo | `Valor = -1` | `CHECK` violado | ✅ |
| Segunda principal ativa | Inserir outra orientação principal | Índice filtrado violado | ✅ |
| Coordenador como participante | Inserir par (projeto, coordenador) | Trigger viola | ✅ |
| Remover último projeto | `DELETE` do vínculo de estudante ativo | Trigger viola | ✅ |
| Remover última principal | `DELETE` da orientação ativa | Trigger viola | ✅ |
| Excluir pesquisador com projeto | `DELETE Pesquisador` | FK viola | ✅ |
| Excluir projeto com publicação | `DELETE Projeto` | FK viola | ✅ |
| Situação inválida | `UPDATE Situacao` | CHECK viola | ✅ |
| Nível inválido | `UPDATE NivelAcademico` | CHECK viola | ✅ |
| Login duplicado | `INSERT Usuario` com `admin` | UNIQUE viola | ✅ |
| Destaque em posição ocupada | Ordem repetida | UNIQUE viola | ✅ |
| SQL Injection | `' OR 1=1 --` / `'; DROP...` | Tratado como texto | ✅ |

## 4. Casos de autenticação e segurança

| Caso | Esperado | Status |
|---|---|---|
| Login admin/coordenador/pesquisador | Sucesso e perfil correto | ✅ |
| Senha incorreta | Falha genérica | ✅ |
| Usuário inexistente | Falha sem revelar existência | ✅ |
| Cinco falhas | Bloqueio temporário de 15 min | ✅ |
| Usuário inativo | Falha | ✅ |
| Troca de senha | Exige senha atual, valida força e confirmação | ✅ |
| Hash no banco | Não contém senha em texto | ✅ |
| Último administrador | Não pode ser desativado/excluído | ✅ |
| Exclusão do próprio usuário | Bloqueada | ✅ |
| Coordenador sem pesquisador | Rejeitado | ✅ |

## 5. Smokes de interface executados

| Fluxo | Evidência |
|---|---|
| Login real no navegador e redirecionamento ao painel | Sessão + dashboard |
| Criação de pesquisador pelo formulário Web Forms | Mensagem “Pesquisador cadastrado com sucesso”; registro na GridView |
| Edição de pesquisador | Nome atualizado e persistido |
| Exclusão com confirmação | Mensagem “Pesquisador excluído com sucesso” |
| Criação de projeto | Mensagem de sucesso; projeto no portfólio |
| Criação de estudante com orientador e projeto | Mensagem de sucesso; vínculos criados |
| Criação de financiamento | Total exibido passou de R$ 755.000 para R$ 767.346 |
| Criação de publicação com autor | Mensagem de sucesso; publicação na lista |
| Acesso de coordenador a página de admin | Página 403 |
| Acesso de pesquisador a projeto | Página 403 |
| Destaques de Helena | 5 de 5; botão de adicionar disponível apenas em posições livres |

## 6. Verificação de requisitos não funcionais

| RNF | Verificação | Resultado |
|---|---|---|
| Segurança de senha | Inspeção do banco + teste | ✅ |
| SQL Injection | Payload em login e nome | ✅ |
| CSRF | `ViewStateUserKey` | ✅ |
| Permissões | Smoke com 3 perfis | ✅ |
| Integridade | 11 testes de banco | ✅ |
| Responsividade | Breakpoints + inspeção | ✅ |
| Acessibilidade | Lighthouse: 0,93 | ✅ (texto principal com contraste corrigido) |
| Erros | Log + página amigável | ✅ |
| Documentação | 15 documentos + README | ✅ |

## 7. Como reproduzir

1. Recrie o banco de desenvolvimento:

```powershell
.\scripts\Setup-Database.ps1 -Recreate
```

2. Compile e abra o portal.
3. Execute os testes automatizados:

```powershell
dotnet test .\tests\GPFA.Tests\GPFA.Tests.csproj
```

4. Para inspecionar manualmente:

```sql
SELECT * FROM vw_IndicadoresGerais;
SELECT * FROM vw_ProjetoResumo;
SELECT * FROM vw_FinanciamentoPorProjeto;
SELECT * FROM vw_AlertasIntegridade;
```

## 8. Defeitos encontrados e corrigidos

| Defeito | Causa | Correção |
|---|---|---|
| Leitura de `SMALLINT/TINYINT` falhava | `GetInt32` em `Ano`/`Ordem` | `Convert.ToInt32` no mapper |
| `SiteUrl` ausente na view de fontes | Mapper esperava coluna não projetada | Coluna adicionada à view |
| Flash não aparecia após redirect | `Redirect(false)` permitia render da página atual e consumia o flash | `Response.Redirect(url, true)` + ignora `ThreadAbortException` no log |
| `CheckBoxList` com `UnorderedList` + colunas | Layout não suportado | `RepeatLayout="Flow"` com CSS flex |
| Troca de orientador bloqueada | Trigger `AFTER UPDATE` via a ausência temporária de principal | Trigger passou a atuar em `DELETE`; validação de encerramento no serviço |
| Exclusão de usuário bloqueada por auditoria | FK `NO ACTION` | FK `ON DELETE SET NULL` |
| Underline do menu ocupava a largura do header | `position:absolute` sem ancestral posicionado | `position:relative` no link |

## 9. Conclusão

Os 20 cenários exigidos foram cobertos. Os 83 testes automatizados passam em uma execução limpa, e os fluxos principais de interface foram exercitados em navegador real. Não há funcionalidade apresentada como pronta sem implementação correspondente.
