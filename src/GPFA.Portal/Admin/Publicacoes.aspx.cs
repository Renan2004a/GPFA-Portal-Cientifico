using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal.Admin
{
    public partial class PublicacoesAdmin : AdminPageBase
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
            ddlTipo.Items.Clear();
            foreach (var tipo in Dominio.TipoPublicacao.Todos)
                ddlTipo.Items.Add(new ListItem(tipo, tipo));

            try
            {
                ddlProjeto.Items.Clear();
                ddlProjeto.Items.Add(new ListItem("Sem projeto associado", ""));
                var projetos = UsuarioAtual.EhAdministrador
                    ? AppServices.Projetos.Listar()
                    : AppServices.Projetos.ListarGerenciaveis(UsuarioAtual);
                foreach (var projeto in projetos)
                    ddlProjeto.Items.Add(new ListItem((projeto.Sigla ?? "Projeto") + " — " + projeto.Titulo,
                        projeto.ProjetoId.ToString()));

                cblAutores.Items.Clear();
                foreach (var p in AppServices.Pesquisadores.ListarAtivos())
                    cblAutores.Items.Add(new ListItem(p.NomeCompleto, p.PesquisadorId.ToString()));
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
                var lista = AppServices.Publicacoes.ListarGerenciaveis(UsuarioAtual, txtBusca.Text);
                gvPublicacoes.DataSource = lista;
                gvPublicacoes.DataBind();
                litTotal.Text = lista.Count + " registro(s).";
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            gvPublicacoes.PageIndex = 0;
            CarregarLista();
        }

        protected void gvPublicacoes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPublicacoes.PageIndex = e.NewPageIndex;
            CarregarLista();
        }

        protected void btnNova_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            litTituloForm.Text = "Nova publicação";
            pnlForm.Visible = true;
            txtTitulo.Focus();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            LimparFormulario();
        }

        protected void cvAutores_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = cblAutores.Items.Cast<ListItem>().Any(i => i.Selected);
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                int ano;
                if (!int.TryParse(txtAno.Text, out ano))
                    throw new ValidacaoException("Informe um ano válido.");

                var publicacao = new Publicacao
                {
                    PublicacaoId = int.Parse(hdnId.Value),
                    Titulo = txtTitulo.Text,
                    Veiculo = txtVeiculo.Text,
                    Ano = ano,
                    Tipo = ddlTipo.SelectedValue,
                    Doi = txtDoi.Text,
                    LinkAcesso = txtLink.Text,
                    ProjetoId = string.IsNullOrWhiteSpace(ddlProjeto.SelectedValue)
                        ? (int?)null : int.Parse(ddlProjeto.SelectedValue),
                    Autores = cblAutores.Items.Cast<ListItem>()
                        .Where(i => i.Selected)
                        .Select(i => new PublicacaoAutor { PesquisadorId = int.Parse(i.Value) })
                        .ToList()
                };

                bool novo = publicacao.PublicacaoId == 0;
                var id = AppServices.Publicacoes.Salvar(publicacao, UsuarioAtual);
                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, novo ? "Criar" : "Atualizar",
                    "Publicacao", id, publicacao.Titulo);
                RedirecionarComSucesso(Request.RawUrl, novo ? "Publicação cadastrada com sucesso." : "Publicação atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlForm.Visible = true;
            }
        }

        protected void gvPublicacoes_RowCommand(object sender, GridViewCommandEventArgs e)
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
                    AppServices.Publicacoes.Excluir(id, UsuarioAtual);
                    AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Excluir", "Publicacao", id,
                        "Publicação excluída pelo painel.");
                    RedirecionarComSucesso(Request.RawUrl, "Publicação excluída com sucesso.");
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
                var p = AppServices.Publicacoes.ObterEntidade(id);
                if (p == null) { MostrarErro("Publicação não encontrada."); return; }

                hdnId.Value = p.PublicacaoId.ToString();
                txtTitulo.Text = p.Titulo;
                txtVeiculo.Text = p.Veiculo;
                txtAno.Text = p.Ano.ToString();
                ddlTipo.SelectedValue = p.Tipo;
                txtDoi.Text = p.Doi;
                txtLink.Text = p.LinkAcesso;

                if (p.ProjetoId.HasValue && ddlProjeto.Items.FindByValue(p.ProjetoId.Value.ToString()) != null)
                    ddlProjeto.SelectedValue = p.ProjetoId.Value.ToString();
                else
                    ddlProjeto.SelectedIndex = 0;

                foreach (ListItem item in cblAutores.Items)
                    item.Selected = p.Autores.Any(a => a.PesquisadorId.ToString() == item.Value);

                litTituloForm.Text = "Editar publicação";
                pnlForm.Visible = true;
                txtTitulo.Focus();
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void LimparFormulario()
        {
            hdnId.Value = "0";
            txtTitulo.Text = txtVeiculo.Text = txtDoi.Text = txtLink.Text = string.Empty;
            txtAno.Text = DateTime.Today.Year.ToString();
            ddlTipo.SelectedIndex = 0;
            if (ddlProjeto.Items.Count > 0) ddlProjeto.SelectedIndex = 0;
            foreach (ListItem item in cblAutores.Items) item.Selected = false;
        }
    }
}
