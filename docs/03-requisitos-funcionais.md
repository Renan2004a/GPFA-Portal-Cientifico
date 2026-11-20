# 03 — Requisitos funcionais

**Convenções**

- **Prioridade:** Essencial (E), Importante (I), Desejável (D).
- **Atores:** Visitante, Pesquisador, Coordenador, Administrador, Sistema.
- Todos os requisitos foram implementados, salvo indicação explícita em contrário.
- A rastreabilidade com histórias de usuário está na coluna “HU”.

---

## RF001 — Autenticação de usuários

- **Ator:** Administrador, Coordenador, Pesquisador.
- **Descrição:** o sistema deve autenticar usuários por login e senha, criar sessão e permitir encerramento seguro.
- **Dados:** login, senha, perfil, pesquisador vinculado, último acesso, tentativas falhas, bloqueio.
- **Regras:** senha nunca em texto puro; bloqueio após 5 falhas por 15 minutos; sessão de 60 minutos; mensagens genéricas para credenciais inválidas.
- **Critérios de aceitação:**
  1. Credenciais válidas redirecionam para o painel.
  2. Credenciais inválidas exibem “Login ou senha inválidos”.
  3. Usuário inativo não autentica.
  4. Após 5 falhas a conta é bloqueada temporariamente.
  5. “Sair” limpa Session, cookie de autenticação e retorna ao portal.
- **Prioridade:** E — **HU001**

## RF002 — Controle de acesso por perfil

- **Ator:** Sistema.
- **Descrição:** restringir páginas e operações conforme o perfil.
- **Regras:** Administrador acessa tudo; Coordenador gerencia apenas projetos sob sua coordenação; Pesquisador acessa painel/relatórios em leitura; visitante acessa somente páginas públicas.
- **Critérios:**
  1. Acesso anônimo a `/Admin` redireciona ao login.
  2. Coordenador recebe 403 ao abrir cadastros globais/usuários.
  3. Coordenador não edita projeto de outro coordenador.
  4. Pesquisador recebe 403 em páginas de gestão.
- **Prioridade:** E — **HU002**

## RF003 — Diretório público de pesquisadores

- **Ator:** Visitante.
- **Descrição:** listar pesquisadores ativos com foto, nome, formação, área, links externos e acesso ao perfil.
- **Dados:** pessoa, pesquisador, links, contadores.
- **Regras:** exibir somente ativos; busca por nome/formação/área/linha; filtro por área.
- **Critérios:**
  1. Cards exibem todos os campos exigidos.
  2. Busca e filtro retornam subconjuntos corretos.
  3. Estado vazio é apresentado quando não há resultados.
  4. Links externos abrem em nova aba com `rel="noopener noreferrer"`.
- **Prioridade:** E — **HU003**

## RF004 — Perfil individual do pesquisador

- **Ator:** Visitante.
- **Descrição:** apresentar a trajetória completa do pesquisador.
- **Dados:** foto, nome, formação, biografia, área, links, projetos, coordenações, orientandos, publicações e destaques.
- **Regras:** separar orientandos atuais e passados; exibir até 5 destaques ordenados; não misturar grupos.
- **Critérios:**
  1. Todos os campos obrigatórios aparecem.
  2. Orientações atuais e encerradas aparecem em seções distintas.
  3. Destaques aparecem numerados e limitados a 5.
  4. Perfil inexistente exibe página amigável.
- **Prioridade:** E — **HU004**

## RF005 — Diretório público de estudantes

- **Ator:** Visitante.
- **Descrição:** listar estudantes com nome, formação, curso, nível, orientador principal, coorientadores, projetos e situação.
- **Regras:** filtros por texto, nível, situação e orientador; exibir vínculo atual.
- **Critérios:**
  1. Todos os campos exigidos aparecem.
  2. Filtros funcionam isoladamente e em conjunto.
  3. Estudante sem projeto/orientador não deve existir entre ativos.
- **Prioridade:** E — **HU005**

## RF006 — Gestão de pesquisadores

- **Ator:** Administrador.
- **Descrição:** CRUD de pesquisadores com dados acadêmicos e de divulgação.
- **Dados:** nome, e-mail, formação, área, titulação, instituição, departamento, linha, foto, biografia, Scholar, ORCID, Lattes, ativo.
- **Regras:** e-mail único; URLs http/https; não excluir com projetos, participações, orientações, publicações ou usuário vinculado.
- **Critérios:**
  1. Cadastro válido aparece no diretório.
  2. Edição reflete no perfil público.
  3. Exclusão com vínculos é bloqueada com mensagem específica.
  4. E-mail duplicado é rejeitado.
  5. URLs inválidas são rejeitadas.
- **Prioridade:** E — **HU006**

## RF007 — Gestão de estudantes

- **Ator:** Administrador.
- **Descrição:** CRUD de estudantes com vínculo obrigatório a orientador principal e a um ou mais projetos.
- **Regras:** RN01, RN04; criação transacional; conclusão encerra orientações/vínculos; reativação exige vínculos; excluir remove histórico mediante confirmação.
- **Critérios:**
  1. Cadastro sem projeto é bloqueado.
  2. Cadastro sem orientador é bloqueado.
  3. Cadastro válido cria orientação principal e vínculos.
  4. Conclusão altera situação e encerra vínculos.
  5. Não é possível excluir o último projeto ativo de um estudante ativo.
- **Prioridade:** E — **HU007**

## RF008 — Gestão de orientações

- **Ator:** Administrador.
- **Descrição:** registrar orientação principal, coorientação interna e encerrar orientações.
- **Regras:** um único orientador principal ativo por estudante; novo principal encerra o anterior; orientador deve ser pesquisador ativo; encerrar a última principal de estudante ativo é bloqueado.
- **Critérios:**
  1. Novo principal mantém histórico.
  2. Duas principais ativas são impedidas pelo banco.
  3. Encerramento inválido exibe mensagem.
  4. Coorientação interna não substitui o principal.
- **Prioridade:** E — **HU008**

## RF009 — Gestão de projetos

- **Ator:** Administrador/Coordenador.
- **Descrição:** CRUD de projetos com título, sigla, resumo, descrição, linha, situação, datas, coordenador, imagem e destaque.
- **Regras:** um único coordenador; coordenador deve ser pesquisador; projeto concluído exige data fim; coordenador só edita os próprios; exclusão bloqueada com vínculos.
- **Critérios:**
  1. Projeto aparece no portfólio público.
  2. Coordenação é única.
  3. Datas inválidas são rejeitadas.
  4. Exclusão com publicações/financiamentos/equipe é bloqueada.
  5. Coordenador não edita projeto alheio.
- **Prioridade:** E — **HU009**

## RF010 — Gestão de participações de pesquisadores

- **Ator:** Administrador/Coordenador do projeto.
- **Descrição:** adicionar, encerrar e remover pesquisadores participantes.
- **Regras:** coordenador não é duplicado; vínculo único por projeto/pesquisador; data de entrada não futura e não anterior ao projeto; projeto concluído não recebe novos participantes.
- **Critérios:**
  1. Participante aparece no detalhe público.
  2. Coordenador é bloqueado como participante.
  3. Duplicidade é rejeitada.
  4. Encerramento mantém histórico.
- **Prioridade:** E — **HU010**

## RF011 — Vínculo de estudantes com projetos

- **Ator:** Administrador/Coordenador do projeto.
- **Descrição:** vincular e desvincular estudantes de projetos.
- **Regras:** RN01; apenas estudantes ativos; um estudante pode ter vários projetos; último projeto ativo não pode ser removido.
- **Critérios:**
  1. Vínculo aparece no perfil e no projeto.
  2. Duplicidade é bloqueada.
  3. Remoção do último vínculo ativo é bloqueada.
- **Prioridade:** E — **HU011**

## RF012 — Gestão de publicações e autoria

- **Ator:** Administrador/Coordenador.
- **Descrição:** CRUD de publicações com título, veículo, ano, tipo, DOI/link, projeto e autores.
- **Regras:** ao menos um autor pesquisador; DOI ou link obrigatório; autores únicos; publicação pode existir sem projeto; exclusão remove autores/destaques por cascata.
- **Critérios:**
  1. Publicação aparece no perfil dos autores e na página pública.
  2. Publicação sem autor é rejeitada.
  3. Publicação sem DOI/link é rejeitada.
  4. Filtros por ano/pesquisador/projeto funcionam.
- **Prioridade:** E — **HU012**

## RF013 — Gestão de destaques de publicações

- **Ator:** Administrador/Coordenador do próprio perfil.
- **Descrição:** escolher até 5 publicações do próprio pesquisador e ordená-las de 1 a 5.
- **Regras:** máximo 5; somente autoria própria; posição única.
- **Critérios:**
  1. Sexto destaque é bloqueado.
  2. Publicação de outro autor é bloqueada.
  3. Posição ocupada é bloqueada.
  4. Perfil público exibe os destaques na ordem.
- **Prioridade:** E — **HU013**

## RF014 — Gestão de fontes financiadoras

- **Ator:** Administrador.
- **Descrição:** CRUD de fontes (CNPq, CAPES, FAPEMIG, parcerias etc.).
- **Regras:** nome único; tipo válido; fonte em uso não pode ser excluída.
- **Critérios:**
  1. Fonte aparece nos formulários de financiamento.
  2. Exclusão com financiamentos é bloqueada.
- **Prioridade:** I — **HU014**

## RF015 — Gestão de financiamentos e cálculo de verbas

- **Ator:** Administrador/Coordenador do projeto.
- **Descrição:** CRUD de financiamentos por projeto/fonte, com valor, processo e período.
- **Regras:** valor > 0; moeda BRL; datas coerentes; processo único por projeto/fonte; totais sem duplicidade.
- **Critérios:**
  1. Total do projeto = soma dos registros.
  2. Total geral = soma de todos os registros.
  3. Total por pesquisador = projetos distintos (coordenação/participação).
  4. Valor negativo/zero é rejeitado.
  5. Processo duplicado é rejeitado.
- **Prioridade:** E — **HU015**

## RF016 — Página pública de projetos

- **Ator:** Visitante.
- **Descrição:** listar e detalhar projetos com filtros por situação, linha e texto.
- **Critérios:**
  1. Cards mostram título, situação, linha, coordenador, equipe, período e verba.
  2. Detalhe mostra descrição, participantes, estudantes, publicações e financiamentos.
  3. Filtros e estados vazios funcionam.
- **Prioridade:** E — **HU009**

## RF017 — Página pública de publicações

- **Ator:** Visitante.
- **Descrição:** listar publicações com filtros por ano, pesquisador, projeto e texto; paginação.
- **Critérios:**
  1. Lista mostra título, autores, veículo, ano e acesso.
  2. Filtros combinados funcionam.
  3. Paginação mantém filtros.
  4. DOI gera link quando não há link explícito.
- **Prioridade:** E — **HU012**

## RF018 — Painel administrativo com indicadores

- **Ator:** Usuários autenticados.
- **Descrição:** dashboard com contadores reais, projetos recentes, alertas, auditoria e diagnóstico de Web Forms.
- **Critérios:**
  1. Indicadores correspondem ao banco.
  2. Alertas de integridade são exibidos.
  3. Diagnóstico mostra `IsPostBack`, ViewState, Session e eventos.
  4. Ações rápidas levam aos módulos corretos.
- **Prioridade:** E — **HU016**

## RF019 — Relatórios administrativos

- **Ator:** Usuários autenticados.
- **Descrição:** relatórios de projetos por situação, financiamentos por projeto/fonte/pesquisador, publicações por ano/pesquisador, orientandos atuais/passados e alertas.
- **Critérios:**
  1. Valores financeiros conferem com as views.
  2. Percentuais de situação somam 100%.
  3. Relatório pode ser impresso.
  4. Semântica dos totais é explicada na tela.
- **Prioridade:** E — **HU016**

## RF020 — Auditoria de ações

- **Ator:** Administrador.
- **Descrição:** registrar login, falhas, criação, atualização, exclusão e encerramentos.
- **Critérios:**
  1. Ações administrativas geram registro com usuário, ação, entidade e data.
  2. Exclusão de usuário preserva o log com usuário nulo.
  3. Dashboard exibe os registros recentes.
- **Prioridade:** I — **HU016**

## RF021 — Tratamento de erros e mensagens

- **Ator:** Sistema.
- **Descrição:** traduzir erros de banco/negócio em mensagens compreensíveis, sem expor stack trace.
- **Critérios:**
  1. Violação de FK retorna mensagem sobre vínculos.
  2. Duplicidade retorna mensagem sobre dados únicos.
  3. Erros inesperados são logados e exibem página amigável.
  4. Erros de validação aparecem no formulário.
- **Prioridade:** E — **transversal**

## RF022 — Página institucional

- **Ator:** Visitante.
- **Descrição:** apresentar história, missão, linhas, números, arquitetura do portal e contato.
- **Critérios:** conteúdo consistente com o restante do portal e links de contato funcionais.
- **Prioridade:** I — **HU003**

## RF023 — Alteração de senha

- **Ator:** Usuário autenticado.
- **Descrição:** trocar a própria senha informando a atual.
- **Critérios:** senha atual válida; nova senha forte; confirmação coincidente; novo hash persistido.
- **Prioridade:** I — **HU001**

## RF024 — Demonstração de ciclo de vida e estado

- **Ator:** Avaliador/Administrador.
- **Descrição:** o dashboard deve demonstrar `Page_Init`, `Page_Load`, `Page_PreRender`, `IsPostBack`, ViewState e Session.
- **Critérios:**
  1. Botão de postback incrementa contador em ViewState.
  2. Lista de eventos em Session é exibida.
  3. Estado de autenticação e itens de Session são visíveis.
- **Prioridade:** E (rubrica) — **HU016**

## RF025 — Carga inicial de demonstração

- **Ator:** Sistema/avaliador.
- **Descrição:** scripts devem popular o banco com dados coerentes para demonstração.
- **Critérios:** 6 pesquisadores, 13 estudantes, 6 projetos, 14 publicações, 12 financiamentos e 3 usuários.
- **Prioridade:** E — **transversal**

---

## Matriz de rastreabilidade resumida

| Requisito | Entidade/tabela principal | Serviço | Página | Teste |
|---|---|---|---|---|
| RF001 | Usuario | AuthService | Login | AutenticacaoTests |
| RF002 | Usuario/Perfil | BasePage/AdminPageBase | Admin/* | AutenticacaoTests + smoke |
| RF003 | Pessoa/Pesquisador | PesquisadorService | Pesquisadores.aspx | Infra + smoke |
| RF004 | Orientacao/Destaque | PesquisadorService | Pesquisador.aspx | PesquisadorCrudTests |
| RF005 | Estudante | EstudanteService | Estudantes.aspx | EstudanteRegrasTests |
| RF006 | Pessoa/Pesquisador | PesquisadorService | Admin/Pesquisadores | PesquisadorCrudTests |
| RF007 | Estudante/Orientacao/EstudanteProjeto | EstudanteService | Admin/Estudantes | EstudanteRegrasTests |
| RF008 | Orientacao | EstudanteService | Admin/Orientacoes | EstudanteRegrasTests |
| RF009 | Projeto | ProjetoService | Admin/Projetos | ProjetoRegrasTests |
| RF010 | ParticipacaoProjeto | ProjetoService | Admin/Participacoes | ProjetoRegrasTests |
| RF011 | EstudanteProjeto | ProjetoService | Admin/Participacoes | EstudanteRegrasTests |
| RF012 | Publicacao/PublicacaoAutor | PublicacaoService | Admin/Publicacoes | PublicacaoDestaqueTests |
| RF013 | DestaquePublicacao | PublicacaoService | Admin/Destaques | PesquisadorCrudTests |
| RF014 | FonteFinanciadora | FinanciamentoService | Admin/Fontes | FinanciamentoTests |
| RF015 | Financiamento | FinanciamentoService | Admin/Financiamentos | FinanciamentoTests |
| RF016 | Projeto + views | ProjetoService | Projetos/Projeto | smoke |
| RF017 | Publicacao + views | PublicacaoService | Publicacoes | smoke |
| RF018 | views | RelatorioService | Admin/Default | RelatoriosTests |
| RF019 | views | RelatorioService | Admin/Relatorios | RelatoriosTests |
| RF020 | LogAuditoria | UsuarioService | Admin/Default | AutenticacaoTests |
| RF021 | — | SqlErrorTranslator | todas | IntegridadeBancoTests |
| RF022 | — | — | Sobre | smoke |
| RF023 | Usuario | AuthService | AlterarSenha | AutenticacaoTests |
| RF024 | Session/ViewState | BasePage | Admin/Default | smoke |
| RF025 | scripts | AppServices | banco | TestDatabase |
