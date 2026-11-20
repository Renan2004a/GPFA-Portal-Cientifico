using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal.Admin
{
    public partial class RelatoriosAdmin : AdminPageBase
    {
        private int _maxPublicacoesAno = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            ExigirLogin();
            if (!Autenticado) return;

            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                litAtualizado.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                CarregarRelatorios();
            }
        }

        private void CarregarRelatorios()
        {
            try
            {
                var ind = AppServices.Relatorios.ObterIndicadores();
                litProjetos.Text = ind.TotalProjetos.ToString();
                litProjetosAndamento.Text = ind.TotalProjetosEmAndamento.ToString();
                litPesquisadores.Text = ind.TotalPesquisadores.ToString();
                litEstudantes.Text = ind.TotalEstudantes.ToString();
                litEstudantesAtivos.Text = ind.TotalEstudantesAtivos.ToString();
                litTotalCaptado.Text = ind.TotalCaptadoFormatado;
                litQtdFinanciamentos.Text = ind.TotalFinanciamentos.ToString();

                var situacoes = AppServices.Relatorios.ProjetosPorSituacao();
                rptSituacao.DataSource = situacoes;
                rptSituacao.DataBind();

                var anos = AppServices.Relatorios.PublicacoesPorAno();
                _maxPublicacoesAno = Math.Max(1, anos.Count == 0 ? 1 : anos.Max(a => a.Quantidade));
                rptPublicacoesAno.DataSource = anos;
                rptPublicacoesAno.DataBind();

                var porProjeto = AppServices.Relatorios.FinanciamentosPorProjeto();
                rptFinProjeto.DataSource = porProjeto;
                rptFinProjeto.DataBind();
                litTotalProjetos.Text = Moeda(porProjeto.Sum(p => p.Valor ?? 0));

                var porFonte = AppServices.Relatorios.FinanciamentosPorFonte();
                rptFinFonte.DataSource = porFonte;
                rptFinFonte.DataBind();
                litTotalFontes.Text = Moeda(porFonte.Sum(f => f.Valor ?? 0));

                var porPesquisador = AppServices.Relatorios.FinanciamentosPorPesquisador();
                var agrupado = porPesquisador
                    .GroupBy(p => p.Grupo)
                    .Select(g => new RelatorioContagem
                    {
                        Rotulo = g.Key,
                        Quantidade = g.Select(x => x.Projeto).Distinct().Count(),
                        Valor = g.Sum(x => x.Valor)
                    })
                    .OrderByDescending(x => x.Valor)
                    .ToList();
                rptPesquisadorTotal.DataSource = agrupado;
                rptPesquisadorTotal.DataBind();

                rptPubPesquisador.DataSource = AppServices.Relatorios.PublicacoesPorPesquisador();
                rptPubPesquisador.DataBind();

                rptOrientadores.DataSource = AppServices.Relatorios.OrientandosPorOrientador();
                rptOrientadores.DataBind();

                var alertas = AppServices.Relatorios.AlertasIntegridade();
                rptAlertas.DataSource = alertas;
                rptAlertas.DataBind();
                pnlSemAlertas.Visible = alertas.Count == 0;
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected string CalcularLarguraAno(object quantidade)
        {
            int valor;
            if (quantidade == null || !int.TryParse(quantidade.ToString(), out valor)) return "0";
            var percentual = (double)valor / _maxPublicacoesAno * 100d;
            return percentual.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
