using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal.Admin
{
    public partial class FontesFinanciadorasAdmin : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ExigirAdministrador();
            if (!EhAdministrador) return;

            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                ddlTipo.Items.Clear();
                foreach (var tipo in Dominio.TipoFonte.Todos)
                    ddlTipo.Items.Add(new ListItem(tipo, tipo));
                CarregarLista();
            }
        }

        private void CarregarLista()
        {
            try
            {
                gvFontes.DataSource = AppServices.Financiamentos.ListarFontes();
                gvFontes.DataBind();
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected void btnNova_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            litTituloForm.Text = "Nova fonte";
            pnlForm.Visible = true;
            txtNome.Focus();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            LimparFormulario();
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                var fonte = new FonteFinanciadora
                {
                    FonteId = int.Parse(hdnId.Value),
                    Nome = txtNome.Text,
                    Sigla = txtSigla.Text,
                    Tipo = ddlTipo.SelectedValue,
                    SiteUrl = txtSite.Text
                };

                bool nova = fonte.FonteId == 0;
                var id = AppServices.Financiamentos.SalvarFonte(fonte, UsuarioAtual);
                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, nova ? "Criar" : "Atualizar",
                    "FonteFinanciadora", id, fonte.Nome);
                RedirecionarComSucesso(Request.RawUrl, nova ? "Fonte cadastrada com sucesso." : "Fonte atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlForm.Visible = true;
            }
        }

        protected void gvFontes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            if (e.CommandName == "Editar")
            {
                var fonte = AppServices.Financiamentos.ObterFonte(id);
                if (fonte == null) { MostrarErro("Fonte não encontrada."); return; }
                hdnId.Value = fonte.FonteId.ToString();
                txtNome.Text = fonte.Nome;
                txtSigla.Text = fonte.Sigla;
                ddlTipo.SelectedValue = fonte.Tipo;
                txtSite.Text = fonte.SiteUrl;
                litTituloForm.Text = "Editar fonte";
                pnlForm.Visible = true;
                txtNome.Focus();
            }
            else if (e.CommandName == "Excluir")
            {
                try
                {
                    AppServices.Financiamentos.ExcluirFonte(id, UsuarioAtual);
                    AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Excluir", "FonteFinanciadora", id,
                        "Fonte excluída pelo painel.");
                    RedirecionarComSucesso(Request.RawUrl, "Fonte excluída com sucesso.");
                }
                catch (Exception ex)
                {
                    MostrarErro(ex);
                    CarregarLista();
                }
            }
        }

        private void LimparFormulario()
        {
            hdnId.Value = "0";
            txtNome.Text = txtSigla.Text = txtSite.Text = string.Empty;
            ddlTipo.SelectedIndex = 0;
        }
    }
}
