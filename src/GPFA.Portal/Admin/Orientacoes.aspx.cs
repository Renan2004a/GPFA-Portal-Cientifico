using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal.Admin
{
    public partial class OrientacoesAdmin : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ExigirAdministrador();
            if (!EhAdministrador) return;

            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                CarregarOpcoes();
                CarregarLista();
            }
        }

        private void CarregarOpcoes()
        {
            try
            {
                ddlEstudante.Items.Clear();
                ddlEstudante.Items.Add(new ListItem("Selecione o estudante...", ""));
                foreach (var estudante in AppServices.Estudantes.Listar(null, null, null, null))
                    ddlEstudante.Items.Add(new ListItem(estudante.NomeCompleto + " · " + estudante.NivelAcademico,
                        estudante.EstudanteId.ToString()));

                ddlOrientador.Items.Clear();
                ddlOrientador.Items.Add(new ListItem("Selecione o orientador...", ""));
                foreach (var p in AppServices.Pesquisadores.ListarAtivos())
                    ddlOrientador.Items.Add(new ListItem(p.NomeCompleto, p.PesquisadorId.ToString()));
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void CarregarLista()
        {
            try
            {
                var todas = AppServices.Estudantes.ListarTodas(txtBusca.Text, null, ddlVinculo.SelectedValue);
                gvOrientacoes.DataSource = todas;
                gvOrientacoes.DataBind();
                litTotal.Text = todas.Count + " registro(s).";
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected void btnNova_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = true;
            pnlEncerrar.Visible = false;
            ddlEstudante.SelectedIndex = 0;
            ddlOrientador.SelectedIndex = 0;
            ddlTipo.SelectedValue = "Principal";
            txtInicio.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtObservacao.Text = string.Empty;
        }

        protected void btnFecharForm_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
        }

        protected void btnFecharEncerrar_Click(object sender, EventArgs e)
        {
            pnlEncerrar.Visible = false;
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            gvOrientacoes.PageIndex = 0;
            CarregarLista();
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                var estudanteId = int.Parse(ddlEstudante.SelectedValue);
                var orientadorId = int.Parse(ddlOrientador.SelectedValue);
                var inicio = DateTime.Parse(txtInicio.Text);

                if (ddlTipo.SelectedValue == "Principal")
                {
                    AppServices.Estudantes.DefinirOrientadorPrincipal(estudanteId, orientadorId, inicio, txtObservacao.Text);
                }
                else
                {
                    AppServices.Estudantes.AdicionarCoorientacaoInterna(estudanteId, orientadorId, inicio, txtObservacao.Text);
                }

                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Criar", "Orientacao", null,
                    "Orientação registrada para o estudante " + ddlEstudante.SelectedItem.Text);
                RedirecionarComSucesso(Request.RawUrl, "Orientação registrada com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlForm.Visible = true;
            }
        }

        protected void gvOrientacoes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOrientacoes.PageIndex = e.NewPageIndex;
            CarregarLista();
        }

        protected void gvOrientacoes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            if (e.CommandName == "Encerrar")
            {
                var orientacao = AppServices.Estudantes.ListarTodas().FirstOrDefault(o => o.OrientacaoId == id);
                if (orientacao == null) { MostrarErro("Orientação não encontrada."); return; }
                hdnOrientacaoId.Value = id.ToString();
                litEncerrarInfo.Text = HttpUtility.HtmlEncode(
                    orientacao.EstudanteNome + " · orientação com " + orientacao.OrientadorNome +
                    " (início " + orientacao.DataInicio.ToString("dd/MM/yyyy") + ")");
                txtFim.Text = DateTime.Today.ToString("yyyy-MM-dd");
                pnlEncerrar.Visible = true;
                pnlForm.Visible = false;
            }
            else if (e.CommandName == "Excluir")
            {
                try
                {
                    AppServices.Estudantes.ExcluirOrientacao(id);
                    AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Excluir", "Orientacao", id,
                        "Registro de orientação excluído.");
                    RedirecionarComSucesso(Request.RawUrl, "Orientação excluída com sucesso.");
                }
                catch (Exception ex)
                {
                    MostrarErro(ex);
                    CarregarLista();
                }
            }
        }

        protected void btnEncerrar_Click(object sender, EventArgs e)
        {
            try
            {
                var id = int.Parse(hdnOrientacaoId.Value);
                var fim = DateTime.Parse(txtFim.Text);
                AppServices.Estudantes.EncerrarOrientacao(id, fim);
                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Encerrar", "Orientacao", id,
                    "Orientação encerrada em " + fim.ToString("dd/MM/yyyy") + ".");
                RedirecionarComSucesso(Request.RawUrl, "Orientação encerrada com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlEncerrar.Visible = true;
            }
        }
    }
}
