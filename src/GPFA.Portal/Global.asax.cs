using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using GPFA.Business;

namespace GPFA.Portal
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var cultura = CultureInfo.GetCultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = cultura;
            Thread.CurrentThread.CurrentUICulture = cultura;

            var connection = ConfigurationManager.ConnectionStrings["GPFA"];
            if (connection == null)
                throw new ConfigurationErrorsException("Cadeia de conexão 'GPFA' não configurada no Web.config.");

            AppServices.Initialize(connection.ConnectionString);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Reforça a codificação das respostas e evita sniffing de conteúdo.
            Response.Headers["X-Content-Type-Options"] = "nosniff";
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex == null) return;
            if (ex is System.Threading.ThreadAbortException) return;

            var contexto = Request != null ? Request.RawUrl : "(sem url)";
            RegistrarErro(contexto, ex);

            Server.ClearError();
            if (Request != null && !Request.Url.AbsolutePath.EndsWith("/Erro.aspx", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Erro.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        public static void RegistrarErro(string contexto, Exception ex)
        {
            try
            {
                var pasta = HttpContext.Current != null
                    ? HttpContext.Current.Server.MapPath("~/App_Data/logs")
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "logs");
                Directory.CreateDirectory(pasta);
                var arquivo = Path.Combine(pasta, "erros-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                var texto = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}{2}{3}{4}",
                    DateTime.Now, contexto, Environment.NewLine, ex, Environment.NewLine + new string('-', 80) + Environment.NewLine);
                lock (typeof(Global))
                {
                    File.AppendAllText(arquivo, texto);
                }
            }
            catch
            {
                // Falha de log nunca deve derrubar a aplicação.
            }
        }
    }
}
