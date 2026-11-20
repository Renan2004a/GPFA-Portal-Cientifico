using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal.Admin
{
    public partial class FinanciamentosAdmin : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ExigirGestao();
            if (UsuarioAtual == null || !UsuarioAtual.PodeGerenciarProjetos) return;

            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                CarregarOpcoes();
                CarregarLista();
            }
        }

        private void CarregarOpcoes()
        {
            try
            {
                var projetos = UsuarioAtual.EhAdministrador
                    ? AppServices.Projetos.Listar()
                    : AppServices.Projetos.ListarGerenciaveis(UsuarioAtual);

                ddlFiltroProjeto.Items.Clear();
                ddlFiltroProjeto.Items.Add(new ListItem("Todos os projetos", ""));
                ddlProjeto.Items.Clear();
                ddlProjeto.Items.Add(new ListItem("Selecione o projeto...", ""));
                foreach (var p in projetos)
                {
                    var texto = (p.Sigla ?? "Projeto") + " — " + p.Titulo;
                    ddlFiltroProjeto.Items.Add(new ListItem(texto, p.ProjetoId.ToString()));
                    ddlProjeto.Items.Add(new ListItem(texto, p.ProjetoId.ToString()));
                }

                var fontes = AppServices.Financiamentos.ListarFontes();
                ddlFiltroFonte.Items.Clear();
                ddlFiltroFonte.Items.Add(new ListItem("Todas as fontes", ""));
                ddlFonte.Items.Clear();
                ddlFonte.Items.Add(new ListItem("Selecione a fonte...", ""));
                foreach (var fonte in fontes)
                {
                    var texto = (fonte.Sigla ?? fonte.Nome) + " — " + fonte.Nome;
                    ddlFiltroFonte.Items.Add(new ListItem(texto, fonte.FonteId.ToString()));
                    ddlFonte.Items.Add(new ListItem(texto, fonte.FonteId.ToString()));
                }
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void CarregarLista()
        {
            try
            {
                int? projetoId = LerInteiro(ddlFiltroProjeto.SelectedValue);
                int? fonteId = LerInteiro(ddlFiltroFonte.SelectedValue);
                var lista = AppServices.Financiamentos.ListarGerenciaveis(UsuarioAtual, projetoId, fonteId, txtBusca.Text);
                gvFinanciamentos.DataSource = lista;
                gvFinanciamentos.DataBind();

                var total = lista.Sum(f => f.Valor);
                litTotal.Text = lista.Count + " registro(s) · total exibido " + Moeda(total) + ".";
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            gvFinanciamentos.PageIndex = 0;
            CarregarLista();
        }

        protected void gvFinanciamentos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFinanciamentos.PageIndex = e.NewPageIndex;
            CarregarLista();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            litTituloForm.Text = "Novo financiamento";
            pnlForm.Visible = true;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            LimparFormulario();
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                decimal valor;
                if (!TryParseValor(txtValor.Text, out valor))
                    throw new ValidacaoException("Informe o valor em reais, por exemplo 120000,00.");

                var financiamento = new Financiamento
                {
                    FinanciamentoId = int.Parse(hdnId.Value),
                    ProjetoId = int.Parse(ddlProjeto.SelectedValue),
                    FonteId = int.Parse(ddlFonte.SelectedValue),
                    Descricao = txtDescricao.Text,
                    NumeroProcesso = txtProcesso.Text,
                    Valor = valor,
                    DataInicio = DateTime.Parse(txtInicio.Text),
                    DataFim = string.IsNullOrWhiteSpace(txtFim.Text) ? (DateTime?)null : DateTime.Parse(txtFim.Text),
                    Moeda = "BRL"
                };

                bool novo = financiamento.FinanciamentoId == 0;
                var id = AppServices.Financiamentos.Salvar(financiamento, UsuarioAtual);
                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, novo ? "Criar" : "Atualizar",
                    "Financiamento", id, financiamento.Descricao + " · " + financiamento.Valor.ToString("C2"));
                RedirecionarComSucesso(Request.RawUrl, novo ? "Financiamento registrado com sucesso." : "Financiamento atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlForm.Visible = true;
            }
        }

        protected void gvFinanciamentos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            if (e.CommandName == "Editar")
            {
                AbrirEdicao(id);
            }
            else if (e.CommandName == "Excluir")
            {
                try
                {
                    AppServices.Financiamentos.Excluir(id, UsuarioAtual);
                    AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Excluir", "Financiamento", id,
                        "Financiamento excluído pelo painel.");
                    RedirecionarComSucesso(Request.RawUrl, "Financiamento excluído com sucesso.");
                }
                catch (Exception ex)
                {
                    MostrarErro(ex);
                    CarregarLista();
                }
            }
        }

        private void AbrirEdicao(int id)
        {
            try
            {
                var f = AppServices.Financiamentos.Obter(id);
                if (f == null) { MostrarErro("Financiamento não encontrado."); return; }

                hdnId.Value = f.FinanciamentoId.ToString();
                if (ddlProjeto.Items.FindByValue(f.ProjetoId.ToString()) != null)
                    ddlProjeto.SelectedValue = f.ProjetoId.ToString();
                ddlFonte.SelectedValue = f.FonteId.ToString();
                txtDescricao.Text = f.Descricao;
                txtProcesso.Text = f.NumeroProcesso;
                txtValor.Text = f.Valor.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
                txtInicio.Text = f.DataInicio.ToString("yyyy-MM-dd");
                txtFim.Text = f.DataFim.HasValue ? f.DataFim.Value.ToString("yyyy-MM-dd") : string.Empty;

                litTituloForm.Text = "Editar financiamento";
                pnlForm.Visible = true;
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void LimparFormulario()
        {
            hdnId.Value = "0";
            if (ddlProjeto.Items.Count > 0) ddlProjeto.SelectedIndex = 0;
            if (ddlFonte.Items.Count > 0) ddlFonte.SelectedIndex = 0;
            txtDescricao.Text = txtProcesso.Text = txtFim.Text = string.Empty;
            txtValor.Text = string.Empty;
            txtInicio.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }

        private static bool TryParseValor(string texto, out decimal valor)
        {
            var culturas = new[] { CultureInfo.GetCultureInfo("pt-BR"), CultureInfo.InvariantCulture };
            foreach (var cultura in culturas)
            {
                if (decimal.TryParse(texto, NumberStyles.Number, cultura, out valor))
                    return true;
            }
            valor = 0;
            return false;
        }

        private static int? LerInteiro(string valor)
        {
            int numero;
            return int.TryParse(valor, out numero) && numero > 0 ? (int?)numero : null;
        }
    }
}
