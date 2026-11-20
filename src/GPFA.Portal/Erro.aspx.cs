using System;

namespace GPFA.Portal
{
    public partial class Erro : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.StatusCode = 500;
        }
    }
}
