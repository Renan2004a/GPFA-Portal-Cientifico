using System;
using System.Collections.Generic;
using System.Linq;
using GPFA.Data;
using GPFA.Models;

namespace GPFA.Business
{
    public class FinanciamentoService
    {
        private readonly FinanciamentoRepository _repository;
        private readonly ProjetoRepository _projetoRepository;

        public FinanciamentoService(FinanciamentoRepository repository, ProjetoRepository projetoRepository)
        {
            _repository = repository;
            _projetoRepository = projetoRepository;
        }

        public List<Financiamento> Listar(int? projetoId = null, int? fonteId = null, string termo = null)
        {
            return _repository.Listar(projetoId, fonteId, termo);
        }

        public List<Financiamento> ListarGerenciaveis(UsuarioSessao usuario, int? projetoId = null,
            int? fonteId = null, string termo = null)
        {
            var lista = _repository.Listar(projetoId, fonteId, termo);
            if (usuario == null || usuario.EhAdministrador) return lista;
            if (!usuario.EhCoordenador || !usuario.PesquisadorId.HasValue) return new List<Financiamento>();

            var ids = _projetoRepository.ListarDoCoordenador(usuario.PesquisadorId.Value)
                .Select(p => p.ProjetoId).ToList();
            return lista.Where(f => ids.Contains(f.ProjetoId)).ToList();
        }

        public Financiamento Obter(int id)
        {
            return _repository.Obter(id);
        }

        public List<FinanciamentoResumo> ListarResumoPorProjeto(int projetoId)
        {
            return _repository.ListarResumoPorProjeto(projetoId);
        }

        public decimal TotalDoProjeto(int projetoId)
        {
            return _repository.TotalDoProjeto(projetoId);
        }

        public int Salvar(Financiamento financiamento, UsuarioSessao usuario)
        {
            if (financiamento == null)
                throw new ValidacaoException("Dados do financiamento não informados.");
            if (usuario == null || !usuario.PodeGerenciarProjetos)
                throw new RegraNegocioException("Você não possui permissão para gerenciar financiamentos.");

            var erros = new List<string>();
            Validacao.Coletar(erros, () => financiamento.Validar());
            Validacao.Coletar(erros, () => financiamento.Descricao = Validacao.TextoObrigatorio(financiamento.Descricao, "descrição", 3, 200));
            Validacao.Coletar(erros, () => financiamento.NumeroProcesso = string.IsNullOrWhiteSpace(financiamento.NumeroProcesso)
                ? null : Validacao.TextoObrigatorio(financiamento.NumeroProcesso, "número do processo", 2, 80));
            Validacao.Coletar(erros, () =>
            {
                if (financiamento.Valor <= 0)
                    throw new ValidacaoException("O valor do financiamento deve ser maior que zero (RN06).");
                if (financiamento.Valor > 9999999999999999m)
                    throw new ValidacaoException("O valor informado excede o limite permitido.");
            });
            Validacao.Coletar(erros, () =>
            {
                if (financiamento.DataInicio == DateTime.MinValue)
                    throw new ValidacaoException("Informe a data de início do financiamento.");
            });
            Validacao.LancarSeHouver(erros);

            if (!usuario.EhAdministrador)
            {
                var projeto = _projetoRepository.ObterResumo(financiamento.ProjetoId);
                if (projeto == null || !usuario.PesquisadorId.HasValue ||
                    projeto.CoordenadorId != usuario.PesquisadorId.Value)
                    throw new RegraNegocioException("Você só pode gerenciar financiamentos de projetos sob sua coordenação.");
            }

            financiamento.Moeda = "BRL";
            if (financiamento.FinanciamentoId > 0)
            {
                _repository.Atualizar(financiamento);
                return financiamento.FinanciamentoId;
            }
            return _repository.Inserir(financiamento);
        }

        public void Excluir(int financiamentoId, UsuarioSessao usuario)
        {
            var financiamento = _repository.Obter(financiamentoId);
            if (financiamento == null) throw new ValidacaoException("Financiamento não encontrado.");

            if (!usuario.EhAdministrador)
            {
                var projeto = _projetoRepository.ObterResumo(financiamento.ProjetoId);
                if (projeto == null || !usuario.PesquisadorId.HasValue ||
                    projeto.CoordenadorId != usuario.PesquisadorId.Value)
                    throw new RegraNegocioException("Você só pode excluir financiamentos de projetos sob sua coordenação.");
            }

            _repository.Excluir(financiamentoId);
        }

        public List<RelatorioContagem> TotalPorProjeto()
        {
            return _repository.ListarTotalPorProjeto();
        }

        public List<RelatorioContagem> TotalPorFonte()
        {
            return _repository.ListarTotalPorFonte();
        }

        public List<RelatorioFinanceiroLinha> TotalPorPesquisador()
        {
            return _repository.ListarTotalPorPesquisador();
        }

        /* -------------------------- Fontes financiadoras ------------------------- */

        public List<FonteFinanciadora> ListarFontes()
        {
            return _repository.ListarFontes();
        }

        public FonteFinanciadora ObterFonte(int id)
        {
            return _repository.ObterFonte(id);
        }

        public List<OpcaoSelecao> ListarOpcoesFontes()
        {
            return _repository.ListarOpcoesFontes();
        }

        public int SalvarFonte(FonteFinanciadora fonte, UsuarioSessao usuario)
        {
            if (usuario == null || !usuario.EhAdministrador)
                throw new RegraNegocioException("Somente o administrador pode gerenciar fontes financiadoras.");

            if (fonte == null)
                throw new ValidacaoException("Dados da fonte não informados.");

            var erros = new List<string>();
            Validacao.Coletar(erros, () => fonte.Validar());
            Validacao.Coletar(erros, () => fonte.Nome = Validacao.TextoObrigatorio(fonte.Nome, "nome", 3, 120));
            Validacao.Coletar(erros, () => fonte.Sigla = string.IsNullOrWhiteSpace(fonte.Sigla) ? null
                : Validacao.TextoObrigatorio(fonte.Sigla, "sigla", 1, 30));
            Validacao.Coletar(erros, () => fonte.SiteUrl = Validacao.UrlOpcional(fonte.SiteUrl, "site"));
            Validacao.LancarSeHouver(erros);

            if (fonte.FonteId > 0)
            {
                _repository.AtualizarFonte(fonte);
                return fonte.FonteId;
            }
            return _repository.InserirFonte(fonte);
        }

        public void ExcluirFonte(int fonteId, UsuarioSessao usuario)
        {
            if (usuario == null || !usuario.EhAdministrador)
                throw new RegraNegocioException("Somente o administrador pode excluir fontes financiadoras.");

            var fonte = _repository.ObterFonte(fonteId);
            if (fonte == null) throw new ValidacaoException("Fonte não encontrada.");
            if (fonte.QtdFinanciamentos > 0)
                throw new IntegridadeDadosException(
                    "Esta fonte não pode ser excluída porque possui " + fonte.QtdFinanciamentos +
                    " financiamento(s) vinculado(s). Exclua ou transfira os registros primeiro.");

            _repository.ExcluirFonte(fonteId);
        }
    }
}
