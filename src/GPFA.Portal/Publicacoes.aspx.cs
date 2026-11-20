using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal
{
    public partial class Publicacoes : BasePage
    {
        private const int TamanhoPagina = 10;

        private int PaginaAtual
        {
            get { return ViewState["PaginaAtual"] == null ? 1 : (int)ViewState["PaginaAtual"]; }
            set { ViewState["PaginaAtual"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                CarregarFiltros();
                AplicarFiltrosDaUrl();
                PaginaAtual = 1;
                CarregarPublicacoes();
            }
        }

        private void CarregarFiltros()
        {
            ddlAno.Items.Clear();
            ddlAno.Items.Add(new ListItem("Todos os anos", ""));
            try
            {
                foreach (var ano in AppServices.Publicacoes.ListarAnos())
                    ddlAno.Items.Add(new ListItem(ano.ToString(), ano.ToString()));

                ddlPesquisador.Items.Clear();
                ddlPesquisador.Items.Add(new ListItem("Todos os pesquisadores", ""));
                foreach (var p in AppServices.Pesquisadores.ListarAtivos())
                    ddlPesquisador.Items.Add(new ListItem(p.NomeCompleto, p.PesquisadorId.ToString()));

                ddlProjeto.Items.Clear();
                ddlProjeto.Items.Add(new ListItem("Todos os projetos", ""));
                foreach (var projeto in AppServices.Projetos.Listar())
                    ddlProjeto.Items.Add(new ListItem((projeto.Sigla ?? "Projeto") + " — " + projeto.Titulo, projeto.ProjetoId.ToString()));
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void AplicarFiltrosDaUrl()
        {
            SelecionarSeExistir(ddlAno, Request.QueryString["ano"]);
            SelecionarSeExistir(ddlPesquisador, Request.QueryString["pesquisador"]);
            SelecionarSeExistir(ddlProjeto, Request.QueryString["projeto"]);
            if (!string.IsNullOrWhiteSpace(Request.QueryString["q"])) txtBusca.Text = Request.QueryString["q"];
        }

        private static void SelecionarSeExistir(DropDownList lista, string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return;
            var item = lista.Items.FindByValue(valor);
            if (item == null) return;
            lista.ClearSelection();
            item.Selected = true;
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("btnFiltrar_Click");
            PaginaAtual = 1;
            CarregarPublicacoes();
        }

        protected void lnkAnterior_Click(object sender, EventArgs e)
        {
            if (PaginaAtual > 1) PaginaAtual--;
            CarregarPublicacoes();
        }

        protected void lnkProxima_Click(object sender, EventArgs e)
        {
            PaginaAtual++;
            CarregarPublicacoes();
        }

        private void CarregarPublicacoes()
        {
            try
            {
                var filtro = new FiltroPublicacoes
                {
                    Ano = LerInteiro(ddlAno.SelectedValue),
                    PesquisadorId = LerInteiro(ddlPesquisador.SelectedValue),
                    ProjetoId = LerInteiro(ddlProjeto.SelectedValue),
                    Termo = txtBusca.Text
                };

                var dados = AppServices.Publicacoes.Listar(filtro);
                var paginado = new PagedDataSource
                {
                    DataSource = dados,
                    AllowPaging = true,
                    PageSize = TamanhoPagina,
                    CurrentPageIndex = Math.Max(0, PaginaAtual - 1)
                };

                int paginas = Math.Max(1, (int)Math.Ceiling((double)dados.Count / TamanhoPagina));
                if (PaginaAtual > paginas) { PaginaAtual = paginas; paginado.CurrentPageIndex = paginas - 1; }

                rptPublicacoes.DataSource = paginado;
                rptPublicacoes.DataBind();

                pnlVazio.Visible = dados.Count == 0;
                litResultado.Text = dados.Count == 1 ? "1 publicação encontrada" : dados.Count + " publicações encontradas";
                litPagina.Text = dados.Count == 0 ? string.Empty : "Página " + PaginaAtual + " de " + paginas;
                lnkAnterior.Visible = dados.Count > 0 && PaginaAtual > 1;
                lnkProxima.Visible = dados.Count > 0 && PaginaAtual < paginas;
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                rptPublicacoes.DataSource = new List<PublicacaoResumo>();
                rptPublicacoes.DataBind();
                pnlVazio.Visible = true;
                litResultado.Text = "Não foi possível carregar as publicações.";
            }
        }

        private static int? LerInteiro(string valor)
        {
            int numero;
            return int.TryParse(valor, out numero) && numero > 0 ? (int?)numero : null;
        }
    }
}
