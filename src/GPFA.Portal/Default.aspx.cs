using System;
using System.Collections.Generic;
using System.Web.UI;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                CarregarIndicadores();
                CarregarDestaques();
                CarregarPesquisadores();
                CarregarPublicacoes();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegistrarEventoCiclo("Page_PreRender");
        }

        private void CarregarIndicadores()
        {
            try
            {
                var indicadores = AppServices.Relatorios.ObterIndicadores();
                litPesquisadores.Text = indicadores.TotalPesquisadores.ToString();
                litEstudantes.Text = indicadores.TotalEstudantes.ToString();
                litEstudantesAtivos.Text = indicadores.TotalEstudantesAtivos.ToString();
                litProjetos.Text = indicadores.TotalProjetos.ToString();
                litProjetosAndamento.Text = indicadores.TotalProjetosEmAndamento.ToString();
                litPublicacoes.Text = indicadores.TotalPublicacoes.ToString();
                litVerbaHero.Text = indicadores.TotalCaptadoFormatado;
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void CarregarDestaques()
        {
            try
            {
                var projetos = AppServices.Projetos.ListarDestaques(3);
                rptProjetos.DataSource = projetos;
                rptProjetos.DataBind();
                pnlSemProjetos.Visible = projetos.Count == 0;
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void CarregarPesquisadores()
        {
            try
            {
                var pesquisadores = AppServices.Pesquisadores.Listar(null, null, true);
                var selecionados = new List<PesquisadorResumo>();
                for (int i = 0; i < pesquisadores.Count && i < 3; i++)
                    selecionados.Add(pesquisadores[i]);

                rptPesquisadores.DataSource = selecionados;
                rptPesquisadores.DataBind();
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void CarregarPublicacoes()
        {
            try
            {
                var publicacoes = AppServices.Publicacoes.ListarRecentes(4);
                rptPublicacoes.DataSource = publicacoes;
                rptPublicacoes.DataBind();
                pnlSemPublicacoes.Visible = publicacoes.Count == 0;
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }
    }
}
