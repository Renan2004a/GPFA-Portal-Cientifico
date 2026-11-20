using System;
using System.Web.UI;
using GPFA.Business;

namespace GPFA.Portal
{
    public partial class AlterarSenha : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("Page_Load");
            ExigirLogin();
        }

        protected void btnAlterar_Click(object sender, EventArgs e)
        {
            Page.Validate("senha");
            if (!IsValid) return;

            try
            {
                AppServices.Auth.AlterarSenha(UsuarioAtual.UsuarioId, txtAtual.Text, txtNova.Text, txtConfirmacao.Text);
                RedirecionarComSucesso("~/Admin/Default.aspx", "Senha alterada com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }
    }
}
