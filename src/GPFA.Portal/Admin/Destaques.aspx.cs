using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Portal.Admin
{
    public partial class DestaquesAdmin : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ExigirGestao();
            if (UsuarioAtual == null || !UsuarioAtual.PodeGerenciarProjetos) return;

            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                CarregarPesquisadores();
                CarregarConteudo();
            }
        }

        private void CarregarPesquisadores()
        {
            try
            {
                ddlPesquisador.Items.Clear();
                var lista = UsuarioAtual.EhAdministrador
                    ? AppServices.Pesquisadores.ListarAtivos()
                    : AppServices.Pesquisadores.ListarAtivos()
                        .Where(p => p.PesquisadorId == UsuarioAtual.PesquisadorId).ToList();

                foreach (var p in lista)
                    ddlPesquisador.Items.Add(new ListItem(p.NomeCompleto, p.PesquisadorId.ToString()));

                var query = Request.QueryString["pesquisador"];
                if (!string.IsNullOrWhiteSpace(query))
                {
                    var item = ddlPesquisador.Items.FindByValue(query);
                    if (item != null) { ddlPesquisador.ClearSelection(); item.Selected = true; }
                }

                if (ddlPesquisador.Items.Count == 0)
                {
                    pnlConteudo.Visible = false;
                    MostrarErro("Nenhum pesquisador disponível para gerenciar destaques.");
                    return;
                }
                ddlPesquisador.Enabled = ddlPesquisador.Items.Count > 1;
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected void ddlPesquisador_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarConteudo();
        }

        private void CarregarConteudo()
        {
            int pesquisadorId;
            if (!int.TryParse(ddlPesquisador.SelectedValue, out pesquisadorId) || pesquisadorId <= 0)
            {
                pnlConteudo.Visible = false;
                return;
            }

            if (!UsuarioAtual.EhAdministrador && UsuarioAtual.PesquisadorId != pesquisadorId)
            {
                MostrarErro("Você só pode gerenciar os destaques do seu próprio perfil.");
                pnlConteudo.Visible = false;
                return;
            }

            try
            {
                pnlConteudo.Visible = true;
                var destaques = AppServices.Publicacoes.ListarDestaques(pesquisadorId);
                gvDestaques.DataSource = destaques;
                gvDestaques.DataBind();
                litContagem.Text = destaques.Count.ToString();

                ddlPublicacao.Items.Clear();
                foreach (var pub in AppServices.Publicacoes.ListarPublicacoesDoPesquisador(pesquisadorId))
                    ddlPublicacao.Items.Add(new ListItem(pub.Ano + " · " + pub.Titulo, pub.PublicacaoId.ToString()));

                ddlOrdem.Items.Clear();
                for (int i = 1; i <= 5; i++)
                {
                    var ocupada = destaques.Any(d => d.Ordem == i);
                    ddlOrdem.Items.Add(new ListItem(ocupada ? i + "º (ocupada)" : i + "º", i.ToString()));
                }

                if (ddlPublicacao.Items.Count == 0)
                {
                    ddlPublicacao.Items.Add(new ListItem("Nenhuma publicação cadastrada para este pesquisador", ""));
                    btnAdicionar.Enabled = false;
                }
                else
                {
                    btnAdicionar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
            }
        }

        protected void btnAdicionar_Click(object sender, EventArgs e)
        {
            try
            {
                var pesquisadorId = int.Parse(ddlPesquisador.SelectedValue);
                var publicacaoId = int.Parse(ddlPublicacao.SelectedValue);
                var ordem = int.Parse(ddlOrdem.SelectedValue);
                AppServices.Publicacoes.AdicionarDestaque(pesquisadorId, publicacaoId, ordem, UsuarioAtual);
                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Criar", "Destaque", publicacaoId,
                    "Destaque adicionado na posição " + ordem + ".");
                RedirecionarComSucesso("~/Admin/Destaques.aspx?pesquisador=" + pesquisadorId, "Destaque adicionado com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                CarregarConteudo();
            }
        }

        protected void gvDestaques_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Remover") return;
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            try
            {
                AppServices.Publicacoes.RemoverDestaque(id, UsuarioAtual);
                AppServices.Usuarios.RegistrarAuditoria(UsuarioAtual, "Excluir", "Destaque", id,
                    "Destaque removido.");
                RedirecionarComSucesso("~/Admin/Destaques.aspx?pesquisador=" + ddlPesquisador.SelectedValue,
                    "Destaque removido com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex);
                CarregarConteudo();
            }
        }
    }
}
