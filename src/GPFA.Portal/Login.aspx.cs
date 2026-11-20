using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using GPFA.Business;

namespace GPFA.Portal
{
    public partial class Login : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                pnlDemo.Visible = string.Equals(ConfigurationManager.AppSettings["DemoMode"], "true",
                    StringComparison.OrdinalIgnoreCase);

                if (Autenticado)
                {
                    Response.Redirect(ReturnUrlSeguro(Request.QueryString["returnUrl"], "~/Admin/Default.aspx"), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
        }

        protected void btnEntrar_Click(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("btnEntrar_Click");
            Page.Validate("login");
            if (!IsValid) return;

            try
            {
                var resultado = AppServices.Auth.Autenticar(txtLogin.Text, txtSenha.Text);
                if (!resultado.Sucesso)
                {
                    pnlErro.Visible = true;
                    litErro.Text = HttpUtility.HtmlEncode(resultado.Mensagem);
                    txtSenha.Text = string.Empty;
                    return;
                }

                Entrar(resultado.Usuario);
                var destino = ReturnUrlSeguro(Request.QueryString["returnUrl"], "~/Admin/Default.aspx");
                Response.Redirect(destino, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlErro.Visible = true;
                litErro.Text = HttpUtility.HtmlEncode(ErroPagina);
                ErroPagina = null;
            }
        }
    }
}
