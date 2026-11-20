using System;
using System.Collections.Generic;
using System.Linq;
using GPFA.Data;
using GPFA.Models;

namespace GPFA.Business
{
    public class PesquisadorService
    {
        private readonly PesquisadorRepository _repository;

        public PesquisadorService(PesquisadorRepository repository)
        {
            _repository = repository;
        }

        public List<PesquisadorResumo> Listar(string termo = null, string area = null, bool somenteAtivos = true)
        {
            return _repository.Listar(termo, area, somenteAtivos);
        }

        public PesquisadorResumo ObterResumo(int id)
        {
            return _repository.ObterResumo(id);
        }

        public Pesquisador ObterEntidade(int id)
        {
            return _repository.ObterEntidade(id);
        }

        public PesquisadorPerfil ObterPerfil(int id)
        {
            var pesquisador = _repository.ObterResumo(id);
            if (pesquisador == null) return null;

            return new PesquisadorPerfil
            {
                Pesquisador = pesquisador,
                Projetos = _repository.ListarProjetos(id),
                ProjetosCoordenados = _repository.ListarProjetosCoordenados(id),
                OrientandosAtuais = _repository.ListarOrientandos(id, true),
                OrientandosPassados = _repository.ListarOrientandos(id, false),
                Publicacoes = _repository.ListarPublicacoes(id),
                Destaques = _repository.ListarDestaques(id)
            };
        }

        public int Salvar(Pesquisador pesquisador)
        {
            if (pesquisador == null)
                throw new ValidacaoException("Dados do pesquisador não informados.");

            var erros = new List<string>();
            Validacao.Coletar(erros, () => pesquisador.ValidarDadosBasicos());
            Validacao.Coletar(erros, () => pesquisador.NomeCompleto = Validacao.TextoObrigatorio(pesquisador.NomeCompleto, "nome completo", 3, 150));
            Validacao.Coletar(erros, () => pesquisador.Email = Validacao.Email(pesquisador.Email));
            Validacao.Coletar(erros, () => pesquisador.FormacaoAcademica = Validacao.TextoObrigatorio(pesquisador.FormacaoAcademica, "formação acadêmica", 3, 200));
            Validacao.Coletar(erros, () => pesquisador.AreaAtuacao = Validacao.TextoObrigatorio(pesquisador.AreaAtuacao, "área de atuação", 3, 150));
            Validacao.Coletar(erros, () => pesquisador.FotoUrl = Validacao.UrlOpcionalOuRelativa(pesquisador.FotoUrl, "foto"));
            Validacao.Coletar(erros, () => pesquisador.GoogleScholarUrl = Validacao.UrlOpcional(pesquisador.GoogleScholarUrl, "Google Scholar"));
            Validacao.Coletar(erros, () => pesquisador.OrcidUrl = Validacao.UrlOpcional(pesquisador.OrcidUrl, "ORCID"));
            Validacao.Coletar(erros, () => pesquisador.LattesUrl = Validacao.UrlOpcional(pesquisador.LattesUrl, "Currículo Lattes"));
            Validacao.LancarSeHouver(erros);

            if (_repository.EmailEmUso(pesquisador.Email, pesquisador.PessoaId > 0 ? (int?)pesquisador.PessoaId : null))
                throw new ValidacaoException("Já existe uma pessoa cadastrada com este e-mail.");

            if (pesquisador.PessoaId > 0)
            {
                _repository.Atualizar(pesquisador);
                return pesquisador.PessoaId;
            }

            pesquisador.DataCadastro = DateTime.Today;
            return _repository.Inserir(pesquisador);
        }

        public void Excluir(int pesquisadorId)
        {
            var vinculos = _repository.ObterVinculos(pesquisadorId);
            if (vinculos == null)
                throw new ValidacaoException("Pesquisador não encontrado.");

            if (vinculos.Total > 0)
            {
                var detalhes = new List<string>();
                if (vinculos.ProjetosCoordenados > 0) detalhes.Add(vinculos.ProjetosCoordenados + " projeto(s) coordenado(s)");
                if (vinculos.Participacoes > 0) detalhes.Add(vinculos.Participacoes + " participação(ões) em projeto");
                if (vinculos.Orientacoes > 0) detalhes.Add(vinculos.Orientacoes + " orientação(ões)");
                if (vinculos.Publicacoes > 0) detalhes.Add(vinculos.Publicacoes + " publicação(ões)");
                if (vinculos.Usuarios > 0) detalhes.Add(vinculos.Usuarios + " usuário(s) de acesso");

                throw new IntegridadeDadosException(
                    "Este pesquisador não pode ser excluído porque possui " + string.Join(", ", detalhes) +
                    ". Transfira ou remova esses vínculos primeiro.");
            }

            _repository.Excluir(pesquisadorId);
        }

        public List<OpcaoSelecao> ListarOpcoes()
        {
            return _repository.ListarAtivosResumo()
                .Select(p => new OpcaoSelecao { Id = p.PesquisadorId, Texto = p.NomeCompleto })
                .ToList();
        }

        public List<PesquisadorResumo> ListarAtivos()
        {
            return _repository.ListarAtivosResumo();
        }
    }
}
