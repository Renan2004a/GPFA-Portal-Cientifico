using System;
using System.Web;
using System.Web.Security;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal
{
    public class FlashMessage
    {
        public string Tipo { get; set; }
        public string Texto { get; set; }
    }

    /// <summary>
    /// Página base das páginas públicas e administrativas. Centraliza:
    /// - proteção CSRF com ViewStateUserKey;
    /// - leitura do usuário mantido em Session;
    /// - autorização por perfil;
    /// - mensagens flash baseadas em Session;
    /// - tratamento amigável de exceções de negócio.
    /// </summary>
    public class BasePage : System.Web.UI.Page
    {
        public const string SessionUsuario = "GPFA.UsuarioAtual";
        public const string SessionFlash = "GPFA.Flash";
        public const string SessionLifecycle = "GPFA.Lifecycle";

        public UsuarioSessao UsuarioAtual
        {
            get { return Session[SessionUsuario] as UsuarioSessao; }
        }

        public bool Autenticado
        {
            get { return Request.IsAuthenticated && UsuarioAtual != null; }
        }

        public string ErroPagina { get; set; }
        public string SucessoPagina { get; set; }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            // Mitiga CSRF: o ViewState só é aceito para a sessão que o emitiu.
            if (Session != null) ViewStateUserKey = Session.SessionID;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegistrarEventoCiclo("Page_PreRender");
        }

        protected void RegistrarEventoCiclo(string evento)
        {
            var lista = Session[SessionLifecycle] as System.Collections.Generic.List<string>;
            if (lista == null)
            {
                lista = new System.Collections.Generic.List<string>();
                Session[SessionLifecycle] = lista;
            }
            lista.Add(string.Format("{0:HH:mm:ss.fff} · {1} · {2}",
                DateTime.Now, evento, VirtualPathUtility.GetFileName(Request.Path)));
            if (lista.Count > 30) lista.RemoveAt(0);
        }

        public void ExigirLogin()
        {
            if (Autenticado) return;

            FormsAuthentication.SignOut();
            Session.Clear();
            var retorno = HttpUtility.UrlEncode(Request.RawUrl);
            Response.Redirect("~/Login.aspx?returnUrl=" + retorno, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        public void ExigirPerfis(params int[] perfis)
        {
            ExigirLogin();
            if (perfis == null || perfis.Length == 0) return;

            foreach (var perfil in perfis)
            {
                if (UsuarioAtual.PerfilId == perfil) return;
            }

            Response.Redirect("~/AcessoNegado.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        public void ExigirAdministrador()
        {
            ExigirPerfis(Dominio.Perfil.Administrador);
        }

        public void ExigirGestao()
        {
            ExigirPerfis(Dominio.Perfil.Administrador, Dominio.Perfil.Coordenador);
        }

        public bool EhAdministrador
        {
            get { return Autenticado && UsuarioAtual.EhAdministrador; }
        }

        public void Entrar(Usuario usuario)
        {
            var sessao = new UsuarioSessao
            {
                UsuarioId = usuario.UsuarioId,
                Login = usuario.Login,
                NomeCompleto = usuario.NomeCompleto,
                PerfilId = usuario.PerfilId,
                PerfilNome = usuario.PerfilNome,
                PesquisadorId = usuario.PesquisadorId
            };

            Session.Clear();
            Session[SessionUsuario] = sessao;
            FormsAuthentication.SetAuthCookie(usuario.Login, false);
        }

        public void Sair()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        public void Flash(string tipo, string mensagem)
        {
            Session[SessionFlash] = new FlashMessage { Tipo = tipo, Texto = mensagem };
        }

        public void Sucesso(string mensagem)
        {
            Flash("success", mensagem);
        }

        public void Informacao(string mensagem)
        {
            Flash("info", mensagem);
        }

        public FlashMessage ObterFlash()
        {
            var flash = Session[SessionFlash] as FlashMessage;
            Session.Remove(SessionFlash);
            return flash;
        }

        public void RedirecionarComSucesso(string url, string mensagem)
        {
            Sucesso(mensagem);
            // Redirecionamento com término imediato: evita que a página atual
            // consuma a mensagem flash antes de o navegador seguir para a próxima.
            Response.Redirect(url, true);
        }

        public void MostrarErro(string mensagem)
        {
            ErroPagina = mensagem;
        }

        public void MostrarErro(Exception ex)
        {
            if (ex is ValidacaoException)
            {
                ErroPagina = ex.Message;
                return;
            }
            if (ex is RegraNegocioException)
            {
                ErroPagina = ex.Message;
                return;
            }

            Global.RegistrarErro(Request.RawUrl, ex);
            ErroPagina = "Ocorreu um erro inesperado ao processar a solicitação. " +
                         "A equipe foi notificada pelo log do sistema. Tente novamente.";
        }

        public static string ReturnUrlSeguro(string valor, string padrao = "~/Default.aspx")
        {
            if (string.IsNullOrWhiteSpace(valor)) return padrao;
            if (valor.StartsWith("/") && !valor.StartsWith("//") && !valor.StartsWith("/\\"))
                return valor;
            return padrao;
        }

        /// <summary>
        /// Gera o HTML de um avatar com fallback para iniciais. Todo texto e URL
        /// é codificado antes de ser inserido no markup.
        /// </summary>
        public string AvatarHtml(object fotoUrl, object nome, string cssClass = "avatar")
        {
            var nomeTexto = nome == null ? string.Empty : nome.ToString();
            var iniciais = Iniciais(nomeTexto);
            var url = fotoUrl == null ? null : fotoUrl.ToString();

            if (!string.IsNullOrWhiteSpace(url))
            {
                var urlSegura = HttpUtility.HtmlAttributeEncode(ResolveUrl(url));
                return "<img class=\"" + HttpUtility.HtmlAttributeEncode(cssClass) + "\" src=\"" + urlSegura +
                       "\" alt=\"Foto de " + HttpUtility.HtmlAttributeEncode(nomeTexto) + "\" loading=\"lazy\" />";
            }

            return "<span class=\"" + HttpUtility.HtmlAttributeEncode(cssClass) + "\" aria-hidden=\"true\">" +
                   HttpUtility.HtmlEncode(iniciais) + "</span>";
        }

        public string Iniciais(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return "?";
            var partes = nome.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 1) return partes[0].Substring(0, 1).ToUpperInvariant();
            return (partes[0].Substring(0, 1) + partes[partes.Length - 1].Substring(0, 1)).ToUpperInvariant();
        }

        public string Moeda(decimal valor)
        {
            return valor.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
        }

        public string MoedaDetalhada(decimal valor)
        {
            return valor.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
        }

        public string DataCurta(DateTime? data)
        {
            return data.HasValue ? data.Value.ToString("dd/MM/yyyy") : "—";
        }

        public string MesAno(DateTime? data)
        {
            return data.HasValue ? data.Value.ToString("MM/yyyy") : "—";
        }

        public string UrlAttr(object url)
        {
            if (url == null || string.IsNullOrWhiteSpace(url.ToString())) return "#";
            return HttpUtility.HtmlAttributeEncode(ResolveUrl(url.ToString()));
        }

        public string TextoComParagrafos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
            var normalizado = texto.Replace("\r\n", "\n").Replace("\r", "\n");
            var blocos = normalizado.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var html = new System.Text.StringBuilder();
            foreach (var bloco in blocos)
            {
                var linhas = bloco.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < linhas.Length; i++)
                {
                    if (i > 0) html.Append("<br />");
                    html.Append(HttpUtility.HtmlEncode(linhas[i]));
                }
                html.Append("<br /><br />");
            }
            return html.ToString();
        }

        protected int ObterIdQueryString(string nome)
        {
            int id;
            return int.TryParse(Request.QueryString[nome], out id) && id > 0 ? id : 0;
        }
    }
}
