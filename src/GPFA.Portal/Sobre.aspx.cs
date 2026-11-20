using System;
using System.Web.UI;
using GPFA.Business;

namespace GPFA.Portal
{
    public partial class Sobre : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarEventoCiclo("Page_Load");
            if (!IsPostBack)
            {
                try
                {
                    var indicadores = AppServices.Relatorios.ObterIndicadores();
                    litPesquisadores.Text = indicadores.TotalPesquisadores.ToString();
                    litEstudantes.Text = indicadores.TotalEstudantes.ToString();
                    litProjetos.Text = indicadores.TotalProjetos.ToString();
                    litPublicacoes.Text = indicadores.TotalPublicacoes.ToString();
                    litVerba.Text = indicadores.TotalCaptadoFormatado;
                }
                catch (Exception ex)
                {
                    MostrarErro(ex);
                }
            }
        }
    }
}
