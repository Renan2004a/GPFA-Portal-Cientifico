using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal.Admin
{
    public partial class Dashboard : AdminPageBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RegistrarEventoCiclo("Page_Init");
            if (!IsPostBack) ViewState["DiagnosticoCliques"] = 0;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                litNomeUsuario.Text = HttpUtility.HtmlEncode(UsuarioAtual.PrimeiroNome);
                CarregarIndicadores();
                CarregarProjetos();
                CarregarIntegridade();
                CarregarAuditoria();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            AtualizarDiagnostico();
        }

        protected void btnDiagnostico_Click(object sender, EventArgs e)
        {
            ViewState["DiagnosticoCliques"] = (int)(ViewState["DiagnosticoCliques"] ?? 0) + 1;
            RegistrarEventoCiclo("btnDiagnostico_Click");
            AtualizarDiagnostico();
        }

        private void CarregarIndicadores()
        {
            try
            {
                var ind = AppServices.Relatorios.ObterIndicadores();
                litPesquisadores.Text = ind.TotalPesquisadores.ToString();
                litEstudantes.Text = ind.TotalEstudantes.ToString();
                litEstudantesAtivos.Text = ind.TotalEstudantesAtivos.ToString();
                litProjetos.Text = ind.TotalProjetos.ToString();
                litProjetosAndamento.Text = ind.TotalProjetosEmAndamento.ToString();
                litProjetosAndamentoBar.Text = ind.TotalProjetosEmAndamento.ToString();
                litVerba.Text = ind.TotalCaptadoFormatado;
                litFinanciamentos.Text = ind.TotalFinanciamentos.ToString();
                litPublicacoes.Text = ind.TotalPublicacoes.ToString();
                var destaques = AppServices.Publicacoes.ListarDestaques().Count;
                litComDestaque.Text = destaques.ToString();

                var maximo = Math.Max(1, Math.Max(ind.TotalPublicacoes, Math.Max(destaques, ind.TotalProjetosEmAndamento)));
                divBarPublicacoes.Style["width"] = LarguraBarra(ind.TotalPublicacoes, maximo);
                divBarDestaques.Style["width"] = LarguraBarra(destaques, maximo);
                divBarProjetos.Style["width"] = LarguraBarra(ind.TotalProjetosEmAndamento, maximo);
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private static string LarguraBarra(int valor, int maximo)
        {
            if (valor <= 0) return "3%";
            var percentual = (double)valor / maximo * 100d;
            return Math.Max(4d, percentual).ToString("0.#", System.Globalization.CultureInfo.InvariantCulture) + "%";
        }

        private void CarregarProjetos()
        {
            try
            {
                var projetos = UsuarioAtual.EhAdministrador
                    ? AppServices.Projetos.Listar()
                    : UsuarioAtual.EhCoordenador
                        ? AppServices.Projetos.ListarGerenciaveis(UsuarioAtual)
                        : AppServices.Projetos.Listar();

                var recentes = projetos.Take(6).ToList();
                rptProjetos.DataSource = recentes;
                rptProjetos.DataBind();
                pnlSemProjetos.Visible = recentes.Count == 0;
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void CarregarIntegridade()
        {
            phIntegridade.Visible = UsuarioAtual.EhAdministrador;
            if (!UsuarioAtual.EhAdministrador) return;

            try
            {
                var alertas = AppServices.Relatorios.AlertasIntegridade();
                rptAlertas.DataSource = alertas;
                rptAlertas.DataBind();
                pnlIntegridadeOk.Visible = alertas.Count == 0;
                pnlComAlertas.Visible = alertas.Count > 0;
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void CarregarAuditoria()
        {
            phAuditoria.Visible = UsuarioAtual.EhAdministrador;
            if (!UsuarioAtual.EhAdministrador) return;

            try
            {
                gvAuditoria.DataSource = AppServices.Usuarios.ListarAuditoria(8);
                gvAuditoria.DataBind();
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void AtualizarDiagnostico()
        {
            litIsPostBack.Text = IsPostBack ? "true" : "false";
            litViewState.Text = ((int)(ViewState["DiagnosticoCliques"] ?? 0)).ToString();
            litSessaoItens.Text = Session.Count.ToString();

            var eventos = Session[SessionLifecycle] as List<string>;
            litSessaoEventos.Text = eventos == null ? "0" : eventos.Count.ToString();

            if (eventos == null || eventos.Count == 0)
            {
                litEventos.Text = string.Empty;
                return;
            }

            var html = new System.Text.StringBuilder();
            html.Append("<div class=\"tiny\" style=\"max-height:180px;overflow:auto;border-top:1px solid var(--line);padding-top:.6rem\">");
            foreach (var evento in eventos.Skip(Math.Max(0, eventos.Count - 7)))
            {
                html.Append("<div class=\"muted\" style=\"padding:.15rem 0\">");
                html.Append(HttpUtility.HtmlEncode(evento));
                html.Append("</div>");
            }
            html.Append("</div>");
            litEventos.Text = html.ToString();
        }
    }
}
