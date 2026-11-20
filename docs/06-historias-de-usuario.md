# 06 — Histórias de usuário

Formato: **“Como [ator], quero [funcionalidade], para [benefício].”**

Cada história traz pré-condições, fluxo principal, alternativos, pós-condições, critérios de aceitação, regras, prioridade e requisitos.

---

## HU001 — Autenticação no painel

**Como** integrante da equipe, **quero** entrar com login e senha, **para** acessar as funções administrativas com segurança.

- **Ator:** Administrador, Coordenador ou Pesquisador.
- **Pré-condições:** usuário cadastrado e ativo; banco disponível.
- **Fluxo principal:**
  1. O usuário acessa “Área restrita”.
  2. Informa login e senha.
  3. O sistema valida o hash PBKDF2.
  4. Cria Session e cookie de autenticação.
  5. Redireciona para o painel ou para a URL de retorno.
- **Fluxos alternativos:**
  - Campos vazios → validação no cliente e servidor.
  - Credenciais inválidas → mensagem genérica; incrementa tentativas.
  - 5 falhas → bloqueio de 15 minutos.
  - Usuário inativo → mensagem específica.
- **Pós-condições:** sessão ativa; último acesso atualizado.
- **Critérios:** login válido funciona; inválido não revela se o login existe; bloqueio funciona; “Sair” encerra.
- **Regras:** RNF001–RNF003. **Prioridade:** Essencial. **RF:** RF001, RF023.

## HU002 — Restrição de acesso por perfil

**Como** administrador, **quero** que cada perfil veja apenas o que lhe compete, **para** proteger dados e evitar alterações indevidas.

- **Pré-condições:** usuário autenticado.
- **Fluxo principal:** o sistema lê `PerfilId` da Session; monta o menu; valida a página; serviços verificam o escopo do projeto.
- **Alternativos:** acesso direto por URL → 403; coordenador tentando projeto alheio → exceção de regra.
- **Pós-condição:** nenhuma alteração indevida é persistida.
- **Critérios:** anônimo → login; coordenador → só próprios projetos; pesquisador → leitura; admin → total.
- **Regras:** RNF004. **Prioridade:** Essencial. **RF:** RF002.

## HU003 — Explorar o portal público

**Como** visitante, **quero** conhecer o grupo, seus números e suas linhas de pesquisa, **para** entender quem somos e o que fazemos.

- **Fluxo:** acessa a página inicial → vê hero, indicadores, projetos em destaque, linhas, equipe, publicações e chamada de contato.
- **Alternativos:** banco vazio → estados vazios.
- **Critérios:** indicadores reais; links funcionais; layout responsivo; navegação para todas as áreas.
- **Prioridade:** Essencial. **RF:** RF003, RF016, RF022.

## HU004 — Consultar perfil de pesquisador

**Como** visitante, **quero** ver a trajetória completa de um pesquisador, **para** avaliar sua atuação e encontrar seus melhores trabalhos.

- **Fluxo:** diretório → card → perfil → biografia, links, projetos, coordenações, orientandos atuais/passados, publicações e 5 destaques.
- **Alternativos:** perfil inexistente → mensagem amigável; sem destaques → aviso.
- **Critérios:** separação atual/passado; destaques numerados; links externos seguros.
- **Regras:** RN02, RN05. **Prioridade:** Essencial. **RF:** RF004.

## HU005 — Consultar diretório de estudantes

**Como** visitante, **quero** ver os estudantes, seus níveis, cursos, orientadores e projetos, **para** conhecer a formação de recursos humanos do grupo.

- **Fluxo:** acessa diretório → usa filtros → vê cards com todos os dados.
- **Critérios:** filtros combinados; situação visível; coorientadores quando existirem; projetos clicáveis.
- **Regras:** RN01, RN02, RN04. **Prioridade:** Essencial. **RF:** RF005.

## HU006 — Gerenciar pesquisadores

**Como** administrador, **quero** cadastrar, editar e excluir pesquisadores, **para** manter o diretório institucional atualizado.

- **Pré-condições:** admin autenticado.
- **Fluxo:** painel → Pesquisadores → Novo/Editar → preenche → salva → recebe confirmação.
- **Alternativos:** e-mail duplicado, URL inválida, exclusão com vínculos.
- **Critérios:** CRUD funcional; alterações refletem no público; exclusão bloqueada com mensagem específica.
- **Regras:** RN07. **Prioridade:** Essencial. **RF:** RF006.

## HU007 — Gerenciar estudantes com vínculos obrigatórios

**Como** administrador, **quero** cadastrar estudantes exigindo orientador e projeto, **para** garantir que ninguém fique solto no laboratório.

- **Fluxo:** painel → Estudantes → Novo → dados + orientador + ao menos um projeto → salva.
- **Alternativos:** sem projeto → erro; sem orientador → erro; orientador inativo → erro; conclusão → fecha vínculos.
- **Critérios:** criação transacional; vínculo visível no projeto; conclusão coerente; reativação exige vínculos.
- **Regras:** RN01, RN04. **Prioridade:** Essencial. **RF:** RF007.

## HU008 — Gerenciar orientações

**Como** administrador, **quero** registrar trocas de orientador e coorientações, **para** manter o histórico acadêmico correto.

- **Fluxo:** painel → Orientações → escolhe estudante/orientador/tipo/data → salva.
- **Alternativos:** encerrar última principal de ativo → bloqueio; orientador inativo → erro.
- **Critérios:** uma principal ativa; histórico preservado; coorientação não substitui principal.
- **Regras:** RN02, RN04. **Prioridade:** Essencial. **RF:** RF008.

## HU009 — Gerenciar projetos e coordenação

**Como** administrador ou coordenador, **quero** criar e manter projetos com um único coordenador, **para** refletir a responsabilidade institucional.

- **Fluxo:** painel → Projetos → Novo/Editar → dados, coordenador, datas, situação, destaque → salva.
- **Alternativos:** projeto concluído sem data fim → erro; coordenador tenta editar projeto alheio → bloqueio; exclusão com vínculos → bloqueio.
- **Critérios:** um coordenador; portfólio atualizado; exclusão segura; coordenador vê apenas os seus.
- **Regras:** RN03, RN07. **Prioridade:** Essencial. **RF:** RF009, RF016.

## HU010 — Gerenciar participações

**Como** gestor do projeto, **quero** adicionar e encerrar pesquisadores participantes, **para** registrar a equipe real de cada projeto.

- **Fluxo:** Projetos → Equipe → escolhe pesquisador/papel/data → adiciona.
- **Alternativos:** coordenador como participante → bloqueio; duplicidade → bloqueio; data inválida → erro.
- **Critérios:** participante aparece no público; coordenador implícito não duplicado; encerramento preserva histórico.
- **Regras:** RN03. **Prioridade:** Essencial. **RF:** RF010.

## HU011 — Gerenciar vínculos de estudantes com projetos

**Como** gestor do projeto, **quero** vincular estudantes a projetos, **para** manter a pesquisa e a formação conectadas.

- **Fluxo:** Projetos → Equipe → Estudantes → seleciona estudante ativo/tipo/data → vincula.
- **Alternativos:** último projeto ativo → bloqueio; duplicidade → bloqueio.
- **Critérios:** vínculo aparece nos dois lados; remoção do último é impedida.
- **Regras:** RN01. **Prioridade:** Essencial. **RF:** RF011.

## HU012 — Gerenciar publicações

**Como** gestor, **quero** registrar publicações com autores e acesso, **para** divulgar a produção científica.

- **Fluxo:** painel → Publicações → Nova/Editar → título, veículo, ano, tipo, DOI/link, projeto, autores → salva.
- **Alternativos:** sem autor → erro; sem DOI/link → erro; exclusão remove destaques.
- **Critérios:** aparece no perfil dos autores e na página pública; filtros funcionam.
- **Regras:** RN05. **Prioridade:** Essencial. **RF:** RF012, RF017.

## HU013 — Escolher os cinco destaques

**Como** pesquisador (ou admin em seu nome), **quero** escolher até cinco publicações importantes, **para** orientar o visitante sobre minha melhor produção.

- **Fluxo:** Destaques → seleciona pesquisador → publicação → posição → adiciona.
- **Alternativos:** sexto → bloqueio; autoria alheia → bloqueio; posição ocupada → bloqueio.
- **Critérios:** perfil público exibe na ordem; limite e autoria garantidos.
- **Regras:** RN05. **Prioridade:** Essencial. **RF:** RF013.

## HU014 — Gerenciar fontes financiadoras

**Como** administrador, **quero** manter as fontes de financiamento, **para** padronizar os registros e relatórios.

- **Fluxo:** painel → Fontes → Novo/Editar → nome, sigla, tipo, site → salva.
- **Alternativos:** fonte em uso → exclusão bloqueada.
- **Critérios:** fonte disponível nos formulários; totais por fonte corretos.
- **Regras:** RN06, RN07. **Prioridade:** Importante. **RF:** RF014.

## HU015 — Registrar financiamentos e acompanhar verbas

**Como** gestor, **quero** registrar valores por projeto/fonte e ver os totais, **para** prestar contas e planejar a pesquisa.

- **Fluxo:** painel → Financiamentos → Novo/Editar → projeto, fonte, descrição, processo, valor, datas → salva.
- **Alternativos:** valor ≤ 0 → erro; datas inválidas → erro; processo duplicado → erro; projeto alheio (coordenador) → bloqueio.
- **Critérios:** total do projeto correto; total geral correto; total por pesquisador sem duplicar projetos; exclusão atualiza totais.
- **Regras:** RN06, RN07. **Prioridade:** Essencial. **RF:** RF015.

## HU016 — Acompanhar indicadores e relatórios

**Como** gestor, **quero** um painel e relatórios confiáveis, **para** tomar decisões e demonstrar resultados.

- **Fluxo:** painel → dashboard com KPIs, alertas, auditoria e diagnóstico; relatórios com gráficos/tabelas e impressão.
- **Alternativos:** inconsistências → alertas; sem dados → estados vazios.
- **Critérios:** números conferem com o banco; semântica financeira explicada; impressão funcional; diagnóstico demonstra Web Forms.
- **Regras:** RN06, RN07. **Prioridade:** Essencial. **RF:** RF018–RF020, RF024.

---

## Priorização (MoSCoW)

| Prioridade | Histórias |
|---|---|
| **Must** | HU001–HU013, HU015, HU016 |
| **Should** | HU014 |
| **Could** | Login de estudante, upload de imagens, integração com Lattes/ORCID |
| **Won't (nesta versão)** | Aplicativo nativo, multilíngue, e-mail transacional |

## Definição de pronto (DoD)

Uma história é considerada pronta quando:

1. A tela funciona com dados reais do banco.
2. As validações de servidor estão ativas.
3. As regras de negócio associadas estão cobertas por teste.
4. Erros de banco são traduzidos em mensagens compreensíveis.
5. A interface é responsiva e acessível.
6. A documentação foi atualizada.
7. O código está versionado com mensagem descritiva.
