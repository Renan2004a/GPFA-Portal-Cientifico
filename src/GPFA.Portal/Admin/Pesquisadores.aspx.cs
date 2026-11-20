using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;
using PesquisadorModel = GPFA.Models.Pesquisador;

namespace GPFA.Portal.Admin
{
    public partial class PesquisadoresAdmin : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ExigirAdministrador();
            if (!EhAdministrador) return;

            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack) CarregarLista();
        }

        private void CarregarLista()
        {
            try
            {
                var lista = AppServices.Pesquisadores.Listar(null, null, false);
                gvPesquisadores.DataSource = lista;
                gvPesquisadores.DataBind();
                litTotal.Text = lista.Count + " registro(s).";
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected void gvPesquisadores_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPesquisadores.PageIndex = e.NewPageIndex;
            CarregarLista();
        }

        protected void gvPesquisadores_RowCommand(object sender, GridViewCommandEventArgs e)
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
                    AppServices.Pesquisadores.Excluir(id);
                    AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Excluir", "Pesquisador", id,
                        "Pesquisador excluído pelo painel.");
                    RedirecionarComSucesso(Request.RawUrl, "Pesquisador excluído com sucesso.");
                }
                catch (Exception ex)
                {
                    MostrarErro(ex);
                    CarregarLista();
                }
            }
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            litTituloForm.Text = "Novo pesquisador";
            pnlForm.Visible = true;
            txtNome.Focus();
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
                var pesquisador = new PesquisadorModel
                {
                    PessoaId = int.Parse(hdnId.Value),
                    NomeCompleto = txtNome.Text,
                    Email = txtEmail.Text,
                    FormacaoAcademica = txtFormacao.Text,
                    AreaAtuacao = txtArea.Text,
                    Titulacao = txtTitulacao.Text,
                    Departamento = txtDepartamento.Text,
                    Instituicao = txtInstituicao.Text,
                    LinhaPesquisa = txtLinha.Text,
                    FotoUrl = txtFoto.Text,
                    MiniBiografia = txtBio.Text,
                    GoogleScholarUrl = txtScholar.Text,
                    OrcidUrl = txtOrcid.Text,
                    LattesUrl = txtLattes.Text,
                    Ativo = chkAtivo.Checked
                };

                bool novo = pesquisador.PessoaId == 0;
                var id = AppServices.Pesquisadores.Salvar(pesquisador);
                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, novo ? "Criar" : "Atualizar",
                    "Pesquisador", id, pesquisador.NomeCompleto);
                RedirecionarComSucesso(Request.RawUrl, novo ? "Pesquisador cadastrado com sucesso." : "Pesquisador atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlForm.Visible = true;
                if (litTituloForm.Text.Length == 0) litTituloForm.Text = "Editar pesquisador";
            }
        }

        private void AbrirEdicao(int id)
        {
            try
            {
                var p = AppServices.Pesquisadores.ObterEntidade(id);
                if (p == null) { MostrarErro("Pesquisador não encontrado."); return; }

                hdnId.Value = p.PessoaId.ToString();
                txtNome.Text = p.NomeCompleto;
                txtEmail.Text = p.Email;
                txtFormacao.Text = p.FormacaoAcademica;
                txtArea.Text = p.AreaAtuacao;
                txtTitulacao.Text = p.Titulacao;
                txtDepartamento.Text = p.Departamento;
                txtInstituicao.Text = p.Instituicao;
                txtLinha.Text = p.LinhaPesquisa;
                txtFoto.Text = p.FotoUrl;
                txtBio.Text = p.MiniBiografia;
                txtScholar.Text = p.GoogleScholarUrl;
                txtOrcid.Text = p.OrcidUrl;
                txtLattes.Text = p.LattesUrl;
                chkAtivo.Checked = p.Ativo;

                litTituloForm.Text = "Editar pesquisador";
                pnlForm.Visible = true;
                txtNome.Focus();
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void LimparFormulario()
        {
            hdnId.Value = "0";
            txtNome.Text = txtEmail.Text = txtFormacao.Text = txtArea.Text = string.Empty;
            txtTitulacao.Text = txtDepartamento.Text = txtInstituicao.Text = txtLinha.Text = string.Empty;
            txtFoto.Text = txtBio.Text = txtScholar.Text = txtOrcid.Text = txtLattes.Text = string.Empty;
            chkAtivo.Checked = true;
        }
    }
}
