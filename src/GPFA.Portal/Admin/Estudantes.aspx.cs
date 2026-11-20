using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal.Admin
{
    public partial class EstudantesAdmin : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ExigirAdministrador();
            if (!EhAdministrador) return;

            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                CarregarOpcoes();
                CarregarLista();
            }
        }

        private void CarregarOpcoes()
        {
            ddlNivel.Items.Clear();
            foreach (var nivel in Dominio.NivelAcademico.Todos)
                ddlNivel.Items.Add(new ListItem(nivel, nivel));

            ddlSituacao.Items.Clear();
            foreach (var situacao in Dominio.SituacaoEstudante.Todas)
                ddlSituacao.Items.Add(new ListItem(situacao, situacao));

            try
            {
                ddlOrientador.Items.Clear();
                ddlOrientador.Items.Add(new ListItem("Selecione o orientador...", ""));
                foreach (var p in AppServices.Pesquisadores.ListarAtivos())
                    ddlOrientador.Items.Add(new ListItem(p.NomeCompleto + " · " + p.AreaAtuacao, p.PesquisadorId.ToString()));

                cblProjetos.Items.Clear();
                foreach (var projeto in AppServices.Projetos.Listar())
                {
                    if (projeto.Situacao == Dominio.SituacaoProjeto.Concluido) continue;
                    cblProjetos.Items.Add(new ListItem(
                        (projeto.Sigla ?? "Projeto") + " — " + projeto.Titulo, projeto.ProjetoId.ToString()));
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
                var lista = AppServices.Estudantes.Listar(null, null, null, null);
                gvEstudantes.DataSource = lista;
                gvEstudantes.DataBind();
                litTotal.Text = lista.Count + " registro(s).";
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected void gvEstudantes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEstudantes.PageIndex = e.NewPageIndex;
            CarregarLista();
        }

        protected void gvEstudantes_RowCommand(object sender, GridViewCommandEventArgs e)
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
                    AppServices.Estudantes.Excluir(id);
                    AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Excluir", "Estudante", id,
                        "Estudante excluído pelo painel, com orientações e vínculos.");
                    RedirecionarComSucesso(Request.RawUrl, "Estudante excluído com sucesso.");
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
            litTituloForm.Text = "Novo estudante";
            pnlForm.Visible = true;
            ddlSituacao.SelectedValue = Dominio.SituacaoEstudante.Ativo;
            ddlSituacao.Enabled = false;
            txtConclusao.Enabled = false;
            cblProjetos.Visible = true;
            pnlProjetosEdicao.Visible = false;
            txtNome.Focus();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            LimparFormulario();
        }

        protected void cvProjetos_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (int.Parse(hdnId.Value) > 0)
            {
                args.IsValid = true;
                return;
            }
            args.IsValid = cblProjetos.Items.Cast<ListItem>().Any(i => i.Selected);
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                int orientadorId;
                if (!int.TryParse(ddlOrientador.SelectedValue, out orientadorId) || orientadorId <= 0)
                    throw new ValidacaoException("Selecione o orientador principal.");

                var estudante = new Estudante
                {
                    PessoaId = int.Parse(hdnId.Value),
                    NomeCompleto = txtNome.Text,
                    Email = txtEmail.Text,
                    NivelAcademico = ddlNivel.SelectedValue,
                    Curso = txtCurso.Text,
                    Situacao = ddlSituacao.SelectedValue,
                    DataIngresso = DateTime.Parse(txtIngresso.Text),
                    DataConclusao = string.IsNullOrWhiteSpace(txtConclusao.Text) ? (DateTime?)null : DateTime.Parse(txtConclusao.Text),
                    CoorientadoresExternos = txtCoorientadores.Text,
                    FormacaoAcademica = txtFormacao.Text,
                    AreaAtuacao = txtArea.Text,
                    FotoUrl = txtFoto.Text,
                    MiniBiografia = txtBio.Text,
                    Ativo = chkAtivo.Checked
                };

                bool novo = estudante.PessoaId == 0;
                int id;
                if (novo)
                {
                    var projetos = cblProjetos.Items.Cast<ListItem>()
                        .Where(i => i.Selected)
                        .Select(i => int.Parse(i.Value))
                        .ToList();
                    id = AppServices.Estudantes.Criar(estudante, orientadorId, projetos, txtTipoVinculo.Text);
                }
                else
                {
                    AppServices.Estudantes.Atualizar(estudante);
                    id = estudante.PessoaId;

                    var resumo = AppServices.Estudantes.ObterResumo(id);
                    if (resumo != null && resumo.OrientadorId != orientadorId)
                    {
                        AppServices.Estudantes.DefinirOrientadorPrincipal(id, orientadorId,
                            DateTime.Today, "Alteração de orientador realizada pelo painel administrativo.");
                    }
                }

                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, novo ? "Criar" : "Atualizar",
                    "Estudante", id, estudante.NomeCompleto);
                RedirecionarComSucesso(Request.RawUrl, novo ? "Estudante cadastrado com sucesso." : "Estudante atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                pnlForm.Visible = true;
            }
        }

        private void AbrirEdicao(int id)
        {
            try
            {
                var e = AppServices.Estudantes.ObterEntidade(id);
                var resumo = AppServices.Estudantes.ObterResumo(id);
                if (e == null) { MostrarErro("Estudante não encontrado."); return; }

                hdnId.Value = e.PessoaId.ToString();
                txtNome.Text = e.NomeCompleto;
                txtEmail.Text = e.Email;
                ddlNivel.SelectedValue = e.NivelAcademico;
                txtCurso.Text = e.Curso;
                ddlSituacao.SelectedValue = e.Situacao;
                txtIngresso.Text = e.DataIngresso.ToString("yyyy-MM-dd");
                txtConclusao.Text = e.DataConclusao.HasValue ? e.DataConclusao.Value.ToString("yyyy-MM-dd") : string.Empty;
                txtCoorientadores.Text = e.CoorientadoresExternos;
                txtFormacao.Text = e.FormacaoAcademica;
                txtArea.Text = e.AreaAtuacao;
                txtFoto.Text = e.FotoUrl;
                txtBio.Text = e.MiniBiografia;
                chkAtivo.Checked = e.Ativo;

                if (resumo != null && resumo.OrientadorId.HasValue)
                    ddlOrientador.SelectedValue = resumo.OrientadorId.Value.ToString();

                litTituloForm.Text = "Editar estudante";
                pnlForm.Visible = true;
                ddlSituacao.Enabled = true;
                txtConclusao.Enabled = true;
                cblProjetos.Visible = false;
                pnlProjetosEdicao.Visible = true;
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
            txtNome.Text = txtEmail.Text = txtCurso.Text = txtIngresso.Text = txtConclusao.Text = string.Empty;
            txtCoorientadores.Text = txtFormacao.Text = txtArea.Text = txtFoto.Text = txtBio.Text = string.Empty;
            txtTipoVinculo.Text = string.Empty;
            ddlNivel.SelectedIndex = 0;
            ddlOrientador.SelectedIndex = 0;
            ddlSituacao.SelectedValue = Dominio.SituacaoEstudante.Ativo;
            foreach (ListItem item in cblProjetos.Items) item.Selected = false;
            chkAtivo.Checked = true;
        }
    }
}
