using System.Collections.Generic;
using System.Linq;
using GPFA.Data;
using GPFA.Models;

namespace GPFA.Business
{
    public class RelatorioService
    {
        private readonly RelatorioRepository _repository;
        private readonly FinanciamentoRepository _financiamentoRepository;

        public RelatorioService(RelatorioRepository repository, FinanciamentoRepository financiamentoRepository)
        {
            _repository = repository;
            _financiamentoRepository = financiamentoRepository;
        }

        public DashboardIndicadores ObterIndicadores()
        {
            return _repository.ObterIndicadores();
        }

        public List<RelatorioContagem> ProjetosPorSituacao()
        {
            var lista = _repository.ListarProjetosPorSituacao();
            var total = lista.Sum(x => x.Quantidade);
            foreach (var item in lista)
                item.Percentual = total == 0 ? 0 : (decimal)item.Quantidade / total * 100m;
            return lista;
        }

        public List<RelatorioContagem> PublicacoesPorAno()
        {
            return _repository.ListarPublicacoesPorAno();
        }

        public List<RelatorioContagem> PublicacoesPorPesquisador()
        {
            return _repository.ListarPublicacoesPorPesquisador();
        }

        public List<OrientadorContagem> OrientandosPorOrientador()
        {
            return _repository.ListarOrientandosPorOrientador();
        }

        public List<RelatorioContagem> FinanciamentosPorProjeto()
        {
            return _financiamentoRepository.ListarTotalPorProjeto();
        }

        public List<RelatorioContagem> FinanciamentosPorFonte()
        {
            var lista = _financiamentoRepository.ListarTotalPorFonte();
            var total = lista.Sum(x => x.Valor ?? 0m);
            foreach (var item in lista)
                item.Percentual = total == 0 ? 0 : (item.Valor ?? 0m) / total * 100m;
            return lista;
        }

        public List<RelatorioFinanceiroLinha> FinanciamentosPorPesquisador()
        {
            return _financiamentoRepository.ListarTotalPorPesquisador();
        }

        public List<AlertaIntegridade> AlertasIntegridade()
        {
            return _repository.ListarAlertasIntegridade();
        }

        public List<string> LinhasPesquisa()
        {
            return _repository.ListarLinhasPesquisa();
        }

        public List<string> AreasAtuacao()
        {
            return _repository.ListarAreasAtuacao();
        }
    }
}
