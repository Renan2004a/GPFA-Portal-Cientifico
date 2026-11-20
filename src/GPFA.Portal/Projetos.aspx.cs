using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal
{
    public partial class Projetos : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                CarregarFiltros();
                AplicarFiltroLinhaDaUrl();
                CarregarProjetos();
            }
        }

        private void CarregarFiltros()
        {
            ddlSituacao.Items.Clear();
            ddlSituacao.Items.Add(new ListItem("Todas as situações", ""));
            foreach (var situacao in Dominio.SituacaoProjeto.Todos)
                ddlSituacao.Items.Add(new ListItem(situacao, situacao));

            try
            {
                ddlLinha.Items.Clear();
                ddlLinha.Items.Add(new ListItem("Todas as linhas", ""));
                foreach (var linha in AppServices.Relatorios.LinhasPesquisa())
                    ddlLinha.Items.Add(new ListItem(linha, linha));
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void AplicarFiltroLinhaDaUrl()
        {
            var linha = Request.QueryString["linha"];
            if (string.IsNullOrWhiteSpace(linha)) return;
            var item = ddlLinha.Items.FindByValue(linha);
            if (item != null)
            {
                ddlLinha.ClearSelection();
                item.Selected = true;
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("btnFiltrar_Click");
            CarregarProjetos();
        }

        private void CarregarProjetos()
        {
            try
            {
                var lista = AppServices.Projetos.Listar(txtBusca.Text, ddlSituacao.SelectedValue,
                    null, null, ddlLinha.SelectedValue);
                rptProjetos.DataSource = lista;
                rptProjetos.DataBind();
                pnlVazio.Visible = lista.Count == 0;
                litResultado.Text = lista.Count == 1
                    ? "1 projeto encontrado"
                    : lista.Count + " projetos encontrados";
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                rptProjetos.DataSource = new List<ProjetoResumo>();
                rptProjetos.DataBind();
                pnlVazio.Visible = true;
                litResultado.Text = "Não foi possível carregar os projetos.";
            }
        }
    }
}
