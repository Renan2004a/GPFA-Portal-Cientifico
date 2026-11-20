using System.Data.SqlClient;
using GPFA.Models;

namespace GPFA.Data
{
    /// <summary>
    /// Converte erros do SQL Server em exceções de negócio compreensíveis.
    /// Nenhuma mensagem técnica é exibida ao visitante.
    /// </summary>
    public static class SqlErrorTranslator
    {
        public static System.Exception Translate(SqlException ex)
        {
            switch (ex.Number)
            {
                case 547:
                    return new IntegridadeDadosException(
                        "A operação não pode ser concluída porque o registro está relacionado a outros dados. " +
                        "Remova ou transfira os vínculos antes de tentar novamente.", ex);
                case 2601:
                case 2627:
                    return new IntegridadeDadosException(
                        "Já existe um registro com os mesmos dados únicos (por exemplo, e-mail, login ou vínculo repetido). " +
                        "Revise as informações e tente novamente.", ex);
                case 515:
                    return new IntegridadeDadosException(
                        "Um campo obrigatório não foi informado. Verifique o formulário e tente novamente.", ex);
                case 8114:
                case 245:
                    return new IntegridadeDadosException(
                        "Um dos valores informados é inválido para o tipo de dado esperado.", ex);
                case 50020:
                case 50021:
                case 50030:
                case 50040:
                case 50041:
                case 50050:
                case 50010:
                case 50011:
                case 50012:
                    return new IntegridadeDadosException(ex.Message, ex);
                default:
                    if (ex.Number >= 50000)
                        return new IntegridadeDadosException(ex.Message, ex);

                    return new IntegridadeDadosException(
                        "Não foi possível concluir a operação no banco de dados. " +
                        "Tente novamente e, se o problema persistir, contate o administrador.", ex);
            }
        }
    }
}
