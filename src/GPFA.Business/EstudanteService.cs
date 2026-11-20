using System;
using System.Collections.Generic;
using System.Linq;
using GPFA.Data;
using GPFA.Models;

namespace GPFA.Business
{
    public class EstudanteService
    {
        private readonly EstudanteRepository _repository;
        private readonly OrientacaoRepository _orientacaoRepository;
        private readonly PesquisadorRepository _pesquisadorRepository;
        private readonly ProjetoRepository _projetoRepository;

        public EstudanteService(EstudanteRepository repository,
            OrientacaoRepository orientacaoRepository,
            PesquisadorRepository pesquisadorRepository,
            ProjetoRepository projetoRepository)
        {
            _repository = repository;
            _orientacaoRepository = orientacaoRepository;
            _pesquisadorRepository = pesquisadorRepository;
            _projetoRepository = projetoRepository;
        }

        public List<EstudanteResumo> Listar(string termo = null, string nivel = null,
            string situacao = null, int? orientadorId = null)
        {
            var lista = _repository.Listar(termo, nivel, situacao, orientadorId);
            foreach (var estudante in lista)
                estudante.Projetos = _repository.ListarProjetos(estudante.EstudanteId);
            return lista;
        }

        public EstudanteResumo ObterResumo(int id)
        {
            return _repository.ObterResumo(id);
        }

        public Estudante ObterEntidade(int id)
        {
            return _repository.ObterEntidade(id);
        }

        public EstudantePerfil ObterPerfil(int id)
        {
            var estudante = _repository.ObterResumo(id);
            if (estudante == null) return null;

            var orientacoes = _repository.ListarOrientacoes(id);
            return new EstudantePerfil
            {
                Estudante = estudante,
                OrientacaoAtual = orientacoes.FirstOrDefault(o => o.Principal && o.DataFim == null),
                Orientacoes = orientacoes,
                Projetos = _repository.ListarProjetos(id)
            };
        }

        public int Criar(Estudante estudante, int orientadorId, IList<int> projetoIds, string tipoVinculo)
        {
            ValidarEstudante(estudante, true);

            if (projetoIds == null || projetoIds.Count == 0)
                throw new ValidacaoException("Todo estudante deve estar vinculado a ao menos um projeto de pesquisa (RN01).");

            var ids = projetoIds.Distinct().ToList();
            foreach (var projetoId in ids)
            {
                var projeto = _projetoRepository.ObterResumo(projetoId);
                if (projeto == null)
                    throw new ValidacaoException("Um dos projetos selecionados não existe mais. Atualize a página e tente novamente.");
            }

            var orientador = _pesquisadorRepository.ObterResumo(orientadorId);
            if (orientador == null || !orientador.Ativo)
                throw new ValidacaoException("O orientador principal deve ser um pesquisador ativo cadastrado no sistema (RN04).");

            if (estudante.DataIngresso > DateTime.Today)
                throw new ValidacaoException("A data de ingresso não pode ser futura.");

            estudante.Situacao = Dominio.SituacaoEstudante.Ativo;
            estudante.DataConclusao = null;
            estudante.Ativo = true;

            return _repository.Inserir(estudante, orientadorId, ids, estudante.DataIngresso, tipoVinculo);
        }

        public void Atualizar(Estudante estudante)
        {
            var atual = _repository.ObterEntidade(estudante.EstudanteId);
            if (atual == null)
                throw new ValidacaoException("Estudante não encontrado.");

            ValidarEstudante(estudante, false);

            if (estudante.Situacao == Dominio.SituacaoEstudante.Concluido)
            {
                if (!estudante.DataConclusao.HasValue)
                    throw new ValidacaoException("Informe a data de conclusão do estudante.");
                if (estudante.DataConclusao.Value < estudante.DataIngresso)
                    throw new ValidacaoException("A data de conclusão não pode ser anterior ao ingresso.");

                if (atual.Situacao != Dominio.SituacaoEstudante.Concluido)
                {
                    // Fecha orientações e vínculos com projetos de forma consistente.
                    _repository.Concluir(estudante.EstudanteId, estudante.DataConclusao.Value);
                    estudante.Situacao = Dominio.SituacaoEstudante.Concluido;
                }
            }
            else if (estudante.Situacao == Dominio.SituacaoEstudante.Ativo)
            {
                estudante.DataConclusao = null;
                if (!_orientacaoRepository.ExistePrincipalAtiva(estudante.EstudanteId))
                    throw new ValidacaoException("Antes de reativar o estudante, defina um orientador principal ativo.");
                if (_projetoRepository.ContarProjetosAtivosDoEstudante(estudante.EstudanteId) == 0)
                    throw new ValidacaoException("Antes de reativar o estudante, vincule-o a ao menos um projeto.");
            }

            _repository.Atualizar(estudante);
        }

        public void Concluir(int estudanteId, DateTime dataConclusao)
        {
            if (dataConclusao > DateTime.Today)
                throw new ValidacaoException("A data de conclusão não pode ser futura.");
            _repository.Concluir(estudanteId, dataConclusao);
        }

        public void Excluir(int estudanteId)
        {
            var estudante = _repository.ObterResumo(estudanteId);
            if (estudante == null)
                throw new ValidacaoException("Estudante não encontrado.");
            _repository.Excluir(estudanteId);
        }

        public void DefinirOrientadorPrincipal(int estudanteId, int orientadorId, DateTime dataInicio, string observacao)
        {
            var estudante = _repository.ObterResumo(estudanteId);
            if (estudante == null) throw new ValidacaoException("Estudante não encontrado.");

            var orientador = _pesquisadorRepository.ObterResumo(orientadorId);
            if (orientador == null || !orientador.Ativo)
                throw new ValidacaoException("O orientador principal deve ser um pesquisador ativo cadastrado (RN04).");

            if (dataInicio > DateTime.Today)
                throw new ValidacaoException("A data de início da orientação não pode ser futura.");
            if (dataInicio < estudante.DataIngresso)
                throw new ValidacaoException("A orientação não pode começar antes do ingresso do estudante.");

            _orientacaoRepository.DefinirPrincipal(estudanteId, orientadorId, dataInicio, observacao);
        }

        public void AdicionarCoorientacaoInterna(int estudanteId, int orientadorId, DateTime dataInicio, string observacao)
        {
            if (_repository.ObterResumo(estudanteId) == null)
                throw new ValidacaoException("Estudante não encontrado.");
            if (_pesquisadorRepository.ObterResumo(orientadorId) == null)
                throw new ValidacaoException("O coorientador interno deve ser um pesquisador cadastrado.");
            if (dataInicio > DateTime.Today)
                throw new ValidacaoException("A data de início não pode ser futura.");

            _orientacaoRepository.InserirCoorientacao(estudanteId, orientadorId, dataInicio, observacao);
        }

        public void EncerrarOrientacao(int orientacaoId, DateTime dataFim)
        {
            if (dataFim > DateTime.Today)
                throw new ValidacaoException("A data de encerramento não pode ser futura.");

            var orientacao = _orientacaoRepository.Obter(orientacaoId);
            if (orientacao == null)
                throw new ValidacaoException("Orientação não encontrada.");
            if (orientacao.DataFim.HasValue)
                throw new ValidacaoException("Esta orientação já está encerrada.");
            if (dataFim < orientacao.DataInicio)
                throw new ValidacaoException("A data de encerramento não pode ser anterior ao início.");

            if (orientacao.Principal &&
                string.Equals(orientacao.EstudanteSituacao, Dominio.SituacaoEstudante.Ativo,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new IntegridadeDadosException(
                    "Um estudante ativo não pode ficar sem orientador principal. " +
                    "Defina um novo orientador principal antes de encerrar a orientação atual (RN04).");
            }

            _orientacaoRepository.Encerrar(orientacaoId, dataFim);
        }

        public void ExcluirOrientacao(int orientacaoId)
        {
            var orientacao = _orientacaoRepository.Obter(orientacaoId);
            if (orientacao == null)
                throw new ValidacaoException("Orientação não encontrada.");

            if (orientacao.Principal && orientacao.DataFim == null &&
                string.Equals(orientacao.EstudanteSituacao, Dominio.SituacaoEstudante.Ativo,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new IntegridadeDadosException(
                    "A orientação principal de um estudante ativo não pode ser excluída. " +
                    "Defina outro orientador ou conclua/desligue o estudante (RN04).");
            }

            _orientacaoRepository.Excluir(orientacaoId);
        }

        public List<Orientacao> ListarOrientacoes(int estudanteId)
        {
            return _repository.ListarOrientacoes(estudanteId);
        }

        public List<ProjetoResumo> ListarProjetos(int estudanteId)
        {
            return _repository.ListarProjetos(estudanteId);
        }

        public List<Orientacao> ListarTodas(string termo = null, int? orientadorId = null, string vinculo = null)
        {
            return _orientacaoRepository.Listar(termo, orientadorId, vinculo);
        }

        private void ValidarEstudante(Estudante estudante, bool novo)
        {
            if (estudante == null)
                throw new ValidacaoException("Dados do estudante não informados.");

            var erros = new List<string>();
            Validacao.Coletar(erros, () => estudante.ValidarDadosBasicos());
            Validacao.Coletar(erros, () => estudante.NomeCompleto = Validacao.TextoObrigatorio(estudante.NomeCompleto, "nome completo", 3, 150));
            Validacao.Coletar(erros, () => estudante.Email = Validacao.Email(estudante.Email));
            Validacao.Coletar(erros, () => estudante.Curso = Validacao.TextoObrigatorio(estudante.Curso, "curso", 2, 120));
            Validacao.Coletar(erros, () => estudante.NivelAcademico = Validacao.TextoObrigatorio(estudante.NivelAcademico, "nível acadêmico", 2, 40));
            Validacao.Coletar(erros, () => estudante.CoorientadoresExternos = string.IsNullOrWhiteSpace(estudante.CoorientadoresExternos)
                ? null : Validacao.TextoObrigatorio(estudante.CoorientadoresExternos, "coorientadores", 2, 500));
            Validacao.Coletar(erros, () => estudante.FotoUrl = Validacao.UrlOpcionalOuRelativa(estudante.FotoUrl, "foto"));
            Validacao.Coletar(erros, () => estudante.GoogleScholarUrl = Validacao.UrlOpcional(estudante.GoogleScholarUrl, "Google Scholar"));
            Validacao.Coletar(erros, () => estudante.OrcidUrl = Validacao.UrlOpcional(estudante.OrcidUrl, "ORCID"));
            Validacao.Coletar(erros, () => estudante.LattesUrl = Validacao.UrlOpcional(estudante.LattesUrl, "Currículo Lattes"));
            Validacao.Coletar(erros, () =>
            {
                if (Array.IndexOf(Dominio.NivelAcademico.Todos, estudante.NivelAcademico) < 0)
                    throw new ValidacaoException("Nível acadêmico inválido.");
            });
            Validacao.Coletar(erros, () =>
            {
                if (estudante.DataIngresso == DateTime.MinValue)
                    throw new ValidacaoException("Informe a data de ingresso.");
            });
            Validacao.LancarSeHouver(erros);

            if (_repository.EmailEmUso(estudante.Email, novo ? (int?)null : estudante.PessoaId))
                throw new ValidacaoException("Já existe uma pessoa cadastrada com este e-mail.");
            if (!novo && !string.IsNullOrEmpty(estudante.Situacao) &&
                Array.IndexOf(Dominio.SituacaoEstudante.Todas, estudante.Situacao) < 0)
                throw new ValidacaoException("Situação do estudante inválida.");
        }
    }
}
