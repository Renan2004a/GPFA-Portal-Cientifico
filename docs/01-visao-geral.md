# 01 — Visão geral do sistema

## 1. Contexto

O Grupo de Pesquisa sobre Formigas Atômicas (GPFA) é um laboratório multidisciplinar fictício da Universidade Federal de Minas Gerais, usado como estudo de caso do projeto final da disciplina de Programação Web. O grupo cresceu e passou a enfrentar um problema crônico de visibilidade e organização: informações sobre integrantes, projetos, publicações e recursos estavam dispersas em planilhas, currículos Lattes desatualizados e páginas isoladas.

A coordenação solicitou um portal web que funcionasse, ao mesmo tempo, como **vitrine científica** para o público externo e como **sistema de gestão** para a equipe interna.

## 2. Problema

| Dimensão | Situação anterior | Consequência |
|---|---|---|
| Pessoas | Dados em planilhas e currículos desatualizados | Dificuldade de apresentar a equipe e a formação de recursos humanos |
| Projetos | Informações pulverizadas | Falta de visão consolidada sobre andamento, equipe e resultados |
| Orientações | Controle informal | Risco de estudantes sem orientador ou sem projeto; confusão entre atuais e egressos |
| Publicações | Listas manuais | Divulgação incompleta e ausência de destaque dos principais artigos |
| Financiamentos | Planilhas paralelas | Risco de valores duplicados e relatórios inconsistentes |
| Transparência | Sem canal institucional único | Baixa visibilidade perante a comunidade e os órgãos de fomento |

## 3. Objetivo geral

Desenvolver um portal web para **centralização, organização, gerenciamento e divulgação** das informações científicas e administrativas do GPFA, utilizando ASP.NET Web Forms, banco relacional SQL Server e arquitetura em camadas.

## 4. Objetivos específicos

1. Cadastrar e divulgar pesquisadores, com formação, área, biografia e links externos.
2. Cadastrar estudantes, garantindo vínculo com projeto e orientador principal.
3. Distinguir orientandos atuais e passados na página de cada pesquisador.
4. Gerenciar projetos com um único coordenador e múltiplos participantes.
5. Registrar publicações, autoria e até cinco destaques por pesquisador.
6. Controlar fontes e registros de financiamento, calculando totais sem duplicidade.
7. Publicar uma vitrine institucional com indicadores reais do banco.
8. Oferecer relatórios administrativos e um painel protegido por autenticação.
9. Aplicar validações, integridade referencial e mensagens compreensíveis.
10. Demonstrar, de forma explícita, o uso de Master Pages, controles de servidor, Data Binding, ciclo de vida e Session.

## 5. Escopo

### Incluído

- Portal público responsivo (início, pesquisadores, estudantes, projetos, publicações, institucional).
- Autenticação e três perfis de acesso.
- CRUD completo dos módulos administrativos.
- Regras de negócio RN01–RN07.
- Cálculo de verbas por projeto, fonte e pesquisador.
- Relatórios e painel com indicadores.
- Scripts SQL, dados de demonstração e testes de integração.
- Documentação acadêmica, diagramas e manual.

### Não incluído nesta versão

- Upload físico de imagens (as imagens são referenciadas por URL/caminho).
- Envio de e-mails transacionais (contato é feito por `mailto:`).
- Integração automática com Lattes, ORCID ou Google Scholar.
- Workflow de aprovação de publicações.
- Aplicativo móvel nativo.
- Multilíngue.

Essas exclusões estão registradas em [`15-historico-decisoes.md`](15-historico-decisoes.md).

## 6. Atores

| Ator | Descrição | Permissões |
|---|---|---|
| **Visitante público** | Qualquer pessoa sem autenticação | Consulta a páginas públicas, filtros e perfis |
| **Pesquisador** | Usuário vinculado a um pesquisador | Painel e relatórios em modo consulta; alteração da própria senha |
| **Coordenador** | Pesquisador responsável por projetos | Gestão dos projetos sob sua coordenação, participações, publicações, destaques e financiamentos; relatórios |
| **Administrador** | Equipe de gestão do portal | Acesso total: cadastros, usuários, fontes, exclusões e relatórios |
| **Sistema/banco** | Mecanismos automáticos | Triggers, constraints, índices e views que garantem integridade |

> Decisão: o mesmo usuário não acumula perfis. A separação por um `PerfilId` único simplifica a autorização e foi suficiente para o escopo acadêmico.

## 7. Diferenciais do projeto

- Regras acadêmicas **verificadas no banco e na camada de negócio**, não apenas na interface.
- Relatórios financeiros com semântica explícita para evitar dupla contagem.
- Interface autoral, responsiva e consistente, sem depender de templates prontos.
- Diagnóstico visível do ciclo de vida do Web Forms e do estado de Session.
- 83 testes de integração executados contra o schema real.
- Scripts de banco reproduzíveis e testáveis.

## 8. Tecnologias e ambiente

- Visual Studio 2026 Community 18.10.
- .NET Framework 4.8.1, C# 7.3.
- ASP.NET Web Forms com IIS Express.
- SQL Server LocalDB 2022 (`MSSQLLocalDB`).
- Bootstrap 5.3, fontes e CSS vendorizados.
- Git/GitHub.
- MSTest para testes.

## 9. Critérios de sucesso

- O portal abre no Visual Studio e executa sem passos manuais ocultos.
- Visitantes navegam por todas as áreas públicas em desktop e mobile.
- Administradores executam CRUD com validações e mensagens claras.
- Operações inválidas são bloqueadas com explicação compreensível.
- Totais financeiros conferem com os registros do banco.
- Os 20 cenários de teste exigidos são cobertos e aprovados.
- A documentação permite que outra pessoa instale, opere e evolua o sistema.

## 10. Rastreabilidade com a rubrica da disciplina

| Critério (25% cada) | Evidência |
|---|---|
| Interpretação e elicitação de requisitos | Documentos 01–06, testes e telas derivadas diretamente do PDF |
| Modelagem orientada a objetos | Documento 07, classes de domínio, serviços e repositórios |
| Domínio do Web Forms e ciclo de vida | Master Pages (inclusive aninhada), `Page_Init/Load/PreRender`, `IsPostBack`, ViewState, Session e página de diagnóstico |
| Interface, usabilidade e responsividade | CSS autoral, cards, layout responsivo, estados vazios, feedback e acessibilidade |
| Banco relacional | Scripts, MER/DER, dicionário, FKs, CHECKs, índices filtrados, triggers e views |
| Separação em camadas | Projetos `Models`, `Data`, `Business`, `Portal` e testes |
| CRUD e tratamento de erros | Páginas administrativas, serviços transacionais e tradução de erros SQL |
| Financiamento e relatórios | `Financiamento`, views de soma, relatórios e testes de não duplicidade |
