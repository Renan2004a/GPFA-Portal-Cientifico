using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal.Admin
{
    public partial class UsuariosAdmin : AdminPageBase
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
                ddlPerfil.Items.Clear();
                foreach (var perfil in AppServices.Usuarios.ListarPerfis())
                    ddlPerfil.Items.Add(new ListItem(perfil.Nome, perfil.PerfilId.ToString()));

                ddlPesquisador.Items.Clear();
                ddlPesquisador.Items.Add(new ListItem("Sem vínculo (permitido apenas para administrador)", ""));
                foreach (var p in AppServices.Pesquisadores.ListarAtivos())
                    ddlPesquisador.Items.Add(new ListItem(p.NomeCompleto, p.PesquisadorId.ToString()));
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
                gvUsuarios.DataSource = AppServices.Usuarios.Listar();
                gvUsuarios.DataBind();
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            litTituloForm.Text = "Novo usuário";
            pnlForm.Visible = true;
            txtLogin.Focus();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            LimparFormulario();
        }

        protected void ddlPerfil_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Mantém o campo habilitado, pois administradores também podem ter vínculo.
            ddlPesquisador.Enabled = true;
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                var usuario = new Usuario
                {
                    UsuarioId = int.Parse(hdnId.Value),
                    Login = txtLogin.Text,
                    NomeCompleto = txtNome.Text,
                    Email = txtEmail.Text,
                    PerfilId = int.Parse(ddlPerfil.SelectedValue),
                    PesquisadorId = string.IsNullOrWhiteSpace(ddlPesquisador.SelectedValue)
                        ? (int?)null : int.Parse(ddlPesquisador.SelectedValue),
                    Ativo = chkAtivo.Checked
                };

                bool novo = usuario.UsuarioId == 0;
                var id = AppServices.Usuarios.Salvar(usuario, txtSenha.Text, UsuarioAtual);
                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, novo ? "Criar" : "Atualizar",
                    "Usuario", id, usuario.Login);
                RedirecionarComSucesso(Request.RawUrl, novo ? "Usuário cadastrado com sucesso." : "Usuário atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlForm.Visible = true;
            }
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            if (e.CommandName == "Editar")
            {
                var usuario = AppServices.Usuarios.Obter(id);
                if (usuario == null) { MostrarErro("Usuário não encontrado."); return; }

                hdnId.Value = usuario.UsuarioId.ToString();
                txtLogin.Text = usuario.Login;
                txtNome.Text = usuario.NomeCompleto;
                txtEmail.Text = usuario.Email;
                ddlPerfil.SelectedValue = usuario.PerfilId.ToString();
                ddlPesquisador.SelectedValue = usuario.PesquisadorId.HasValue ? usuario.PesquisadorId.Value.ToString() : "";
                chkAtivo.Checked = usuario.Ativo;
                txtSenha.Text = string.Empty;
                litTituloForm.Text = "Editar usuário";
                pnlForm.Visible = true;
                txtLogin.Focus();
            }
            else if (e.CommandName == "Excluir")
            {
                try
                {
                    AppServices.Usuarios.Excluir(id, UsuarioAtual);
                    AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Excluir", "Usuario", id,
                        "Usuário excluído pelo painel.");
                    RedirecionarComSucesso(Request.RawUrl, "Usuário excluído com sucesso.");
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
            txtLogin.Text = txtNome.Text = txtEmail.Text = txtSenha.Text = string.Empty;
            if (ddlPerfil.Items.Count > 0) ddlPerfil.SelectedIndex = 0;
            if (ddlPesquisador.Items.Count > 0) ddlPesquisador.SelectedIndex = 0;
            chkAtivo.Checked = true;
        }
    }
}
