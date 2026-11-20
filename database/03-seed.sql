/* ============================================================================
   GPFA - Script 03 - Dados iniciais para demonstração e testes
   Os nomes, projetos e publicações são fictícios e servem à apresentação
   acadêmica do portal.
   ============================================================================ */
:setvar DbName "GPFA_Portal"
USE [$(DbName)];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

SET IDENTITY_INSERT dbo.PerfilAcesso ON;
INSERT dbo.PerfilAcesso (PerfilId, Nome, Descricao) VALUES
 (1, N'Administrador', N'Acesso total ao painel, usuários, cadastros e relatórios.'),
 (2, N'Coordenador',  N'Gestão dos projetos sob sua coordenação, participações, publicações e financiamentos.'),
 (3, N'Pesquisador',  N'Consulta ao painel e aos relatórios; edição do próprio perfil acadêmico.');
SET IDENTITY_INSERT dbo.PerfilAcesso OFF;
GO

SET IDENTITY_INSERT dbo.Pessoa ON;
INSERT dbo.Pessoa (PessoaId, NomeCompleto, Email, TipoPessoa, FotoUrl, FormacaoAcademica, AreaAtuacao, MiniBiografia, GoogleScholarUrl, OrcidUrl, LattesUrl, Ativo) VALUES
 (1, N'Helena Marques Ferreira', N'helena.ferreira@gpfa.ufmg.br', 'P', N'/Content/img/avatars/helena-marques.svg', N'Doutorado em Física (UFMG) · Pós-doutorado (MIT)', N'Materiais Quânticos', N'Coordena o GPFA e investiga fenômenos de transporte em materiais bidimensionais. Atua na formação de mestres e doutores e na interface entre física fundamental e dispositivos de baixo consumo.', N'https://scholar.google.com/citations?user=gpfa-helena', N'https://orcid.org/0000-0002-1111-2222', N'http://lattes.cnpq.br/1111111111111111', 1),
 (2, N'Rafael Nogueira Lima', N'rafael.lima@gpfa.ufmg.br', 'P', N'/Content/img/avatars/rafael-lima.svg', N'Doutorado em Nanotecnologia (UFMG)', N'Nanofotônica', N'Desenvolve sensores ópticos e dispositivos fotônicos de alta sensibilidade, com ênfase em ressonâncias em microcavidades e aplicações em diagnóstico.', N'https://scholar.google.com/citations?user=gpfa-rafael', N'https://orcid.org/0000-0003-3333-4444', N'http://lattes.cnpq.br/2222222222222222', 1),
 (3, N'Camila Duarte Alves', N'camila.alves@gpfa.ufmg.br', 'P', N'/Content/img/avatars/camila-alves.svg', N'Doutorado em Ciência da Computação (USP)', N'Simulação Computacional', N'Pesquisa métodos numéricos para transporte quântico e aprendizado de máquina aplicado à ciência de materiais. Coordena a infraestrutura computacional do grupo.', N'https://scholar.google.com/citations?user=gpfa-camila', N'https://orcid.org/0000-0001-5555-6666', N'http://lattes.cnpq.br/3333333333333333', 1),
 (4, N'Bruno Teixeira Campos', N'bruno.campos@gpfa.ufmg.br', 'P', N'/Content/img/avatars/bruno-campos.svg', N'Pós-doutorado em Engenharia Ambiental (UFMG)', N'Sensores Ambientais', N'Coordena redes de sensores ambientais de baixo custo para monitoramento de recursos hídricos, integrando hardware aberto e análise de dados.', N'https://scholar.google.com/citations?user=gpfa-bruno', N'https://orcid.org/0000-0004-7777-8888', N'http://lattes.cnpq.br/4444444444444444', 1),
 (5, N'Larissa Prado Mendes', N'larissa.mendes@gpfa.ufmg.br', 'P', N'/Content/img/avatars/larissa-mendes.svg', N'Doutorado em Química (Unicamp)', N'Materiais Funcionais', N'Investiga a síntese e a funcionalização de materiais para remediação ambiental, com foco em processos sustentáveis e economia circular.', N'https://scholar.google.com/citations?user=gpfa-larissa', N'https://orcid.org/0000-0005-9999-0000', N'http://lattes.cnpq.br/5555555555555555', 1),
 (6, N'Gustavo Henrique Rocha', N'gustavo.rocha@gpfa.ufmg.br', 'P', N'/Content/img/avatars/gustavo-rocha.svg', N'Doutorado em Física (UFMG)', N'Espectroscopia Avançada', N'Utiliza técnicas de espectroscopia de alta resolução para caracterizar interfaces atômicas e materiais nanoestruturados.', N'https://scholar.google.com/citations?user=gpfa-gustavo', N'https://orcid.org/0000-0006-1212-1313', N'http://lattes.cnpq.br/6666666666666666', 1),

 (7,  N'Ana Beatriz Souza', N'ana.souza@aluno.gpfa.ufmg.br', 'E', N'/Content/img/avatars/ana-souza.svg', N'Mestrado em Física (em curso)', N'Física da Matéria Condensada', N'Mestranda dedicada ao estudo de heteroestruturas bidimensionais e transporte eletrônico.', NULL, NULL, NULL, 1),
 (8,  N'Lucas Martins Oliveira', N'lucas.oliveira@aluno.gpfa.ufmg.br', 'E', N'/Content/img/avatars/lucas-oliveira.svg', N'Graduação em Engenharia de Materiais (em curso)', N'Materiais e Nanotecnologia', N'Bolsista de iniciação científica em nanofotônica e fabricação de dispositivos.', NULL, NULL, NULL, 1),
 (9,  N'Mariana Costa Ribeiro', N'mariana.ribeiro@aluno.gpfa.ufmg.br', 'E', N'/Content/img/avatars/mariana-ribeiro.svg', N'Doutorado em Nanotecnologia (em curso)', N'Simulação de Nanoestruturas', N'Doutoranda que desenvolve modelos computacionais de transporte quântico.', NULL, NULL, NULL, 1),
 (10, N'Pedro Henrique Almeida', N'pedro.almeida@aluno.gpfa.ufmg.br', 'E', N'/Content/img/avatars/pedro-almeida.svg', N'Mestrado em Ciência da Computação (em curso)', N'Computação Científica', N'Mestrando responsável por pipelines de simulação e visualização de dados científicos.', NULL, NULL, NULL, 1),
 (11, N'Julia Fernandes Rocha', N'julia.rocha@aluno.gpfa.ufmg.br', 'E', N'/Content/img/avatars/julia-rocha.svg', N'Graduação em Física (em curso)', N'Materiais Quânticos', N'Bolsista de iniciação científica em caracterização de materiais bidimensionais.', NULL, NULL, NULL, 1),
 (12, N'Thiago Barbosa Nunes', N'thiago.nunes@aluno.gpfa.ufmg.br', 'E', N'/Content/img/avatars/thiago-nunes.svg', N'Curso Técnico em Química (em curso)', N'Química Ambiental', N'Apoia a operação de sensores ambientais e a análise de amostras de água.', NULL, NULL, NULL, 1),
 (13, N'Beatriz Almeida Castro', N'beatriz.castro@aluno.gpfa.ufmg.br', 'E', N'/Content/img/avatars/beatriz-castro.svg', N'Doutorado em Química (em curso)', N'Materiais Funcionais', N'Doutoranda em síntese de materiais funcionalizados para remediação de água.', NULL, NULL, NULL, 1),
 (14, N'Gabriel Rocha Pires', N'gabriel.pires@aluno.gpfa.ufmg.br', 'E', N'/Content/img/avatars/gabriel-pires.svg', N'Mestrado em Engenharia Elétrica (em curso)', N'Instrumentação Eletrônica', N'Mestrando em instrumentação de baixo consumo para redes de sensores.', NULL, NULL, NULL, 1),
 (15, N'Sofia Andrade Melo', N'sofia.melo@aluno.gpfa.ufmg.br', 'E', N'/Content/img/avatars/sofia-melo.svg', N'Graduação em Engenharia Ambiental (em curso)', N'Monitoramento Ambiental', N'Bolsista de iniciação científica em monitoramento participativo de rios.', NULL, NULL, NULL, 1),
 (16, N'Henrique Silva Tavares', N'henrique.tavares@aluno.gpfa.ufmg.br', 'E', N'/Content/img/avatars/henrique-tavares.svg', N'Mestrado em Física (em curso)', N'Espectroscopia', N'Mestrando em espectroscopia de interfaces atômicas.', NULL, NULL, NULL, 1),
 (17, N'Renata Lopes Vieira', N'renata.vieira@alumni.gpfa.ufmg.br', 'E', N'/Content/img/avatars/renata-vieira.svg', N'Doutorado em Física (concluído em 2024)', N'Materiais Quânticos', N'Ex-orientanda do grupo, atualmente em pós-doutorado no exterior.', NULL, NULL, NULL, 1),
 (18, N'Felipe Andrade Santos', N'felipe.santos@alumni.gpfa.ufmg.br', 'E', N'/Content/img/avatars/felipe-santos.svg', N'Mestrado em Nanotecnologia (concluído em 2023)', N'Nanofotônica', N'Ex-orientando do grupo, atua na indústria de fotônica.', NULL, NULL, NULL, 1),
 (19, N'Isabela Moreira Dias', N'isabela.dias@alumni.gpfa.ufmg.br', 'E', N'/Content/img/avatars/isabela-dias.svg', N'Graduação em Ciência da Computação (concluída em 2025)', N'Computação Científica', N'Ex-bolsista de iniciação científica, hoje desenvolvedora de software científico.', NULL, NULL, NULL, 1);
SET IDENTITY_INSERT dbo.Pessoa OFF;
GO

INSERT dbo.Pesquisador (PesquisadorId, Titulacao, Instituicao, Departamento, LinhaPesquisa, DataCadastro) VALUES
 (1, N'Doutora', N'Universidade Federal de Minas Gerais', N'Departamento de Física', N'Materiais Quânticos', '2018-03-01'),
 (2, N'Doutor',  N'Universidade Federal de Minas Gerais', N'Departamento de Física', N'Nanofotônica', '2019-08-01'),
 (3, N'Doutora', N'Universidade Federal de Minas Gerais', N'Departamento de Ciência da Computação', N'Simulação Computacional', '2020-02-01'),
 (4, N'Doutor',  N'Universidade Federal de Minas Gerais', N'Departamento de Engenharia Sanitária e Ambiental', N'Sensores Ambientais', '2021-04-01'),
 (5, N'Doutora', N'Universidade Federal de Minas Gerais', N'Departamento de Química', N'Materiais Funcionais', '2019-02-01'),
 (6, N'Doutor',  N'Universidade Federal de Minas Gerais', N'Departamento de Física', N'Espectroscopia Avançada', '2022-07-01');
GO

INSERT dbo.Estudante (EstudanteId, NivelAcademico, Curso, Situacao, DataIngresso, DataConclusao, CoorientadoresExternos) VALUES
 (7,  N'Mestrado', N'Física', N'Ativo', '2024-03-01', NULL, N'Prof. Dr. André Vasconcelos (Universidade de Lisboa)'),
 (8,  N'Iniciação Científica', N'Engenharia de Materiais', N'Ativo', '2025-08-01', NULL, NULL),
 (9,  N'Doutorado', N'Nanotecnologia', N'Ativo', '2023-02-01', NULL, N'Dra. Elena Rossi (Politecnico di Torino)'),
 (10, N'Mestrado', N'Ciência da Computação', N'Ativo', '2024-08-01', NULL, NULL),
 (11, N'Iniciação Científica', N'Física', N'Ativo', '2025-03-01', NULL, NULL),
 (12, N'Técnico', N'Curso Técnico em Química', N'Ativo', '2025-05-01', NULL, NULL),
 (13, N'Doutorado', N'Química', N'Ativo', '2023-03-01', NULL, N'Prof. Dr. Marcos Aurélio (UFOP)'),
 (14, N'Mestrado', N'Engenharia Elétrica', N'Ativo', '2024-09-01', NULL, NULL),
 (15, N'Iniciação Científica', N'Engenharia Ambiental', N'Ativo', '2025-04-01', NULL, NULL),
 (16, N'Mestrado', N'Física', N'Ativo', '2025-02-01', NULL, NULL),
 (17, N'Doutorado', N'Física', N'Concluído', '2019-03-01', '2024-02-28', N'Prof. Dr. Peter Klein (RWTH Aachen)'),
 (18, N'Mestrado', N'Nanotecnologia', N'Concluído', '2021-03-01', '2023-12-15', NULL),
 (19, N'Graduação', N'Ciência da Computação', N'Concluído', '2022-02-01', '2025-03-10', NULL);
GO

SET IDENTITY_INSERT dbo.Projeto ON;
INSERT dbo.Projeto (ProjetoId, Titulo, Sigla, Resumo, Descricao, LinhaPesquisa, Situacao, DataInicio, DataFimPrevista, DataFim, CoordenadorId, ImagemUrl, Destaque) VALUES
 (1, N'Materiais quânticos bidimensionais para dispositivos de baixo consumo', N'MQ-2D',
     N'Investigação de heteroestruturas bidimensionais e de fenômenos de transporte com potencial para dispositivos eletrônicos de baixo consumo energético.',
     N'O projeto combina crescimento de amostras, caracterização espectroscópica e simulação de transporte para compreender mecanismos de dissipação em materiais bidimensionais. Os resultados orientam a escolha de heteroestruturas para transistores e sensores de alta eficiência.',
     N'Materiais Quânticos', N'Em andamento', '2024-01-01', '2026-12-31', NULL, 1, N'/Content/img/projetos/materiais-quanticos.svg', 1),
 (2, N'Nanofotônica aplicada a sensores ópticos de alta sensibilidade', N'NANO-SENSE',
     N'Desenvolvimento de microcavidades e metamateriais para sensoriamento óptico de biomarcadores e gases em concentrações ultratraço.',
     N'As atividades envolvem projeto de dispositivos, fabricação em sala limpa e caracterização óptica. O objetivo é reduzir limites de detecção e viabilizar sensores portáteis para diagnóstico e monitoramento ambiental.',
     N'Nanofotônica', N'Em andamento', '2023-03-01', '2026-12-31', NULL, 2, N'/Content/img/projetos/nanofotonica.svg', 0),
 (3, N'Simulação computacional de transporte eletrônico em nanoestruturas', N'QUANTUM-SIM',
     N'Modelagem multiescala de transporte quântico em nanoestruturas, com métodos numéricos de alto desempenho e validação experimental.',
     N'O projeto desenvolve códigos de simulação, pipelines de dados e visualizações para investigar regimes de transporte balístico e difusivo. Também investiga o uso de aprendizado de máquina como acelerador de simulações.',
     N'Simulação Computacional', N'Em andamento', '2025-02-01', '2027-01-31', NULL, 3, N'/Content/img/projetos/simulacao.svg', 1),
 (4, N'Rede de sensores ambientais inteligentes para a bacia do Rio das Velhas', N'RIO-VIVO',
     N'Implantação de uma rede de sensores de baixo custo para monitoramento contínuo da qualidade da água, com painel público de indicadores.',
     N'O projeto integra hardware aberto, comunicação sem fio e análise de dados para detectar alterações de temperatura, turbidez, condutividade e pH. A rede é operada em parceria com comunidades ribeirinhas e comitês de bacia.',
     N'Sensores Ambientais', N'Em andamento', '2024-08-01', '2026-07-31', NULL, 4, N'/Content/img/projetos/sensores-ambientais.svg', 1),
 (5, N'Síntese de materiais funcionalizados para remediação de água', N'ÁGUA-LIMPA',
     N'Desenvolvimento de adsorventes e fotocatalisadores de baixo custo para remoção de contaminantes emergentes em água.',
     N'Foram sintetizados e caracterizados materiais híbridos, avaliados em ensaios de adsorção e degradação fotocatalítica. O projeto gerou publicações, depósito de patente e formação de recursos humanos.',
     N'Materiais Funcionais', N'Concluído', '2021-03-01', '2024-12-31', '2024-12-31', 5, N'/Content/img/projetos/materiais-funcionais.svg', 0),
 (6, N'Espectroscopia avançada de interfaces atômicas', N'ATOM-SPEC',
     N'Desenvolvimento de metodologias de espectroscopia de alta resolução para caracterizar interfaces e defeitos em materiais nanoestruturados.',
     N'O projeto está em fase de planejamento e implantação de infraestrutura. Prevê a integração de técnicas ópticas e de fotoelétrons para estudos in situ de interfaces.',
     N'Espectroscopia Avançada', N'Planejamento', '2026-01-01', '2028-12-31', NULL, 6, N'/Content/img/projetos/espectroscopia.svg', 0);
SET IDENTITY_INSERT dbo.Projeto OFF;
GO

INSERT dbo.ParticipacaoProjeto (ProjetoId, PesquisadorId, Papel, DataEntrada, DataSaida) VALUES
 (1, 2, N'Pesquisador sênior', '2024-01-15', NULL),
 (1, 5, N'Pesquisadora colaboradora', '2024-02-01', NULL),
 (1, 6, N'Pesquisador colaborador', '2024-03-01', NULL),
 (2, 1, N'Pesquisadora colaboradora', '2023-03-15', NULL),
 (2, 4, N'Pesquisador colaborador', '2023-06-01', NULL),
 (3, 1, N'Pesquisadora colaboradora', '2025-02-15', NULL),
 (3, 6, N'Pesquisador colaborador', '2025-03-01', NULL),
 (4, 3, N'Pesquisadora colaboradora', '2024-08-15', NULL),
 (4, 5, N'Pesquisadora colaboradora', '2024-09-01', NULL),
 (5, 6, N'Pesquisador colaborador', '2021-04-01', '2024-12-31'),
 (5, 1, N'Pesquisadora colaboradora', '2021-05-01', '2024-12-31'),
 (6, 2, N'Pesquisador colaborador', '2026-01-15', NULL);
GO

INSERT dbo.EstudanteProjeto (EstudanteId, ProjetoId, DataEntrada, DataSaida, TipoVinculo) VALUES
 (7, 1, '2024-03-01', NULL, N'Bolsa de mestrado'),
 (7, 3, '2025-03-01', NULL, N'Colaboração em simulação'),
 (8, 2, '2025-08-01', NULL, N'Iniciação científica'),
 (9, 3, '2023-02-01', NULL, N'Bolsa de doutorado'),
 (10, 3, '2024-08-01', NULL, N'Bolsa de mestrado'),
 (11, 1, '2025-03-01', NULL, N'Iniciação científica'),
 (12, 4, '2025-05-01', NULL, N'Apoio técnico'),
 (13, 1, '2023-03-01', NULL, N'Bolsa de doutorado'),
 (13, 5, '2023-03-01', '2024-12-31', N'Pesquisa de tese'),
 (14, 4, '2024-09-01', NULL, N'Bolsa de mestrado'),
 (15, 4, '2025-04-01', NULL, N'Iniciação científica'),
 (16, 6, '2025-02-01', NULL, N'Bolsa de mestrado'),
 (17, 1, '2019-03-01', '2024-02-28', N'Bolsa de doutorado'),
 (18, 2, '2021-03-01', '2023-12-15', N'Bolsa de mestrado'),
 (19, 3, '2022-03-01', '2025-03-10', N'Iniciação científica');
GO

INSERT dbo.Orientacao (EstudanteId, OrientadorId, Principal, DataInicio, DataFim, Observacao) VALUES
 (7, 1, 1, '2024-03-01', NULL, N'Orientação de mestrado em andamento.'),
 (8, 2, 1, '2025-08-01', NULL, N'Orientação de iniciação científica.'),
 (9, 3, 1, '2023-02-01', NULL, N'Orientação de doutorado em andamento.'),
 (10, 3, 1, '2024-08-01', NULL, N'Orientação de mestrado em andamento.'),
 (11, 1, 1, '2025-03-01', NULL, N'Orientação de iniciação científica.'),
 (12, 5, 1, '2025-05-01', NULL, N'Orientação técnica.'),
 (13, 5, 1, '2023-03-01', NULL, N'Orientação de doutorado em andamento.'),
 (14, 4, 1, '2024-09-01', NULL, N'Orientação de mestrado em andamento.'),
 (15, 4, 1, '2025-04-01', NULL, N'Orientação de iniciação científica.'),
 (16, 6, 1, '2025-02-01', NULL, N'Orientação de mestrado em andamento.'),
 (17, 5, 1, '2019-03-01', '2021-02-28', N'Primeira orientadora durante o doutorado.'),
 (17, 1, 1, '2021-03-01', '2024-02-28', N'Orientação concluída com defesa em fevereiro de 2024.'),
 (18, 2, 1, '2021-03-01', '2023-12-15', N'Orientação concluída com defesa em dezembro de 2023.'),
 (19, 3, 1, '2022-03-01', '2025-03-10', N'Orientação de graduação concluída.');
GO

SET IDENTITY_INSERT dbo.Publicacao ON;
INSERT dbo.Publicacao (PublicacaoId, Titulo, Veiculo, Ano, Tipo, Doi, LinkAcesso, ProjetoId) VALUES
 (1,  N'Transporte eletrônico em heteroestruturas bidimensionais: efeitos de interface e desordem', N'Revista Brasileira de Física Aplicada', 2026, N'Artigo', N'10.1000/gpfa.2026.001', N'https://doi.org/10.1000/gpfa.2026.001', 1),
 (2,  N'Sensores ópticos baseados em ressonâncias de modo galeria em microcavidades', N'Photonics Letters', 2025, N'Artigo', N'10.1000/gpfa.2025.014', N'https://doi.org/10.1000/gpfa.2025.014', 2),
 (3,  N'Simulação de transporte quântico em nanoestruturas de grafeno com contatos realistas', N'Journal of Computational Nanoscience', 2025, N'Artigo', N'10.1000/gpfa.2025.007', N'https://doi.org/10.1000/gpfa.2025.007', 3),
 (4,  N'Materiais funcionalizados para remediação de águas contaminadas por contaminantes emergentes', N'Química Nova', 2024, N'Artigo', N'10.1000/gpfa.2024.022', N'https://doi.org/10.1000/gpfa.2024.022', 5),
 (5,  N'Rede de sensores ambientais de baixo custo para monitoramento contínuo de rios', N'Environmental Monitoring and Assessment', 2024, N'Artigo', N'10.1000/gpfa.2024.031', N'https://doi.org/10.1000/gpfa.2024.031', 4),
 (6,  N'Espectroscopia Raman de interfaces atômicas em filmes ultrafinos', N'Physical Review B', 2024, N'Artigo', N'10.1000/gpfa.2024.045', N'https://doi.org/10.1000/gpfa.2024.045', 1),
 (7,  N'Nanofotônica aplicada à detecção de biomarcadores em baixas concentrações', N'Optics Express', 2023, N'Artigo', N'10.1000/gpfa.2023.018', N'https://doi.org/10.1000/gpfa.2023.018', 2),
 (8,  N'Síntese verde de nanopartículas magnéticas para tratamento de efluentes', N'Materials Chemistry and Physics', 2023, N'Artigo', N'10.1000/gpfa.2023.009', N'https://doi.org/10.1000/gpfa.2023.009', 5),
 (9,  N'Modelagem multiescala de transporte em dispositivos bidimensionais', N'Computational Materials Science', 2022, N'Artigo', N'10.1000/gpfa.2022.027', N'https://doi.org/10.1000/gpfa.2022.027', 3),
 (10, N'Sensores ambientais para monitoramento de rios: uma revisão crítica', N'Water Research', 2021, N'Artigo', N'10.1000/gpfa.2021.004', N'https://doi.org/10.1000/gpfa.2021.004', 4),
 (11, N'Propriedades ópticas de pontos quânticos coloidais em matrizes poliméricas', N'Journal of Luminescence', 2020, N'Artigo', N'10.1000/gpfa.2020.013', N'https://doi.org/10.1000/gpfa.2020.013', 2),
 (12, N'Espectroscopia de fotoelétrons em filmes ultrafinos: um guia prático', N'Surface Science Reports', 2019, N'Artigo', N'10.1000/gpfa.2019.002', N'https://doi.org/10.1000/gpfa.2019.002', NULL),
 (13, N'Perspectivas em materiais quânticos para computação de baixo consumo', N'Reviews in Quantum Materials', 2026, N'Artigo', N'10.1000/gpfa.2026.002', N'https://doi.org/10.1000/gpfa.2026.002', 1),
 (14, N'Sensoriamento ambiental participativo: integração entre ciência cidadã e dados abertos', N'Sustainability', 2025, N'Artigo', N'10.1000/gpfa.2025.021', N'https://doi.org/10.1000/gpfa.2025.021', 4);
SET IDENTITY_INSERT dbo.Publicacao OFF;
GO

INSERT dbo.PublicacaoAutor (PublicacaoId, PesquisadorId, OrdemAutoria) VALUES
 (1, 1, 1), (1, 2, 2), (1, 3, 3),
 (2, 2, 1), (2, 4, 2),
 (3, 3, 1), (3, 1, 2),
 (4, 5, 1), (4, 6, 2),
 (5, 4, 1), (5, 5, 2), (5, 3, 3),
 (6, 6, 1), (6, 1, 2),
 (7, 2, 1), (7, 4, 2),
 (8, 5, 1),
 (9, 3, 1),
 (10, 4, 1), (10, 5, 2),
 (11, 2, 1), (11, 1, 2), (11, 6, 3),
 (12, 6, 1),
 (13, 1, 1), (13, 3, 2),
 (14, 4, 1), (14, 5, 2);
GO

INSERT dbo.DestaquePublicacao (PesquisadorId, PublicacaoId, Ordem) VALUES
 (1, 13, 1), (1, 1, 2), (1, 3, 3), (1, 11, 4), (1, 6, 5),
 (2, 2, 1), (2, 7, 2), (2, 1, 3), (2, 11, 4),
 (3, 3, 1), (3, 1, 2), (3, 5, 3),
 (4, 5, 1), (4, 14, 2), (4, 7, 3), (4, 10, 4),
 (5, 4, 1), (5, 8, 2), (5, 5, 3),
 (6, 6, 1), (6, 11, 2), (6, 12, 3);
GO

SET IDENTITY_INSERT dbo.FonteFinanciadora ON;
INSERT dbo.FonteFinanciadora (FonteId, Nome, Sigla, Tipo, SiteUrl) VALUES
 (1, N'Conselho Nacional de Desenvolvimento Científico e Tecnológico', N'CNPq', N'Agência pública', N'https://www.gov.br/cnpq'),
 (2, N'Coordenação de Aperfeiçoamento de Pessoal de Nível Superior', N'CAPES', N'Agência pública', N'https://www.gov.br/capes'),
 (3, N'Fundação de Amparo à Pesquisa do Estado de Minas Gerais', N'FAPEMIG', N'Fundação', N'https://fapemig.br'),
 (4, N'Instituto Horizonte de Inovação', N'IHI', N'Parceria privada', NULL),
 (5, N'Rede Nacional de Laboratórios Ambientais', N'RNLA', N'Outra', NULL);
SET IDENTITY_INSERT dbo.FonteFinanciadora OFF;
GO

INSERT dbo.Financiamento (ProjetoId, FonteId, Descricao, NumeroProcesso, Valor, DataInicio, DataFim, Moeda) VALUES
 (1, 1, N'Edital Universal - Materiais quânticos bidimensionais', N'401234/2024-0', 120000.00, '2024-01-01', '2026-12-31', 'BRL'),
 (1, 3, N'Projeto de pesquisa científica e tecnológica', N'APQ-01234-24', 80000.00, '2024-03-01', '2026-02-28', 'BRL'),
 (1, 4, N'Acordo de cooperação para dispositivos de baixo consumo', N'IHI-2025-07', 50000.00, '2025-01-01', '2025-12-31', 'BRL'),
 (2, 2, N'Programa de apoio à pós-graduação - PROEX', N'CAPES-001', 90000.00, '2023-03-01', '2026-12-31', 'BRL'),
 (2, 3, N'Apoio a projetos de nanofotônica', N'APQ-04567-23', 60000.00, '2023-06-01', '2025-12-31', 'BRL'),
 (3, 1, N'Bolsa e custeio para simulação computacional', N'402345/2025-1', 45000.00, '2025-02-01', '2027-01-31', 'BRL'),
 (3, 2, N'Apoio à pós-graduação em computação científica', N'CAPES-002', 30000.00, '2025-03-01', '2026-12-31', 'BRL'),
 (4, 3, N'Rede de monitoramento ambiental do Rio das Velhas', N'APQ-07890-24', 110000.00, '2024-08-01', '2026-07-31', 'BRL'),
 (4, 5, N'Parceria para operação da rede de sensores', N'RNLA-2025-03', 40000.00, '2025-01-01', '2026-06-30', 'BRL'),
 (5, 1, N'Edital Universal - remediação de água', N'403456/2021-2', 70000.00, '2021-03-01', '2024-12-31', 'BRL'),
 (5, 2, N'Bolsa de doutorado vinculada ao projeto', N'CAPES-003', 25000.00, '2021-03-01', '2024-12-31', 'BRL'),
 (6, 1, N'Apoio à implantação de infraestrutura de espectroscopia', N'404567/2026-9', 35000.00, '2026-01-01', '2027-12-31', 'BRL');
GO

PRINT N'Dados de demonstração inseridos com sucesso.';
GO
