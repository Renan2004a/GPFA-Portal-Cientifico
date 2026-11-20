using System;
using System.Web;
using System.Web.UI;
using GPFA.Models;

namespace GPFA.Portal
{
    public partial class SiteMaster : MasterPage
    {
        public UsuarioSessao Usuario
        {
            get
            {
                var page = Page as BasePage;
                return page == null ? null : page.UsuarioAtual;
            }
        }

        public bool AreaAdministrativa
        {
            get { return Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/Admin/", StringComparison.OrdinalIgnoreCase); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                litIniciaisUsuario.Text = HttpUtility.HtmlEncode(ObterIniciais(Usuario));
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var page = Page as BasePage;
            var usuario = page == null ? null : page.UsuarioAtual;
            pnlAutenticado.Visible = usuario != null;
            pnlAnonimo.Visible = usuario == null;

            if (usuario != null)
            {
                litUsuarioNome.Text = HttpUtility.HtmlEncode(usuario.PrimeiroNome);
                litUsuarioPerfil.Text = HttpUtility.HtmlEncode(usuario.PerfilNome);
                litIniciaisUsuario.Text = HttpUtility.HtmlEncode(ObterIniciais(usuario));
            }

            string tipo = null;
            string mensagem = null;

            if (page != null)
            {
                var flash = page.ObterFlash();
                if (flash != null)
                {
                    tipo = flash.Tipo;
                    mensagem = flash.Texto;
                }
                if (!string.IsNullOrWhiteSpace(page.ErroPagina))
                {
                    tipo = "danger";
                    mensagem = page.ErroPagina;
                }
                else if (!string.IsNullOrWhiteSpace(page.SucessoPagina))
                {
                    tipo = "success";
                    mensagem = page.SucessoPagina;
                }
            }

            if (!string.IsNullOrWhiteSpace(mensagem))
            {
                pnlFlash.Visible = true;
                var classe = "alert-gpfa alert-" + (string.IsNullOrWhiteSpace(tipo) ? "info" : tipo) + "-gpfa";
                divFlash.Attributes["class"] = classe;
                litFlash.Text = HttpUtility.HtmlEncode(mensagem);
            }
        }

        public string NavClass(string pagina)
        {
            if (AreaAdministrativa) return "nav-link";
            var atual = System.IO.Path.GetFileName(Request.AppRelativeCurrentExecutionFilePath);
            bool ativo = string.Equals(atual, pagina, StringComparison.OrdinalIgnoreCase);

            if (!ativo && string.Equals(pagina, "Pesquisadores.aspx", StringComparison.OrdinalIgnoreCase))
                ativo = string.Equals(atual, "Pesquisador.aspx", StringComparison.OrdinalIgnoreCase);
            if (!ativo && string.Equals(pagina, "Projetos.aspx", StringComparison.OrdinalIgnoreCase))
                ativo = string.Equals(atual, "Projeto.aspx", StringComparison.OrdinalIgnoreCase);

            return ativo ? "nav-link active" : "nav-link";
        }

        protected void lnkSair_Click(object sender, EventArgs e)
        {
            var page = Page as BasePage;
            if (page != null) page.Sair();
        }

        private static string ObterIniciais(UsuarioSessao usuario)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.NomeCompleto)) return "?";
            var partes = usuario.NomeCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 1) return partes[0].Substring(0, 1).ToUpperInvariant();
            return (partes[0].Substring(0, 1) + partes[partes.Length - 1].Substring(0, 1)).ToUpperInvariant();
        }
    }
}
