using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal
{
    public partial class Estudantes : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                CarregarFiltros();
                CarregarEstudantes();
            }
        }

        private void CarregarFiltros()
        {
            ddlNivel.Items.Clear();
            ddlNivel.Items.Add(new ListItem("Todos os níveis", ""));
            foreach (var nivel in Dominio.NivelAcademico.Todos)
                ddlNivel.Items.Add(new ListItem(nivel, nivel));

            ddlSituacao.Items.Clear();
            ddlSituacao.Items.Add(new ListItem("Todas as situações", ""));
            foreach (var situacao in Dominio.SituacaoEstudante.Todas)
                ddlSituacao.Items.Add(new ListItem(situacao, situacao));

            try
            {
                ddlOrientador.Items.Clear();
                ddlOrientador.Items.Add(new ListItem("Todos os orientadores", ""));
                foreach (var pesquisador in AppServices.Pesquisadores.ListarAtivos())
                    ddlOrientador.Items.Add(new ListItem(pesquisador.NomeCompleto, pesquisador.PesquisadorId.ToString()));
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("btnFiltrar_Click");
            CarregarEstudantes();
        }

        private void CarregarEstudantes()
        {
            try
            {
                int orientadorId;
                int? filtroOrientador = int.TryParse(ddlOrientador.SelectedValue, out orientadorId) && orientadorId > 0
                    ? (int?)orientadorId : null;

                var lista = AppServices.Estudantes.Listar(txtBusca.Text, ddlNivel.SelectedValue,
                    ddlSituacao.SelectedValue, filtroOrientador);

                rptEstudantes.DataSource = lista;
                rptEstudantes.DataBind();
                pnlVazio.Visible = lista.Count == 0;
                litResultado.Text = lista.Count == 1
                    ? "1 estudante encontrado"
                    : lista.Count + " estudantes encontrados";
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                rptEstudantes.DataSource = new List<EstudanteResumo>();
                rptEstudantes.DataBind();
                pnlVazio.Visible = true;
                litResultado.Text = "Não foi possível carregar o diretório.";
            }
        }
    }
}
