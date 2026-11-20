# 15 — Histórico de decisões

Registro no formato ADR simplificado: contexto, decisão, alternativas e consequências. Decisões marcadas com “implementado” possuem evidência no código/documentação.

---

## D-01 — Usar ASP.NET Web Forms com .NET Framework 4.8.1

- **Contexto:** a disciplina exige Web Forms; o ambiente possui Visual Studio 2026 e referências do .NET Framework 4.8.1.
- **Decisão:** criar uma Web Application clássica (`GPFA.Portal`) em `v4.8.1`, C# 7.3, IIS Express e SQL Server LocalDB, mantendo as bibliotecas em SDK-style compatíveis com `net481`.
- **Alternativas:** ASP.NET Core MVC/Razor (não atende à exigência), Web Site Project (menos controle de build), .NET 10 (incompatível com Web Forms).
- **Consequência:** o site é compilado pelo MSBuild do Visual Studio; o SDK `dotnet` é usado para bibliotecas e testes. Documentado no manual.
- **Status:** implementado e validado.

## D-02 — Reorganizar o trabalho para três integrantes

- **Contexto:** o PDF menciona equipe de 5 pessoas; a equipe real tem 3.
- **Decisão:** consolidar as responsabilidades em três frentes: arquitetura/requisitos/backend/banco (Renan), interface/páginas públicas (Vanessa), banco/CRUD/financiamentos/relatórios (Vanusa), com revisão cruzada.
- **Consequência:** o escopo foi priorizado para entregar os oito critérios da rubrica sem reduzir a qualidade. A diferença está registrada no README e neste documento.
- **Status:** decisão organizacional.

## D-03 — Arquitetura em camadas com projetos separados

- **Contexto:** a rubrica avalia separação de responsabilidades.
- **Decisão:** `Models`, `Data`, `Business`, `Portal` e `Tests`; dependências unidirecionais; `AppServices` como composição.
- **Alternativas:** projeto único (simples, mas frágil), MVC (fora do escopo), DDD completo (excessivo).
- **Consequência:** mais arquivos e configuração, porém testabilidade, legibilidade e aderência à rubrica.
- **Status:** implementado.

## D-04 — Coordenador como FK única no projeto

- **Contexto:** RN03 exige um único coordenador.
- **Decisão:** `Projeto.CoordenadorId` referencia `Pesquisador`; coordenador é participante implícito.
- **Alternativas:** tabela `CoordenacaoProjeto` com múltiplos registros (exigiria restrição extra), campo texto (sem integridade).
- **Consequência:** cardinalidade 1:1 garantida na origem; participantes não duplicam o coordenador.
- **Status:** implementado com trigger.

## D-05 — Orientação como histórico, com uma principal ativa

- **Contexto:** RN02 exige atuais e passados; RN04 exige principal.
- **Decisão:** `Orientacao` com `Principal`, `DataInicio`, `DataFim`; índice único filtrado para principal ativa; troca transacional.
- **Alternativas:** campo `OrientadorAtualId` no estudante (perde histórico), tabela separada de histórico (duplicação).
- **Consequência:** histórico completo; consultas de atuais/passados claras; trigger em DELETE + validação de encerramento no serviço.
- **Status:** implementado e testado.

## D-06 — Coorientadores externos em texto livre

- **Contexto:** a carta permite coorientadores externos sem cadastro.
- **Decisão:** `Estudante.CoorientadoresExternos` (NVARCHAR 500); coorientadores internos em `Orientacao` com `Principal=0`.
- **Alternativas:** criar pessoa externa (poluiria o diretório), tabela de coorientadores com nome (mais estrutura sem necessidade).
- **Consequência:** atende à regra sem inventar cadastros; não há link de perfil para externos.
- **Status:** implementado.

## D-07 — Sem login de estudante nesta versão

- **Contexto:** o PDF cita estudantes como integrantes, mas não define funcionalidades autenticadas exclusivas.
- **Decisão:** estudantes têm perfil público; gestão é feita por admin/coordenador. O perfil “Pesquisador” tem acesso de leitura.
- **Alternativas:** criar login de estudante sem funções (risco de segurança), permitir edição do próprio perfil (escopo extra).
- **Consequência:** menor superfície de ataque e escopo controlado. Evolução possível sem mudar o modelo (basta criar `Usuario` vinculado ao estudante).
- **Status:** decisão de escopo, documentada.

## D-08 — “Até cinco” destaques em vez de “exatamente cinco”

- **Contexto:** a carta diz “exatamente os 5 artigos”, mas um pesquisador pode ter menos de cinco publicações elegíveis.
- **Decisão:** permitir 1–5; bloquear o sexto.
- **Alternativas:** exigir 5 (impediria perfis com menos produção), permitir mais (violaria a regra).
- **Consequência:** cumpre o limite e mantém usabilidade.
- **Status:** implementado com CHECK + trigger + UI.

## D-09 — Financiamento pertence ao projeto; totais por pesquisador são visão derivada

- **Contexto:** RN06 pede total por projeto e por pesquisador sem duplicidade.
- **Decisão:** `Financiamento` ligado a `Projeto` e `Fonte`; total por projeto soma registros; total por pesquisador soma projetos distintos em que atua.
- **Alternativas:** vincular valor diretamente a pessoas (duplicaria recurso compartilhado), ratear valores (não há regra de rateio).
- **Consequência:** total geral não é a soma dos totais por pesquisador; a tela de relatórios explica. Testes garantem que cada projeto entra uma vez por pesquisador.
- **Status:** implementado e testado.

## D-10 — Três perfis com escopo no serviço

- **Contexto:** perfis diferentes e risco de acesso indevido.
- **Decisão:** Administrador, Coordenador e Pesquisador. A UI esconde menus, mas a autorização real ocorre em `AdminPageBase` e nos serviços.
- **Alternativas:** confiar apenas na UI (inseguro), permissões por registro (complexidade excessiva).
- **Consequência:** coordenador só altera projetos próprios; tentativas diretas por URL geram 403.
- **Status:** implementado e testado.

## D-11 — Assets e fontes vendorizados

- **Contexto:** o avaliador pode estar offline; CDNs externas quebrariam o layout.
- **Decisão:** Bootstrap 5.3, fontes Inter/Source Serif 4 e avatares SVG armazenados no projeto; ícones inline.
- **Alternativas:** CDN (simples, dependente de rede), ícones de fonte externa (mais requisições).
- **Consequência:** repositório maior, porém execução determinística.
- **Status:** implementado.

## D-12 — LocalDB + scripts idempotentes

- **Contexto:** ambiente acadêmico sem servidor SQL dedicado.
- **Decisão:** LocalDB `MSSQLLocalDB`, scripts SQL versionados e `Setup-Database.ps1` com `-Recreate`.
- **Alternativas:** arquivo MDF versionado (binário, ruim para Git), SQL Server completo (nem sempre disponível).
- **Consequência:** instalação simples e reproduzível; produção pode usar SQL Server completo com a mesma schema.
- **Status:** implementado.

## D-13 — Testes de integração contra o schema real

- **Contexto:** mocked tests não validariam triggers, CHECKs e índices filtrados.
- **Decisão:** criar `GPFA_Portal_Test`, executar os mesmos scripts e rodar 83 testes de serviços e SQL.
- **Alternativas:** testes unitários puros (não cobrem banco), testes manuais (não reproduzíveis).
- **Consequência:** confiança na integridade; execução recria o banco (não toca o de demonstração).
- **Status:** implementado; 83/83.

## D-14 — Sem upload de imagens

- **Contexto:** o escopo pede foto, mas não define armazenamento de arquivos.
- **Decisão:** referenciar URL ou caminho local; avatares SVG são gerados por script.
- **Alternativas:** upload com armazenamento em disco (risco de path traversal, limpeza, backup) ou banco (pesado).
- **Consequência:** simplicidade e segurança; o admin pode usar uma URL ou colocar o arquivo em `Content/img`.
- **Status:** decisão de escopo, documentada.

## D-15 — Git/GitHub com branches e PR

- **Contexto:** a disciplina exige versionamento real e colaboração.
- **Decisão:** repositório privado `GPFA-Portal-Cientifico`, branches `main`/`develop`/`feature/*`, Conventional Commits, issues, milestones e PR de entrega.
- **Repositório:** https://github.com/Renan2004a/GPFA-Portal-Cientifico
- **Convites:** enviados para `vanessarodrigues5307` e `Vanusa-Oliveira` com permissão **write** em 20/11/2026. O GitHub registra as solicitações como **pendentes** até que cada colaboradora aceite. Não se declara aceite sem confirmação da plataforma.
- **Autoria:** os commits desta máquina são de Renan Alves Tavares (única conta autenticada no ambiente). Não foram fabricados commits em nome das colegas; a contribuição delas ocorrerá via branches/PRs após aceite.
- **Status:** repositório criado e publicado; branches `main`, `develop` e `feature/*` enviadas; PR #13 (`develop` → `main`) revisado e integrado. As colaboradoras ainda precisam aceitar os convites e registrar suas contribuições.

## D-16 — Priorização das entregas

- **Contexto:** prazos 02/12 e 11/12.
- **Decisão:** um único projeto evolutivo. Parte 1 prioriza requisitos, OO, Master Pages, navegação e telas; Parte 2 adiciona banco completo, CRUD, financiamentos, relatórios e testes.
- **Consequência:** não há dois projetos independentes; a Parte 2 reutiliza a base da Parte 1.
- **Status:** implementado.

## D-17 — Moeda única BRL

- **Contexto:** a carta cita fontes nacionais e “valor captado”.
- **Decisão:** restringir `Moeda='BRL'` e formatar em pt-BR.
- **Alternativas:** múltiplas moedas com conversão (fora do escopo e sem taxa definida).
- **Consequência:** relatórios comparáveis; valores internacionais deveriam ser convertidos antes do cadastro.
- **Status:** implementado.

## D-18 — Publicação pode existir sem projeto

- **Contexto:** nem toda produção deriva de um projeto cadastrado.
- **Decisão:** `Publicacao.ProjetoId` opcional.
- **Consequência:** publicações institucionais/anteriores ao portal podem ser cadastradas; filtros por projeto simplesmente não as incluem.
- **Status:** implementado.

---

## Limitações e pendências conhecidas

| Item | Situação | Encaminhamento |
|---|---|---|
| Aceite dos convites do GitHub | Pendente (convites enviados) | Vanessa e Vanusa devem aceitar e criar suas branches |
| Commits das colaboradoras | Ainda não existem | Serão feitos após o aceite, em branches próprias, com revisão |
| Testes de UI automatizados | Não implementados | Suíte cobre negócio/banco; fluxos Web Forms foram validados por smoke tests |
| Upload de imagens | Fora do escopo | Usar URL/caminho local |
| Login de estudante | Fora do escopo | Evolução futura |
| Exportação PDF/Excel | Não implementada | Impressão do navegador atende à apresentação |
| Acessibilidade | Lighthouse 0,93 | Melhorias contínuas de contraste e rótulos |
| Dados | Fictícios | Substituir por dados reais antes de uso institucional |

## Próximos passos sugeridos

1. Colaboradoras aceitarem os convites e revisarem os documentos 01–06.
2. Abrir PRs de revisão de interface e de banco, com comentários.
3. Executar a suíte em outra máquina para validar o manual de instalação.
4. Gravar a demonstração da Parte 1 (navegação + ciclo de vida) e da Parte 2 (CRUD + relatórios).
5. Se necessário, evoluir para upload de imagens e autoatendimento de senha.
