# 14 — Manual do usuário

## 1. Visão geral

O portal tem duas experiências:

- **Pública:** qualquer visitante consulta pessoas, projetos, publicações e informações institucionais.
- **Restrita:** usuários autenticados gerenciam dados conforme o perfil.

O menu superior está sempre disponível: **Início, Pesquisadores, Estudantes, Projetos, Publicações e O grupo**. O botão **Área restrita** leva ao login.

## 2. Área pública

### 2.1 Início

Apresenta:

- identidade e missão do GPFA;
- indicadores (pesquisadores, estudantes, projetos, publicações e verba);
- projetos em destaque;
- linhas de pesquisa;
- prévia da equipe;
- publicações recentes;
- chamada para colaboração.

Os números são calculados em tempo real do banco.

### 2.2 Pesquisadores

- Use a busca por nome, formação, área ou linha.
- Filtre por área de atuação.
- Cada card mostra foto/iniciais, formação, área, links e contadores.
- Clique no nome ou em **Perfil** para abrir a página completa.

### 2.3 Perfil do pesquisador

A página traz:

1. Cabeçalho com foto, nome, formação, área e links (Scholar, ORCID, Lattes, e-mail).
2. Biografia e linha de pesquisa.
3. **Cinco publicações em destaque**, numeradas.
4. Projetos vinculados.
5. **Orientandos atuais**.
6. **Orientandos egressos** (seção separada).
7. Todas as publicações.
8. Painel lateral com números e projetos coordenados.

> A separação entre orientandos atuais e passados é intencional e atende à regra RN02.

### 2.4 Estudantes

- Busque por nome, curso, orientador ou projeto.
- Filtre por nível, situação e orientador.
- Cada card mostra nível, curso, situação, orientador principal, coorientadores, projetos e ingresso.

### 2.5 Projetos

- Busque por título, sigla, coordenador ou tema.
- Filtre por situação e linha de pesquisa.
- Cada card mostra situação, coordenador, período, equipe e verba.
- **Ver detalhes** abre a página completa:
  - descrição;
  - coordenação;
  - pesquisadores participantes;
  - estudantes;
  - publicações;
  - financiamentos e total.

### 2.6 Publicações

- Filtre por ano, pesquisador, projeto ou texto.
- A lista é paginada (10 por página).
- Clique no título ou em **Acessar** para abrir DOI/link.
- Autores e veículo aparecem em cada item.

### 2.7 O grupo

- História, missão e linhas de pesquisa.
- Números do grupo.
- Explicação da arquitetura do portal.
- Contato institucional e orientação para estudantes.

## 3. Acesso restrito

### 3.1 Login

1. Clique em **Área restrita**.
2. Informe login e senha.
3. Clique em **Entrar no painel**.

Mensagens possíveis:

- “Login ou senha inválidos.” — credenciais incorretas.
- “Este usuário está inativo...” — conta desativada.
- “Conta temporariamente bloqueada...” — após 5 falhas, aguarde 15 minutos.

### 3.2 Sair

Use **Sair** no topo. A sessão e o cookie são encerrados.

### 3.3 Alterar senha

Menu do usuário → **Alterar minha senha** (ou acesse `/AlterarSenha.aspx`). É necessário informar a senha atual. A nova senha deve ter ao menos 8 caracteres, com maiúscula, minúscula e número.

## 4. Painel administrativo

### 4.1 Visão geral

Mostra:

- KPIs de pesquisadores, estudantes, projetos e verba;
- projetos recentes;
- alertas de integridade;
- auditoria recente (admin);
- **Estado do Web Forms**: `IsPostBack`, contador em ViewState, eventos em Session e botão **Executar PostBack**.

Esse painel demonstra o ciclo de vida e o gerenciamento de estado exigidos na disciplina.

### 4.2 Perfis

| Perfil | Pode |
|---|---|
| **Administrador** | Tudo: cadastros, usuários, fontes, projetos, publicações, financiamentos, exclusões e relatórios |
| **Coordenador** | Gerenciar projetos sob sua coordenação, participações, vínculos, publicações, destaques e financiamentos; relatórios |
| **Pesquisador** | Consultar painel e relatórios; alterar a própria senha |

Menus e botões são ocultados conforme o perfil. Acesso direto por URL a uma área proibida resulta em **403**.

### 4.3 Pesquisadores (admin)

1. **Novo pesquisador** → preencha nome, e-mail, formação, área, titulação, departamento, instituição, linha, foto, biografia e links.
2. **Salvar pesquisador**.
3. Use **Editar** na lista para alterar.
4. **Excluir** só funciona se não houver projetos, participações, orientações, publicações ou usuário vinculado.

Boas práticas:

- Use foto quadrada.
- Informe URLs completas (`https://...`).
- Mantenha a mini-biografia curta e objetiva.

### 4.4 Estudantes (admin)

1. **Novo estudante**.
2. Preencha dados, nível, curso, ingresso.
3. Selecione o **orientador principal** (somente pesquisadores ativos).
4. Marque **ao menos um projeto**.
5. Salve.

Regras:

- Sem projeto ou sem orientador → erro.
- O cadastro cria automaticamente a orientação principal e os vínculos.
- Ao definir situação **Concluído**, informe a data; orientações e vínculos ativos são encerrados.
- Para reativar, é necessário redefinir orientador e projeto.
- Vínculos posteriores são gerenciados em **Projetos → Equipe**.

### 4.5 Orientações (admin)

- **Nova orientação**: escolha estudante, orientador, tipo (principal/coorientação interna) e data.
- Definir um novo principal encerra o anterior, preservando o histórico.
- **Encerrar** solicita a data de saída.
- Encerrar a última principal de um estudante ativo é bloqueado.
- **Excluir** também é bloqueado na mesma situação.

Coorientadores externos são cadastrados como texto no formulário do estudante.

### 4.6 Projetos

1. **Novo projeto** (ou **Novo projeto** no topo).
2. Informe título, sigla, resumo, descrição, linha, situação, datas e coordenador.
3. Marque **Exibir na página inicial** para destaque.
4. Salve.

Observações:

- Projetos concluídos exigem data de término.
- Coordenadores só editam seus próprios projetos.
- A exclusão é exclusiva do administrador e bloqueada se houver vínculos.
- Use **Equipe** para gerenciar participantes e estudantes.

### 4.7 Participações e vínculos

Selecione um projeto no topo.

**Pesquisadores:**

- escolha pesquisador, papel e data de entrada;
- **Adicionar participante**;
- **Encerrar** registra a data de saída;
- **Remover** exclui o registro.

**Estudantes:**

- escolha estudante ativo, tipo de vínculo e entrada;
- **Vincular estudante**;
- **Encerrar** ou **Excluir** — o último projeto ativo de um estudante ativo é protegido.

O coordenador não aparece na lista de participantes porque já é participante implícito.

### 4.8 Publicações

1. **Nova publicação**.
2. Informe título, veículo, ano, tipo, DOI e/ou link.
3. Associe um projeto (opcional).
4. Marque um ou mais autores pesquisadores.
5. Salve.

Regras:

- DOI ou link é obrigatório.
- Ao menos um autor é obrigatório.
- A exclusão remove autores e destaques associados.
- Coordenador só gerencia publicações de seus projetos.

### 4.9 Destaques

1. Selecione o pesquisador.
2. Escolha a publicação e uma posição livre (1–5).
3. **Adicionar destaque**.

O sistema bloqueia:

- sexto destaque;
- publicação da qual o pesquisador não é autor;
- posição ocupada.

### 4.10 Financiamentos

1. **Novo financiamento**.
2. Selecione projeto e fonte.
3. Informe descrição, processo, valor e datas.
4. Salve.

- Valor deve ser maior que zero.
- Valores são em BRL.
- Processo não pode repetir na mesma fonte/projeto.
- O total do projeto é atualizado automaticamente.
- Coordenador só gerencia projetos próprios.

### 4.11 Fontes financiadoras (admin)

Cadastre nome, sigla, tipo e site. Fontes em uso não podem ser excluídas.

### 4.12 Usuários e permissões (admin)

1. **Novo usuário**.
2. Informe login, nome, e-mail, perfil e senha.
3. Coordenador/pesquisador exigem vínculo com um pesquisador.
4. Salve.

Proteções:

- login/e-mail únicos;
- senha forte;
- não é possível excluir o próprio usuário;
- o último administrador ativo não pode ser desativado/excluído.

### 4.13 Relatórios

Disponíveis para usuários autenticados:

- projetos por situação (com percentuais);
- financiamentos por projeto;
- financiamentos por fonte;
- total captado por pesquisador;
- publicações por ano;
- publicações por pesquisador;
- orientandos atuais e passados;
- alertas de integridade.

Use **Imprimir** para gerar uma versão limpa (sem menus).

#### Entendendo os valores

- **Total do projeto:** soma dos financiamentos daquele projeto.
- **Total geral:** soma de todos os financiamentos; cada registro conta uma vez.
- **Total por pesquisador:** soma dos projetos distintos em que atua. Um projeto compartilhado aparece no total de cada pesquisador, por isso a soma entre pessoas pode ser maior que o total geral. Isso **não** é duplicidade nos relatórios: são visões diferentes.

## 5. Mensagens e estados

- **Verde:** operação concluída.
- **Vermelho:** erro de validação, regra de negócio ou integridade.
- **Amarelo:** aviso.
- **Azul:** informação.
- **Estado vazio:** explica a ausência de dados e oferece ação.
- **Confirmação:** toda exclusão pede confirmação.

## 6. Acessibilidade e atalhos

- Use **Tab** para navegar e **Enter/Espaço** para acionar.
- O primeiro **Tab** oferece “Pular para o conteúdo”.
- Formulários têm labels associados e mensagens de erro textuais.
- Tabelas têm cabeçalhos.
- O menu colapsa em telas pequenas; use o botão de menu.
- Contraste e foco foram ajustados para leitura confortável.

## 7. Perguntas frequentes

**Por que não consigo excluir um pesquisador?**
Ele possui vínculos (projetos, orientações, publicações, participações ou usuário). Transfira/remova os vínculos primeiro.

**Por que não consigo encerrar a orientação?**
É a última orientação principal de um estudante ativo. Defina outro principal antes.

**Por que o total por pesquisador é maior que o total geral?**
Porque projetos compartilhados aparecem no total de cada pesquisador. Veja a explicação na tela de relatórios.

**Posso ter mais de cinco destaques?**
Não. O limite é 5 por pesquisador (RN05).

**Como incluir um coorientador externo?**
No cadastro do estudante, campo “Coorientadores externos”. Não é necessário cadastrá-lo como pesquisador.

**Um projeto pode ter dois coordenadores?**
Não. A regra RN03 exige um único coordenador responsável. Os demais são participantes.

**Esqueci a senha.**
Nesta versão, o administrador pode redefinir a senha em **Usuários**. Não há autoatendimento por e-mail.
