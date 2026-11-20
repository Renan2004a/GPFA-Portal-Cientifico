using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal.Admin
{
    public partial class ParticipacoesAdmin : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ExigirGestao();
            if (UsuarioAtual == null || !UsuarioAtual.PodeGerenciarProjetos) return;

            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                CarregarProjetos();
                SelecionarProjetoDaUrl();
                CarregarProjetoSelecionado();
            }
        }

        private void CarregarProjetos()
        {
            try
            {
                ddlProjeto.Items.Clear();
                ddlProjeto.Items.Add(new ListItem("Selecione um projeto...", ""));
                foreach (var projeto in AppServices.Projetos.ListarGerenciaveis(UsuarioAtual))
                    ddlProjeto.Items.Add(new ListItem((projeto.Sigla ?? "Projeto") + " — " + projeto.Titulo,
                        projeto.ProjetoId.ToString()));

                pnlSemProjeto.Visible = ddlProjeto.Items.Count <= 1;
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void SelecionarProjetoDaUrl()
        {
            var query = Request.QueryString["projeto"];
            if (!string.IsNullOrWhiteSpace(query))
            {
                var item = ddlProjeto.Items.FindByValue(query);
                if (item != null)
                {
                    ddlProjeto.ClearSelection();
                    item.Selected = true;
                }
            }
            if (ddlProjeto.SelectedIndex <= 0 && ddlProjeto.Items.Count > 1)
                ddlProjeto.SelectedIndex = 1;
        }

        protected void ddlProjeto_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarProjetoSelecionado();
        }

        private void CarregarProjetoSelecionado()
        {
            int id;
            if (!int.TryParse(ddlProjeto.SelectedValue, out id) || id <= 0)
            {
                pnlConteudo.Visible = false;
                return;
            }

            if (!AppServices.Projetos.PodeGerenciar(UsuarioAtual, id))
            {
                MostrarErro("Você não possui permissão para gerenciar este projeto.");
                pnlConteudo.Visible = false;
                return;
            }

            try
            {
                var projeto = AppServices.Projetos.ObterResumo(id);
                if (projeto == null) { pnlConteudo.Visible = false; return; }

                pnlConteudo.Visible = true;
                litProjetoTitulo.Text = HttpUtility.HtmlEncode(projeto.Titulo);
                litProjetoInfo.Text = HttpUtility.HtmlEncode(projeto.Sigla + " · " + projeto.PeriodoFormatado + " · " + projeto.Situacao);
                litCoordenador.Text = HttpUtility.HtmlEncode(projeto.CoordenadorNome);

                var participacoes = AppServices.Projetos.ListarParticipacoes(id);
                gvParticipantes.DataSource = participacoes;
                gvParticipantes.DataBind();
                litQtdParticipantes.Text = participacoes.Count + " registro(s)";

                var vinculos = AppServices.Projetos.ListarVinculosEstudantes(id);
                gvEstudantes.DataSource = vinculos;
                gvEstudantes.DataBind();
                litQtdEstudantes.Text = vinculos.Count + " registro(s)";

                CarregarOpcoesParticipantes(id, projeto.CoordenadorId);
                txtEntrada.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtDataSaida.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        private void CarregarOpcoesParticipantes(int projetoId, int coordenadorId)
        {
            var idsParticipantes = AppServices.Projetos.ListarParticipacoes(projetoId)
                .Select(p => p.PesquisadorId).ToList();

            ddlPesquisador.Items.Clear();
            ddlPesquisador.Items.Add(new ListItem("Selecione o pesquisador...", ""));
            foreach (var p in AppServices.Pesquisadores.ListarAtivos())
            {
                if (p.PesquisadorId == coordenadorId) continue;
                if (idsParticipantes.Contains(p.PesquisadorId)) continue;
                ddlPesquisador.Items.Add(new ListItem(p.NomeCompleto, p.PesquisadorId.ToString()));
            }

            var idsEstudantes = AppServices.Projetos.ListarVinculosEstudantes(projetoId)
                .Select(v => v.EstudanteId).ToList();

            ddlEstudante.Items.Clear();
            ddlEstudante.Items.Add(new ListItem("Selecione o estudante...", ""));
            foreach (var e in AppServices.Estudantes.Listar(null, null, Dominio.SituacaoEstudante.Ativo, null))
            {
                if (idsEstudantes.Contains(e.EstudanteId)) continue;
                ddlEstudante.Items.Add(new ListItem(e.NomeCompleto + " · " + e.NivelAcademico, e.EstudanteId.ToString()));
            }
        }

        protected void btnAdicionarParticipante_Click(object sender, EventArgs e)
        {
            try
            {
                var projetoId = int.Parse(ddlProjeto.SelectedValue);
                var pesquisadorId = int.Parse(ddlPesquisador.SelectedValue);
                var entrada = DateTime.Parse(txtEntrada.Text);
                AppServices.Projetos.AdicionarParticipante(projetoId, pesquisadorId, txtPapel.Text, entrada, UsuarioAtual);
                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Criar", "Participacao", projetoId,
                    "Participante adicionado ao projeto.");
                RedirecionarComSucesso("~/Admin/Participacoes.aspx?projeto=" + projetoId, "Participante adicionado com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                CarregarProjetoSelecionado();
            }
        }

        protected void btnVincularEstudante_Click(object sender, EventArgs e)
        {
            try
            {
                var projetoId = int.Parse(ddlProjeto.SelectedValue);
                var estudanteId = int.Parse(ddlEstudante.SelectedValue);
                var entrada = DateTime.Parse(txtEntradaEstudante.Text);
                AppServices.Projetos.VincularEstudante(projetoId, estudanteId, entrada, txtTipoVinculo.Text, UsuarioAtual);
                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Criar", "EstudanteProjeto", projetoId,
                    "Estudante vinculado ao projeto.");
                RedirecionarComSucesso("~/Admin/Participacoes.aspx?projeto=" + projetoId, "Estudante vinculado com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                CarregarProjetoSelecionado();
            }
        }

        protected void gvParticipantes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            if (e.CommandName == "Encerrar")
            {
                hdnTipo.Value = "P";
                hdnVinculoId.Value = id.ToString();
                litEncerrarInfo.Text = "Participação de pesquisador selecionada. Informe a data de saída.";
                txtDataSaida.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
            else if (e.CommandName == "Excluir")
            {
                try
                {
                    AppServices.Projetos.ExcluirParticipacao(id, UsuarioAtual);
                    AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Excluir", "Participacao", id,
                        "Participação removida.");
                    RedirecionarComSucesso(Request.RawUrl, "Participação removida com sucesso.");
                }
                catch (Exception ex)
                {
                    MostrarErro(ex);
                    CarregarProjetoSelecionado();
                }
            }
        }

        protected void gvEstudantes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            if (e.CommandName == "Encerrar")
            {
                hdnTipo.Value = "E";
                hdnVinculoId.Value = id.ToString();
                litEncerrarInfo.Text = "Vínculo de estudante selecionado. Informe a data de saída.";
                txtDataSaida.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
            else if (e.CommandName == "Excluir")
            {
                try
                {
                    AppServices.Projetos.ExcluirVinculoEstudante(id, UsuarioAtual);
                    AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Excluir", "EstudanteProjeto", id,
                        "Vínculo de estudante removido.");
                    RedirecionarComSucesso(Request.RawUrl, "Vínculo removido com sucesso.");
                }
                catch (Exception ex)
                {
                    MostrarErro(ex);
                    CarregarProjetoSelecionado();
                }
            }
        }

        protected void btnEncerrarVinculo_Click(object sender, EventArgs e)
        {
            try
            {
                var id = int.Parse(hdnVinculoId.Value);
                var data = DateTime.Parse(txtDataSaida.Text);

                if (hdnTipo.Value == "P")
                    AppServices.Projetos.EncerrarParticipacao(id, data, UsuarioAtual);
                else if (hdnTipo.Value == "E")
                    AppServices.Projetos.EncerrarVinculoEstudante(id, data, UsuarioAtual);
                else
                    throw new ValidacaoException("Selecione um vínculo na lista antes de encerrar.");

                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Encerrar", "VinculoProjeto", id,
                    "Vínculo encerrado em " + data.ToString("dd/MM/yyyy") + ".");
                RedirecionarComSucesso(Request.RawUrl, "Vínculo encerrado com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                CarregarProjetoSelecionado();
            }
        }
    }
}
