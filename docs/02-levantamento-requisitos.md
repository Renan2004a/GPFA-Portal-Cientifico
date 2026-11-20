# 02 — Levantamento e elicitação de requisitos

## 1. Fonte de elicitação

A fonte oficial é o documento **“Projeto Final da disciplina”** (3 páginas), uma carta da coordenação do GPFA à equipe de desenvolvimento. O texto foi lido integralmente, incluindo:

- a narrativa sobre visibilidade e organização;
- a descrição do perfil individual e dos cinco artigos em destaque;
- os projetos e o valor de verba captado;
- as quatro regras acadêmicas rígidas (estudantes, orientadores, coordenação e hierarquia de orientação);
- as rubricas e prazos das Partes 1 e 2.

A rubrica foi usada como **critério de aceitação de alto nível**: cada item avaliado possui uma evidência no sistema e na documentação.

## 2. Técnicas de elicitação aplicadas

| Técnica | Aplicação |
|---|---|
| Análise documental | Leitura da carta e das rubricas |
| Identificação de atores | Derivação dos perfis a partir das responsabilidades descritas |
| Análise de regras | Tradução das quatro regras acadêmicas em restrições e validações |
| Modelagem de domínio | Identificação de entidades, associações e cardinalidades |
| Prototipação incremental | Telas públicas primeiro (Parte 1), seguidas do painel e banco (Parte 2) |
| Verificação por testes | Cada regra crítica possui teste automatizado |

## 3. Partes interessadas

| Parte interessada | Interesse | Influência |
|---|---|---|
| Coordenação do GPFA | Divulgação, consistência e relatórios | Alta |
| Pesquisadores | Perfil atualizado e visibilidade | Alta |
| Estudantes | Vínculo correto, orientação e transparência | Média |
| Público externo | Encontrar pessoas, projetos e publicações | Média |
| Universidade / fomento | Relatórios confiáveis de produção e recursos | Alta |
| Equipe de desenvolvimento | Escopo viável e avaliação pelos critérios | Alta |
| Professor da disciplina | Aderência à rubrica e uso real de Web Forms/SQL Server | Alta |

## 4. Necessidades identificadas

### 4.1 Público externo

- Saber quem são os integrantes, sua formação, área e trajetória.
- Encontrar links para Google Scholar, ORCID e Lattes.
- Conhecer os projetos, seu status e resultados.
- Consultar publicações por ano, autor e projeto.
- Ver os cinco artigos que cada pesquisador considera mais importantes.
- Entender a identidade e as linhas de pesquisa do grupo.

### 4.2 Gestão interna

- Manter um diretório único de pessoas.
- Garantir que estudantes não fiquem “soltos”.
- Separar orientandos atuais de egressos.
- Garantir um único coordenador por projeto.
- Registrar participações e vínculos sem duplicação.
- Controlar publicações, autoria e destaques.
- Registrar financiamentos por projeto/fonte e calcular verbas.
- Produzir relatórios confiáveis.
- Proteger o painel e limitar ações por perfil.

## 5. Atores e decisões de permissão

A carta cita visitantes, administradores, coordenadores, pesquisadores e estudantes. A análise definiu:

| Ator | Acesso ao sistema | Justificativa |
|---|---|---|
| Visitante | Somente leitura pública | Não há cadastro público; reduz superfície de ataque |
| Estudante | Sem login próprio nesta versão | O estudante é representado no perfil público; gestão feita por admin/coordenador. Login próprio ficaria sem funcionalidades exclusivas no escopo atual |
| Pesquisador | Login de consulta | Permite acompanhar indicadores e relatórios sem risco de alteração indevida |
| Coordenador | Login com escopo nos próprios projetos | Atende à gestão descentralizada descrita na carta |
| Administrador | Login total | Necessário para cadastros globais, usuários, fontes e exclusões |

> Limitação assumida: o perfil “Estudante” não possui autenticação nesta entrega. A decisão está registrada em `15-historico-decisoes.md` (D-07) e pode ser evoluída sem alterar o modelo.

## 6. Regras extraídas da carta

| Regra da carta | Tradução no sistema |
|---|---|
| Nenhum estudante solto; vínculo com um ou mais projetos | RN01 — criação transacional e trigger de proteção do último vínculo |
| Orientandos atuais e passados em listas separadas | RN02 — histórico em `Orientacao` e separação por `DataFim`/situação |
| Vários pesquisadores, **um único** coordenador | RN03 — FK única `Projeto.CoordenadorId`; coordenador implícito |
| Orientador principal obrigatório e cadastrado | RN04 — `Orientacao.Principal`, índice filtrado, validação de pesquisador ativo |
| Coorientadores externos podem ser texto livre | Campo `Estudante.CoorientadoresExternos` + coorientação interna opcional |
| Até 5 artigos em destaque por pesquisador | RN05 — tabela de destaques, limite por trigger e interface de posições |
| Verba captada por projeto e por pesquisador | RN06 — `Financiamento` + views; semântica documentada |
| Integridade dos relacionamentos | RN07 — FKs, CHECKs, triggers e tradução de erros |

## 7. Ambiguidades e decisões adotadas

| # | Ambiguidade | Decisão | Justificativa |
|---|---|---|---|
| A1 | Quantos destaques exatamente? | **No máximo 5**, permitindo menos | A carta diz “destacar exatamente os 5”; nem todo perfil pode ter 5 publicações elegíveis. O sistema permite até 5 e exibe os existentes |
| A2 | Coorientadores internos também são pesquisadores? | Sim, podem ser registrados em `Orientacao` com `Principal=0`; externos ficam em texto | Atende à flexibilidade pedida sem inventar cadastro externo |
| A3 | O coordenador conta como participante? | Sim, implicitamente; não é duplicado na tabela de participação | Evita contagem dupla e garante um único coordenador |
| A4 | “Total captado por pesquisador” é aditivo? | Não; é a soma dos projetos distintos em que atua | Um mesmo recurso pode financiar vários pesquisadores; somar entre pessoas duplicaria o valor |
| A5 | Estudantes concluídos precisam de projeto ativo? | Não; a obrigatoriedade vale para estudantes **ativos** | Egressos mantêm histórico, mas não vínculo ativo |
| A6 | Exclusões devem apagar históricos? | Bloqueadas quando há vínculos; o administrador deve removê-los/transferi-los | Preserva integridade e rastreabilidade |
| A7 | Como armazenar senhas? | PBKDF2-HMAC-SHA256 com salt e 120.000 iterações | Nunca texto puro; compatível com .NET Framework |
| A8 | O portal precisa de login de estudante? | Não nesta versão | Não há requisito funcional exclusivo; evita complexidade sem valor avaliativo |
| A9 | Moeda estrangeira? | Somente BRL | A carta fala em “valor captado” e fontes nacionais; misturar moedas distorceria relatórios |
| A10 | Publicação sem projeto? | Permitida | Nem toda produção nasce de um projeto cadastrado |

## 8. Requisitos de interface derivados

- Vitrine visualmente atraente, com identidade institucional e não “formulário acadêmico”.
- Cards de projetos e pessoas com hierarquia clara.
- Separação visual explícita entre orientandos atuais e passados.
- Estados vazios e mensagens de erro/sucesso.
- Layout responsivo para desktop, tablet e celular.
- Navegação pública e administrativa distintas, com menu contextual.
- Acessibilidade: foco visível, contraste, textos alternativos, cabeçalhos ordenados e navegação por teclado.

## 9. Restrições

- Desenvolvimento obrigatório em **ASP.NET Web Forms** e **SQL Server**, não substituíveis por SPA/Node/Python.
- Compatibilidade com a versão instalada do Visual Studio/.NET.
- Prazos: Parte 1 em **02/12/2026**; Parte 2 em **11/12/2026**.
- Equipe real de 3 integrantes, embora o documento cite 5.
- Ambiente local com LocalDB, sem servidor de produção.

## 10. Premissas

- O navegador do avaliador suporta HTML5/CSS3 modernos.
- O usuário executará os scripts SQL antes de abrir o portal.
- As imagens podem ser servidas localmente (avatares SVG gerados) ou por URL.
- O avaliador aceitará dados fictícios para demonstração.

## 11. Riscos e mitigação

| Risco | Impacto | Mitigação |
|---|---|---|
| Ambiente sem .NET Framework/Web Forms | Alto | Documentação de pré-requisitos e verificação no VS 2026; o projeto usa 4.8.1 já instalado |
| Banco não criado | Alto | Script idempotente e mensagens de erro claras |
| Regras implementadas só na UI | Alto | Serviços transacionais + triggers + testes |
| Duplicidade financeira | Alto | Soma por projeto em views e testes específicos |
| Perda de sessão | Médio | Redirecionamento para login; mensagens claras |
| Escopo excessivo | Médio | Priorização Parte 1 → Parte 2 no mesmo projeto |

## 12. Critérios de aceitação do levantamento

- [x] Todas as regras da carta foram mapeadas.
- [x] Atores e permissões foram definidos e justificados.
- [x] Ambiguidades foram registradas com decisão.
- [x] Requisitos funcionais e não funcionais foram derivados.
- [x] Histórias de usuário cobrem todos os módulos.
- [x] Cada critério da rubrica tem evidência rastreável.
