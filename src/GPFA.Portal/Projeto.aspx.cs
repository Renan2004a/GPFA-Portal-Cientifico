using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal
{
    public partial class Projeto : BasePage
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
                var detalhe = AppServices.Projetos.ObterDetalhe(id);
                if (detalhe == null)
                {
                    pnlNaoEncontrado.Visible = true;
                    return;
                }

                var p = detalhe.Projeto;
                pnlProjeto.Visible = true;
                Page.Title = p.Titulo;

                litTitulo.Text = HttpUtility.HtmlEncode(p.Titulo);
                litSigla.Text = HttpUtility.HtmlEncode(p.Sigla ?? "Projeto GPFA");
                litSiglaCrumb.Text = HttpUtility.HtmlEncode(p.Sigla ?? "Projeto");
                litResumo.Text = HttpUtility.HtmlEncode(p.Resumo);
                litLinha.Text = HttpUtility.HtmlEncode(p.LinhaPesquisa ?? "Pesquisa");
                litLinhaSidebar.Text = HttpUtility.HtmlEncode(p.LinhaPesquisa ?? "Não informada");
                litDescricao.Text = TextoComParagrafos(string.IsNullOrWhiteSpace(p.Descricao) ? p.Resumo : p.Descricao);

                spnStatus.Attributes["class"] = "status-pill " + p.SituacaoCss;
                spnStatus.InnerText = p.Situacao;

                imgProjeto.ImageUrl = string.IsNullOrWhiteSpace(p.ImagemUrl)
                    ? ResolveUrl("~/Content/img/projetos/materiais-quanticos.svg")
                    : ResolveUrl(p.ImagemUrl);

                litSituacao.Text = HttpUtility.HtmlEncode(p.Situacao);
                litInicio.Text = DataCurta(p.DataInicio);
                litFim.Text = p.DataFim.HasValue ? DataCurta(p.DataFim) : (p.DataFimPrevista.HasValue ? "previsão " + DataCurta(p.DataFimPrevista) : "em aberto");
                litQtdPesquisadores.Text = p.QtdPesquisadores.ToString();
                litQtdEstudantes.Text = p.QtdEstudantes.ToString();
                litQtdPublicacoes.Text = p.QtdPublicacoes.ToString();
                litTotal.Text = Moeda(p.TotalCaptado);

                if (detalhe.Coordenador != null)
                {
                    litCoordenadorAvatar.Text = AvatarHtml(detalhe.Coordenador.FotoUrl, detalhe.Coordenador.NomeCompleto, "avatar");
                    litCoordenadorNome.Text = HttpUtility.HtmlEncode(detalhe.Coordenador.NomeCompleto);
                    litCoordenadorFormacao.Text = HttpUtility.HtmlEncode(detalhe.Coordenador.FormacaoAcademica);
                    hdnCoordenadorId.Value = detalhe.Coordenador.PesquisadorId.ToString();
                }

                var participacoes = AppServices.Projetos.ListarParticipacoes(id)
                    .Where(x => x.Ativa).ToList();
                rptParticipantes.DataSource = participacoes;
                rptParticipantes.DataBind();
                pnlSemParticipantes.Visible = participacoes.Count == 0;

                rptEstudantes.DataSource = detalhe.Estudantes;
                rptEstudantes.DataBind();
                pnlSemEstudantes.Visible = detalhe.Estudantes.Count == 0;

                rptPublicacoes.DataSource = detalhe.Publicacoes;
                rptPublicacoes.DataBind();
                pnlSemPublicacoes.Visible = detalhe.Publicacoes.Count == 0;

                rptFinanciamentos.DataSource = detalhe.Financiamentos;
                rptFinanciamentos.DataBind();
                pnlSemFinanciamentos.Visible = detalhe.Financiamentos.Count == 0;

                rptFontes.DataSource = detalhe.Financiamentos;
                rptFontes.DataBind();
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlNaoEncontrado.Visible = true;
            }
        }
    }
}
