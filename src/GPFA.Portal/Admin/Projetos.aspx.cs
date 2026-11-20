using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;
using ProjetoModel = GPFA.Models.Projeto;

namespace GPFA.Portal.Admin
{
    public partial class ProjetosAdmin : AdminPageBase
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
            ddlSituacao.Items.Clear();
            foreach (var situacao in Dominio.SituacaoProjeto.Todos)
                ddlSituacao.Items.Add(new ListItem(situacao, situacao));

            try
            {
                ddlCoordenador.Items.Clear();
                ddlCoordenador.Items.Add(new ListItem("Selecione o coordenador...", ""));

                var pesquisadores = UsuarioAtual.EhAdministrador
                    ? AppServices.Pesquisadores.ListarAtivos()
                    : AppServices.Pesquisadores.ListarAtivos().Where(p => p.PesquisadorId == UsuarioAtual.PesquisadorId).ToList();

                foreach (var p in pesquisadores)
                    ddlCoordenador.Items.Add(new ListItem(p.NomeCompleto, p.PesquisadorId.ToString()));
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
                var lista = AppServices.Projetos.ListarGerenciaveis(UsuarioAtual);
                gvProjetos.DataSource = lista;
                gvProjetos.DataBind();
                litTotal.Text = lista.Count + " projeto(s).";
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            litTituloForm.Text = "Novo projeto";
            pnlForm.Visible = true;

            if (!UsuarioAtual.EhAdministrador && UsuarioAtual.PesquisadorId.HasValue)
            {
                ddlCoordenador.SelectedValue = UsuarioAtual.PesquisadorId.Value.ToString();
                ddlCoordenador.Enabled = false;
                pnlCoordFixo.Visible = true;
            }
            else
            {
                ddlCoordenador.Enabled = true;
                pnlCoordFixo.Visible = false;
            }

            txtTitulo.Focus();
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
                var projeto = new ProjetoModel
                {
                    ProjetoId = int.Parse(hdnId.Value),
                    Titulo = txtTitulo.Text,
                    Sigla = txtSigla.Text,
                    Resumo = txtResumo.Text,
                    Descricao = txtDescricao.Text,
                    LinhaPesquisa = txtLinha.Text,
                    Situacao = ddlSituacao.SelectedValue,
                    DataInicio = DateTime.Parse(txtInicio.Text),
                    DataFimPrevista = string.IsNullOrWhiteSpace(txtFimPrevisto.Text) ? (DateTime?)null : DateTime.Parse(txtFimPrevisto.Text),
                    DataFim = string.IsNullOrWhiteSpace(txtFim.Text) ? (DateTime?)null : DateTime.Parse(txtFim.Text),
                    CoordenadorId = int.Parse(ddlCoordenador.SelectedValue),
                    ImagemUrl = txtImagem.Text,
                    Destaque = chkDestaque.Checked
                };

                if (!UsuarioAtual.EhAdministrador && UsuarioAtual.PesquisadorId.HasValue)
                    projeto.CoordenadorId = UsuarioAtual.PesquisadorId.Value;

                bool novo = projeto.ProjetoId == 0;
                var id = AppServices.Projetos.Salvar(projeto, UsuarioAtual);
                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, novo ? "Criar" : "Atualizar",
                    "Projeto", id, projeto.Titulo);
                RedirecionarComSucesso(Request.RawUrl, novo ? "Projeto criado com sucesso." : "Projeto atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlForm.Visible = true;
            }
        }

        protected void gvProjetos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProjetos.PageIndex = e.NewPageIndex;
            CarregarLista();
        }

        protected void gvProjetos_RowCommand(object sender, GridViewCommandEventArgs e)
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
                    AppServices.Projetos.Excluir(id, UsuarioAtual);
                    AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Excluir", "Projeto", id,
                        "Projeto excluído pelo painel.");
                    RedirecionarComSucesso(Request.RawUrl, "Projeto excluído com sucesso.");
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
                if (!AppServices.Projetos.PodeGerenciar(UsuarioAtual, id))
                {
                    MostrarErro("Você não possui permissão para editar este projeto.");
                    return;
                }

                var p = AppServices.Projetos.ObterEntidade(id);
                if (p == null) { MostrarErro("Projeto não encontrado."); return; }

                hdnId.Value = p.ProjetoId.ToString();
                txtTitulo.Text = p.Titulo;
                txtSigla.Text = p.Sigla;
                txtResumo.Text = p.Resumo;
                txtDescricao.Text = p.Descricao;
                txtLinha.Text = p.LinhaPesquisa;
                ddlSituacao.SelectedValue = p.Situacao;
                txtInicio.Text = p.DataInicio.ToString("yyyy-MM-dd");
                txtFimPrevisto.Text = p.DataFimPrevista.HasValue ? p.DataFimPrevista.Value.ToString("yyyy-MM-dd") : string.Empty;
                txtFim.Text = p.DataFim.HasValue ? p.DataFim.Value.ToString("yyyy-MM-dd") : string.Empty;
                txtImagem.Text = p.ImagemUrl;
                chkDestaque.Checked = p.Destaque;

                if (ddlCoordenador.Items.FindByValue(p.CoordenadorId.ToString()) != null)
                    ddlCoordenador.SelectedValue = p.CoordenadorId.ToString();
                ddlCoordenador.Enabled = UsuarioAtual.EhAdministrador;
                pnlCoordFixo.Visible = !UsuarioAtual.EhAdministrador;

                litTituloForm.Text = "Editar projeto";
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
            txtTitulo.Text = txtSigla.Text = txtResumo.Text = txtDescricao.Text = txtLinha.Text = string.Empty;
            txtInicio.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtFimPrevisto.Text = txtFim.Text = txtImagem.Text = string.Empty;
            ddlSituacao.SelectedValue = Dominio.SituacaoProjeto.EmAndamento;
            if (ddlCoordenador.Items.Count > 0) ddlCoordenador.SelectedIndex = 0;
            chkDestaque.Checked = false;
        }
    }
}
