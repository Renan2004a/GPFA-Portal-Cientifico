using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal
{
    public partial class Pesquisadores : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                CarregarAreas();
                AplicarFiltrosDaUrl();
                CarregarPesquisadores();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegistrarEventoCiclo("Page_PreRender");
        }

        private void CarregarAreas()
        {
            try
            {
                ddlArea.Items.Clear();
                ddlArea.Items.Add(new System.Web.UI.WebControls.ListItem("Todas as áreas", ""));
                foreach (var area in AppServices.Relatorios.AreasAtuacao())
                    ddlArea.Items.Add(new System.Web.UI.WebControls.ListItem(area, area));
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void AplicarFiltrosDaUrl()
        {
            var area = Request.QueryString["area"];
            var termo = Request.QueryString["q"];
            if (!string.IsNullOrWhiteSpace(area))
            {
                var item = ddlArea.Items.FindByValue(area);
                if (item != null) ddlArea.ClearSelection();
                if (item != null) item.Selected = true;
            }
            if (!string.IsNullOrWhiteSpace(termo)) txtBusca.Text = termo;
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("btnFiltrar_Click");
            CarregarPesquisadores();
        }

        private void CarregarPesquisadores()
        {
            try
            {
                var lista = AppServices.Pesquisadores.Listar(txtBusca.Text, ddlArea.SelectedValue, true);
                rptPesquisadores.DataSource = lista;
                rptPesquisadores.DataBind();
                pnlVazio.Visible = lista.Count == 0;
                litResultado.Text = lista.Count == 1
                    ? "1 pesquisador encontrado"
                    : lista.Count + " pesquisadores encontrados";
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                rptPesquisadores.DataSource = new List<PesquisadorResumo>();
                rptPesquisadores.DataBind();
                pnlVazio.Visible = true;
                litResultado.Text = "Não foi possível carregar o diretório.";
            }
        }
    }
}
