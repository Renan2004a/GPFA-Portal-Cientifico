using System;
using System.Collections.Generic;
using System.Linq;
using GPFA.Data;
using GPFA.Models;

namespace GPFA.Business
{
    public class ProjetoService
    {
        private readonly ProjetoRepository _repository;
        private readonly PesquisadorRepository _pesquisadorRepository;
        private readonly FinanciamentoRepository _financiamentoRepository;
        private readonly PublicacaoRepository _publicacaoRepository;
        private readonly EstudanteRepository _estudanteRepository;

        public ProjetoService(ProjetoRepository repository,
            PesquisadorRepository pesquisadorRepository,
            FinanciamentoRepository financiamentoRepository,
            PublicacaoRepository publicacaoRepository,
            EstudanteRepository estudanteRepository)
        {
            _repository = repository;
            _pesquisadorRepository = pesquisadorRepository;
            _financiamentoRepository = financiamentoRepository;
            _publicacaoRepository = publicacaoRepository;
            _estudanteRepository = estudanteRepository;
        }

        public List<ProjetoResumo> Listar(string termo = null, string situacao = null,
            int? coordenadorId = null, bool? destaque = null, string linha = null)
        {
            return _repository.Listar(termo, situacao, coordenadorId, destaque, linha);
        }

        public List<ProjetoResumo> ListarDestaques(int limite)
        {
            return _repository.ListarDestaques(limite);
        }

        public ProjetoResumo ObterResumo(int id)
        {
            return _repository.ObterResumo(id);
        }

        public Projeto ObterEntidade(int id)
        {
            return _repository.ObterEntidade(id);
        }

        public ProjetoDetalhe ObterDetalhe(int id)
        {
            var resumo = _repository.ObterResumo(id);
            if (resumo == null) return null;

            return new ProjetoDetalhe
            {
                Projeto = resumo,
                Coordenador = _pesquisadorRepository.ObterResumo(resumo.CoordenadorId),
                Participantes = _repository.ListarParticipantesAtivos(id),
                Estudantes = _repository.ListarEstudantes(id),
                Financiamentos = _financiamentoRepository.ListarResumoPorProjeto(id),
                Publicacoes = _publicacaoRepository.Listar(new FiltroPublicacoes { ProjetoId = id })
            };
        }

        public int Salvar(Projeto projeto, UsuarioSessao usuario)
        {
            if (projeto == null)
                throw new ValidacaoException("Dados do projeto não informados.");
            if (usuario == null || !usuario.PodeGerenciarProjetos)
                throw new RegraNegocioException("Você não possui permissão para gerenciar projetos.");

            var erros = new List<string>();
            Validacao.Coletar(erros, () => projeto.Validar());
            Validacao.Coletar(erros, () => projeto.Titulo = Validacao.TextoObrigatorio(projeto.Titulo, "título", 5, 200));
            Validacao.Coletar(erros, () => projeto.Resumo = Validacao.TextoObrigatorio(projeto.Resumo, "resumo", 10, 1000));
            Validacao.Coletar(erros, () => projeto.Sigla = string.IsNullOrWhiteSpace(projeto.Sigla) ? null
                : Validacao.TextoObrigatorio(projeto.Sigla, "sigla", 2, 30));
            Validacao.Coletar(erros, () => projeto.LinhaPesquisa = string.IsNullOrWhiteSpace(projeto.LinhaPesquisa) ? null
                : Validacao.TextoObrigatorio(projeto.LinhaPesquisa, "linha de pesquisa", 2, 120));
            Validacao.Coletar(erros, () => projeto.ImagemUrl = Validacao.UrlOpcionalOuRelativa(projeto.ImagemUrl, "imagem"));
            Validacao.Coletar(erros, () =>
            {
                if (projeto.DataInicio == DateTime.MinValue)
                    throw new ValidacaoException("Informe a data de início do projeto.");
            });
            Validacao.LancarSeHouver(erros);

            if (usuario.EhCoordenador && !usuario.EhAdministrador)
            {
                if (!usuario.PesquisadorId.HasValue)
                    throw new RegraNegocioException("O usuário coordenador não está vinculado a um pesquisador.");
                if (projeto.ProjetoId > 0)
                {
                    var atual = _repository.ObterEntidade(projeto.ProjetoId);
                    if (atual == null || atual.CoordenadorId != usuario.PesquisadorId.Value)
                        throw new RegraNegocioException("Você só pode editar projetos sob sua coordenação.");
                }
                projeto.CoordenadorId = usuario.PesquisadorId.Value;
            }

            var coordenador = _pesquisadorRepository.ObterResumo(projeto.CoordenadorId);
            if (coordenador == null)
                throw new ValidacaoException("O coordenador deve ser um pesquisador cadastrado (RN03).");
            if (!string.IsNullOrWhiteSpace(projeto.Situacao) &&
                Array.IndexOf(Dominio.SituacaoProjeto.Todos, projeto.Situacao) < 0)
                throw new ValidacaoException("Situação de projeto inválida.");

            if (projeto.ProjetoId > 0)
            {
                _repository.Atualizar(projeto);
                return projeto.ProjetoId;
            }

            return _repository.Inserir(projeto);
        }

        public void Excluir(int projetoId, UsuarioSessao usuario)
        {
            if (usuario == null || !usuario.EhAdministrador)
                throw new RegraNegocioException("Somente o administrador pode excluir projetos.");

            var vinculos = _repository.ObterVinculos(projetoId);
            if (vinculos == null)
                throw new ValidacaoException("Projeto não encontrado.");

            if (vinculos.Total > 0)
            {
                var detalhes = new List<string>();
                if (vinculos.Publicacoes > 0) detalhes.Add(vinculos.Publicacoes + " publicação(ões)");
                if (vinculos.Financiamentos > 0) detalhes.Add(vinculos.Financiamentos + " financiamento(s)");
                if (vinculos.Participacoes > 0) detalhes.Add(vinculos.Participacoes + " participação(ões) de pesquisadores");
                if (vinculos.Estudantes > 0) detalhes.Add(vinculos.Estudantes + " vínculo(s) de estudantes");

                throw new IntegridadeDadosException(
                    "O projeto não pode ser excluído porque possui " + string.Join(", ", detalhes) +
                    ". Remova ou transfira esses vínculos primeiro para preservar a integridade dos dados.");
            }

            _repository.Excluir(projetoId);
        }

        public bool PodeGerenciar(UsuarioSessao usuario, int projetoId)
        {
            if (usuario == null) return false;
            if (usuario.EhAdministrador) return true;
            if (!usuario.EhCoordenador || !usuario.PesquisadorId.HasValue) return false;

            var projeto = _repository.ObterResumo(projetoId);
            return projeto != null && projeto.CoordenadorId == usuario.PesquisadorId.Value;
        }

        public List<ProjetoResumo> ListarGerenciaveis(UsuarioSessao usuario)
        {
            if (usuario == null) return new List<ProjetoResumo>();
            if (usuario.EhAdministrador) return _repository.Listar();
            if (!usuario.EhCoordenador || !usuario.PesquisadorId.HasValue) return new List<ProjetoResumo>();
            return _repository.ListarDoCoordenador(usuario.PesquisadorId.Value);
        }

        /* ------------------------------- Participações --------------------------- */

        public List<ParticipacaoProjeto> ListarParticipacoes(int projetoId)
        {
            return _repository.ListarParticipacoes(projetoId);
        }

        public void AdicionarParticipante(int projetoId, int pesquisadorId, string papel,
            DateTime dataEntrada, UsuarioSessao usuario)
        {
            AutorizarGestao(usuario, projetoId);

            var projeto = _repository.ObterEntidade(projetoId);
            if (projeto == null) throw new ValidacaoException("Projeto não encontrado.");
            if (projeto.CoordenadorId == pesquisadorId)
                throw new ValidacaoException("O coordenador já é participante implícito e não precisa ser adicionado novamente (RN03).");
            if (projeto.Concluido)
                throw new ValidacaoException("Não é possível adicionar participantes a um projeto concluído.");
            if (!_pesquisadorRepository.ObterResumo(pesquisadorId).Ativo)
                throw new ValidacaoException("Selecione um pesquisador ativo.");
            if (dataEntrada > DateTime.Today)
                throw new ValidacaoException("A data de entrada não pode ser futura.");
            if (dataEntrada < projeto.DataInicio)
                throw new ValidacaoException("A participação não pode começar antes do início do projeto.");

            _repository.InserirParticipacao(new ParticipacaoProjeto
            {
                ProjetoId = projetoId,
                PesquisadorId = pesquisadorId,
                Papel = string.IsNullOrWhiteSpace(papel) ? "Pesquisador" : papel.Trim(),
                DataEntrada = dataEntrada
            });
        }

        public void EncerrarParticipacao(int participacaoId, DateTime dataSaida, UsuarioSessao usuario)
        {
            var participacao = _repository.ObterParticipacao(participacaoId);
            if (participacao == null) throw new ValidacaoException("Participação não encontrada.");
            AutorizarGestao(usuario, participacao.ProjetoId);

            if (dataSaida > DateTime.Today)
                throw new ValidacaoException("A data de saída não pode ser futura.");
            if (dataSaida < participacao.DataEntrada)
                throw new ValidacaoException("A data de saída não pode ser anterior à entrada.");

            _repository.EncerrarParticipacao(participacaoId, dataSaida);
        }

        public void ExcluirParticipacao(int participacaoId, UsuarioSessao usuario)
        {
            var participacao = _repository.ObterParticipacao(participacaoId);
            if (participacao == null) throw new ValidacaoException("Participação não encontrada.");
            AutorizarGestao(usuario, participacao.ProjetoId);
            _repository.ExcluirParticipacao(participacaoId);
        }

        /* --------------------------- Estudantes no projeto ----------------------- */

        public List<EstudanteResumo> ListarEstudantes(int projetoId)
        {
            return _repository.ListarEstudantes(projetoId);
        }

        public List<EstudanteProjeto> ListarVinculosEstudantes(int projetoId)
        {
            return _repository.ListarVinculosEstudantes(projetoId);
        }

        public void VincularEstudante(int projetoId, int estudanteId, DateTime dataEntrada,
            string tipoVinculo, UsuarioSessao usuario)
        {
            AutorizarGestao(usuario, projetoId);
            var estudante = _estudanteRepository.ObterResumo(estudanteId);
            if (estudante == null)
                throw new ValidacaoException("Estudante não encontrado.");
            if (estudante.Situacao != Dominio.SituacaoEstudante.Ativo)
                throw new ValidacaoException("Apenas estudantes ativos podem ser vinculados a novos projetos.");
            if (dataEntrada > DateTime.Today)
                throw new ValidacaoException("A data de entrada não pode ser futura.");
            _repository.VincularEstudante(projetoId, estudanteId, dataEntrada, tipoVinculo);
        }

        public void EncerrarVinculoEstudante(int estudanteProjetoId, DateTime dataSaida, UsuarioSessao usuario)
        {
            var vinculo = _repository.ListarVinculosEstudantes(
                ObterProjetoIdDoVinculo(estudanteProjetoId)).FirstOrDefault(v => v.EstudanteProjetoId == estudanteProjetoId);
            if (vinculo == null) throw new ValidacaoException("Vínculo não encontrado.");
            AutorizarGestao(usuario, vinculo.ProjetoId);
            if (dataSaida > DateTime.Today)
                throw new ValidacaoException("A data de saída não pode ser futura.");
            _repository.EncerrarVinculoEstudante(estudanteProjetoId, dataSaida);
        }

        public void ExcluirVinculoEstudante(int estudanteProjetoId, UsuarioSessao usuario)
        {
            var vinculo = _repository.ListarVinculosEstudantes(
                ObterProjetoIdDoVinculo(estudanteProjetoId)).FirstOrDefault(v => v.EstudanteProjetoId == estudanteProjetoId);
            if (vinculo == null) throw new ValidacaoException("Vínculo não encontrado.");
            AutorizarGestao(usuario, vinculo.ProjetoId);
            _repository.ExcluirVinculoEstudante(estudanteProjetoId);
        }

        private int ObterProjetoIdDoVinculo(int estudanteProjetoId)
        {
            return _repository.ObterProjetoIdDoVinculo(estudanteProjetoId);
        }

        private void AutorizarGestao(UsuarioSessao usuario, int projetoId)
        {
            if (!PodeGerenciar(usuario, projetoId))
                throw new RegraNegocioException("Você não possui permissão para gerenciar este projeto.");
        }
    }
}
