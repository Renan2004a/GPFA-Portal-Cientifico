# 05 — Regras de negócio

As regras RN01–RN07 vêm diretamente da carta da coordenação e são o coração do sistema. Cada regra é aplicada em **mais de uma camada** (interface, serviço e banco) para não depender de um único ponto de falha.

---

## RN01 — Vínculo obrigatório dos estudantes

> Nenhum estudante de graduação ou pós-graduação pode ficar solto no laboratório. Todo estudante precisa estar obrigatoriamente vinculado a um ou mais projetos de pesquisa do grupo.

### Implementação

1. **Interface:** o formulário de estudante exibe a lista de projetos e o `CustomValidator` exige ao menos uma seleção.
2. **Serviço:** `EstudanteService.Criar` rejeita lista vazia e grava, em uma única transação, pessoa → estudante → orientação → vínculos com projetos.
3. **Banco:** `EstudanteProjeto` é N:N com `UNIQUE (EstudanteId, ProjetoId)`; o trigger `TRG_EstudanteProjeto_ProtegerVinculo` impede remover ou encerrar o último vínculo ativo de estudante ativo.
4. **Monitoramento:** `vw_AlertasIntegridade` lista eventuais estudantes ativos sem projeto.

### Cenários testados

- Cadastro sem projeto → `ValidacaoException`.
- Cadastro válido → `QtdProjetosAtivos = 1`.
- Remoção do último vínculo → `IntegridadeDadosException`.
- Estudante com dois projetos pode encerrar um.

### Decisão

A obrigatoriedade vale para **estudantes ativos**. Egressos e desligados preservam o histórico e não precisam de vínculo ativo.

---

## RN02 — Vínculo dos orientadores e separação atual/passado

> Todos os estudantes orientados por um pesquisador devem aparecer em sua página individual. É necessário separar orientandos atuais e passados.

### Implementação

- Tabela `Orientacao` com `DataInicio`, `DataFim` e `Principal`.
- `vw_Orientandos` classifica:
  - **Atual:** `DataFim IS NULL` **e** estudante `Ativo`;
  - **Passado:** `DataFim IS NOT NULL` **ou** estudante não ativo.
- O perfil público (`Pesquisador.aspx`) exibe duas seções distintas, com contadores próprios.
- `vw_OrientandosPorOrientador` alimenta o relatório.
- Teste garante que um mesmo estudante não aparece nas duas listas.

### Decisão

O histórico é preservado: trocar de orientador encerra a orientação anterior, não a apaga.

---

## RN03 — Coordenação única dos projetos

> Um projeto pode ter vários pesquisadores participantes, mas **apenas um** professor/pesquisador coordenador responsável perante a instituição.

### Implementação

- `Projeto.CoordenadorId` é uma FK **única por projeto** — não existe tabela de “coordenadores”.
- O coordenador é **participante implícito**. A tabela `ParticipacaoProjeto` guarda apenas os demais participantes.
- Trigger `TRG_Participacao_Validar` bloqueia inserir o coordenador como participante.
- Serviço valida que o coordenador é pesquisador cadastrado.
- `vw_ProjetoResumo` calcula `QtdPesquisadores = 1 + participações ativas`.

### Por que não uma tabela de coordenação?

Uma tabela `CoordenacaoProjeto` permitiria múltiplos registros e exigiria uma restrição adicional para garantir exatamente um. A FK única no projeto é mais simples, expressa a cardinalidade 1:1 e impede a inconsistência na origem.

---

## RN04 — Hierarquia de orientação

> Todo estudante precisa de um orientador principal cadastrado. Pode ter coorientadores externos ou de outros departamentos, que não precisam ser pesquisadores do sistema.

### Implementação

- `Orientacao.Principal` + índice único filtrado:

```sql
CREATE UNIQUE INDEX UX_Orientacao_PrincipalAtiva
ON dbo.Orientacao (EstudanteId)
WHERE Principal = 1 AND DataFim IS NULL;
```

- O serviço exige orientador **ativo** e cadastrado.
- Novo principal encerra o anterior na mesma transação.
- Trigger `TRG_Orientacao_ProtegerPrincipal` impede excluir a última principal ativa de estudante ativo.
- Encerramento por `UPDATE` é bloqueado no serviço antes da atualização (o trigger statement-level não conseguiria distinguir a troca em duas instruções).
- Coorientadores **externos**: campo texto `Estudante.CoorientadoresExternos`.
- Coorientadores **internos**: `Orientacao` com `Principal=0`, sem limite de um único.

### Decisão

Exigir que coorientadores externos fossem cadastrados criaria pessoas fictícias sem perfil público. O campo texto atende à flexibilidade pedida pela coordenação.

---

## RN05 — Publicações científicas e cinco destaques

> Cada pesquisador deve ter uma seção de publicações. O sistema deve destacar os cinco artigos que considera mais importantes, com título, veículo, ano e DOI/link.

### Implementação

- `Publicacao` + `PublicacaoAutor` (N:N com ordem de autoria).
- `DestaquePublicacao` com `UNIQUE (PesquisadorId, PublicacaoId)` e `UNIQUE (PesquisadorId, Ordem)`.
- `CHECK (Ordem BETWEEN 1 AND 5)`.
- Trigger `TRG_Destaque_Validar`:
  - máximo 5 por pesquisador;
  - somente publicações em que o pesquisador é autor.
- Interface administrativa mostra as 5 posições e bloqueia posições ocupadas.
- Perfil público exibe os destaques numerados.

### Decisão

A carta diz “exatamente 5”. O sistema permite **até** 5, pois um pesquisador pode ter menos de cinco publicações elegíveis. Quando possui cinco, o sexto é bloqueado — que é o comportamento relevante para a regra.

---

## RN06 — Financiamentos e cálculo de verbas

> Cada projeto pode ter um ou mais registros de financiamento. O sistema deve registrar o valor captado por projeto e apresentar o total por projeto e por pesquisador, sem duplicidade.

### Modelo

```text
Projeto 1 ─── N Financiamento N ─── 1 FonteFinanciadora
```

- `CHECK (Valor > 0)`.
- `CHECK (Moeda = 'BRL')`.
- `UNIQUE (ProjetoId, FonteId, NumeroProcesso)` quando o processo é informado.
- Datas validadas (`DataFim >= DataInicio`).

### Semântica dos totais

| Total | Consulta | Significado |
|---|---|---|
| **Por projeto** | `SUM(Valor) WHERE ProjetoId = @id` | Soma exata dos registros daquele projeto |
| **Geral** | `SUM(Valor)` em `Financiamento` | Cada registro conta uma vez |
| **Por fonte** | `vw_FinanciamentoPorFonte` | Soma por fonte, cada registro uma vez |
| **Por pesquisador** | `vw_FinanciamentoPorPesquisador` | Soma dos totais de **projetos distintos** em que coordena ou participa |

### Por que a soma por pesquisador não é igual ao total geral?

Um financiamento pertence ao **projeto**, não à pessoa. Se Helena e Rafael participam do mesmo projeto de R$ 100.000, o projeto tem R$ 100.000; o total de Helena inclui R$ 100.000 e o de Rafael também. Somar os dois totais daria R$ 200.000, mas o recurso é o mesmo. Por isso:

- o total geral é calculado **uma vez por registro**;
- o total por pesquisador é uma visão de **acesso a recursos**, não uma partição do orçamento;
- a tela de relatórios explica essa semântica.

Dentro do total de um pesquisador, cada projeto é somado uma única vez, mesmo que ele tenha múltiplas formas de participação (coordenação + participação são mutuamente exclusivas por trigger).

### Testes

- Projeto com dois financiamentos soma corretamente.
- Exclusão de um registro reduz o total.
- Fonte em uso não pode ser excluída.
- Processo duplicado é rejeitado.
- `vw_FinanciamentoPorProjeto` confere com `SUM` direto.
- Soma das fontes confere com o total geral.
- Total de Helena no seed = R$ 570.000.

---

## RN07 — Integridade dos dados

> Não permitir exclusões ou alterações que provoquem inconsistências nos relacionamentos.

### Implementação

| Mecanismo | Exemplo |
|---|---|
| FK `NO ACTION` | Excluir pesquisador com projeto, orientação ou publicação é bloqueado |
| `UNIQUE` | e-mail, login, vínculo projeto/pesquisador, projeto/estudante, destaque/posição |
| `CHECK` | valor > 0, situação válida, nível válido, datas coerentes, ordem 1–5 |
| Índice filtrado | uma principal ativa por estudante |
| Trigger | último projeto, última orientação, coordenador duplicado, limite de destaques, tipo de pessoa |
| Serviço | pré-validação com mensagens específicas antes de chegar ao banco |
| Tradutor | `SqlException` → `IntegridadeDadosException` com texto compreensível |
| View de alertas | detecção de inconsistências residuais |

### Exclusões bloqueadas

- Pesquisador com projeto coordenado, participação, orientação, publicação ou usuário.
- Projeto com publicação, financiamento, participação ou estudante.
- Fonte com financiamento.
- Último administrador ativo.
- Estudante ativo sem último projeto/orientação.
- Usuário autenticado excluindo a si mesmo.

### Exclusões permitidas com cascata controlada

- Publicação → autores e destaques (composição).
- Estudante → orientações e vínculos (após desativação e confirmação), pois são partes do agregado.

---

## Resumo de implementação

| Regra | Banco | Serviço | Interface | Teste |
|---|---|---|---|---|
| RN01 | FK, UNIQUE, trigger | transação + validação | CustomValidator + CheckBoxList | EstudanteRegrasTests |
| RN02 | `DataFim`, views | consultas separadas | duas seções | PesquisadorCrudTests |
| RN03 | FK única, trigger | validação de coordenador | dropdown único | ProjetoRegrasTests |
| RN04 | índice filtrado, trigger | troca transacional | tipo principal/coorientação | EstudanteRegrasTests |
| RN05 | UNIQUE, CHECK, trigger | limite/autoria | posições 1–5 | PesquisadorCrudTests |
| RN06 | CHECK, UNIQUE, views | soma por projeto | formulário e relatórios | FinanciamentoTests |
| RN07 | FKs, CHECKs, triggers | pré-validações | mensagens | IntegridadeBancoTests |
