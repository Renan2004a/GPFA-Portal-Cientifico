using System;
using System.Web;
using System.Web.UI;
using GPFA.Models;

namespace GPFA.Portal.Admin
{
    public partial class AdminMaster : MasterPage
    {
        public UsuarioSessao Usuario
        {
            get
            {
                var page = Page as BasePage;
                return page == null ? null : page.UsuarioAtual;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = Usuario;
            bool admin = usuario != null && usuario.EhAdministrador;
            bool gestao = usuario != null && usuario.PodeGerenciarProjetos;

            phGestao.Visible = gestao;
            phCatalogo.Visible = admin;

            litPerfilTopo.Text = HttpUtility.HtmlEncode(usuario == null ? "sessão não identificada" : usuario.PerfilNome);
        }

        public string AdminNavClass(string pagina)
        {
            var atual = System.IO.Path.GetFileName(Request.AppRelativeCurrentExecutionFilePath);
            return string.Equals(atual, pagina, StringComparison.OrdinalIgnoreCase)
                ? "active" : string.Empty;
        }
    }
}
