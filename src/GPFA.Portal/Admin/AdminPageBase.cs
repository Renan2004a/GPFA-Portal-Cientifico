using System;

namespace GPFA.Portal.Admin
{
    /// <summary>
    /// Base das páginas do painel. Exige sessão autenticada e mantém o usuário
    /// disponível para as páginas e para a Master Page aninhada.
    /// </summary>
    public class AdminPageBase : BasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Autenticado)
            {
                ExigirLogin();
                return;
            }
            RegistrarEventoCiclo("Page_Load administrativo");
        }

        protected void ExigirPesquisadorProprio(int pesquisadorId)
        {
            if (UsuarioAtual == null) { ExigirLogin(); return; }
            if (UsuarioAtual.EhAdministrador) return;
            if (UsuarioAtual.PesquisadorId == pesquisadorId) return;
            Response.Redirect("~/AcessoNegado.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
