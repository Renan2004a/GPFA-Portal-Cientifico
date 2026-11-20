using System;
using System.Web;
using System.Web.UI;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal
{
    public partial class Pesquisador : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                var id = ObterIdQueryString("id");
                if (id == 0) { pnlNaoEncontrado.Visible = true; return; }
                Carregar(id);
            }
        }

        private void Carregar(int id)
        {
            try
            {
                var perfil = AppServices.Pesquisadores.ObterPerfil(id);
                if (perfil == null)
                {
                    pnlNaoEncontrado.Visible = true;
                    return;
                }

                var p = perfil.Pesquisador;
                pnlPerfil.Visible = true;
                Page.Title = p.NomeCompleto + " · Pesquisador";

                litFoto.Text = AvatarHtml(p.FotoUrl, p.NomeCompleto, "avatar avatar-xl");
                litNome.Text = HttpUtility.HtmlEncode(p.NomeCompleto);
                litArea.Text = HttpUtility.HtmlEncode(p.AreaAtuacao ?? "Pesquisador(a)");
                litFormacao.Text = HttpUtility.HtmlEncode(p.FormacaoAcademica);
                litBio.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(p.MiniBiografia)
                    ? "Este pesquisador ainda não cadastrou uma mini-biografia."
                    : p.MiniBiografia);
                litLinha.Text = HttpUtility.HtmlEncode(p.LinhaPesquisa ?? "Linha não informada");
                litDepartamento.Text = HttpUtility.HtmlEncode(p.Departamento ?? p.Instituicao ?? "GPFA");
                litInstituicao.Text = HttpUtility.HtmlEncode((p.Departamento ?? "") +
                    (string.IsNullOrWhiteSpace(p.Instituicao) ? "" : " · " + p.Instituicao));

                ConfigurarLink(lnkScholar, p.GoogleScholarUrl);
                ConfigurarLink(lnkOrcid, p.OrcidUrl);
                ConfigurarLink(lnkLattes, p.LattesUrl);
                lnkEmail.Visible = !string.IsNullOrWhiteSpace(p.Email);
                lnkEmail.NavigateUrl = "mailto:" + p.Email;
                lnkEmailContato.Visible = !string.IsNullOrWhiteSpace(p.Email);
                lnkEmailContato.NavigateUrl = "mailto:" + p.Email;

                litQtdProjetos.Text = p.QtdProjetos.ToString();
                litQtdCoordenados.Text = p.QtdProjetosCoordenados.ToString();
                litQtdOrientandos.Text = p.QtdOrientandosAtuais.ToString();
                litQtdEgressos.Text = p.QtdOrientandosPassados.ToString();
                litQtdPublicacoes.Text = p.QtdPublicacoes.ToString();
                litVerba.Text = Moeda(p.TotalCaptado);

                rptProjetos.DataSource = perfil.Projetos;
                rptProjetos.DataBind();
                pnlSemProjetos.Visible = perfil.Projetos.Count == 0;

                rptDestaques.DataSource = perfil.Destaques;
                rptDestaques.DataBind();
                pnlSemDestaques.Visible = perfil.Destaques.Count == 0;

                rptProjetosCoordenados.DataSource = perfil.ProjetosCoordenados;
                rptProjetosCoordenados.DataBind();
                pnlSemCoordenacao.Visible = perfil.ProjetosCoordenados.Count == 0;

                rptOrientandosAtuais.DataSource = perfil.OrientandosAtuais;
                rptOrientandosAtuais.DataBind();
                pnlSemOrientandosAtuais.Visible = perfil.OrientandosAtuais.Count == 0;

                rptOrientandosPassados.DataSource = perfil.OrientandosPassados;
                rptOrientandosPassados.DataBind();
                pnlSemOrientandosPassados.Visible = perfil.OrientandosPassados.Count == 0;

                rptPublicacoes.DataSource = perfil.Publicacoes;
                rptPublicacoes.DataBind();
                pnlSemPublicacoes.Visible = perfil.Publicacoes.Count == 0;
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlNaoEncontrado.Visible = true;
            }
        }

        private static void ConfigurarLink(System.Web.UI.WebControls.HyperLink link, string url)
        {
            link.Visible = !string.IsNullOrWhiteSpace(url);
            link.NavigateUrl = url;
        }
    }
}
