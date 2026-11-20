using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GPFA.Data
{
    /// <summary>
    /// Fábrica central de conexões. A cadeia de conexão é configurada uma vez
    /// (Global.asax no portal ou fixture de testes) e nunca fica hard-coded nas
    /// camadas.
    /// </summary>
    public static class Database
    {
        private static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_connectionString))
                {
                    var configured = ConfigurationManager.ConnectionStrings["GPFA"];
                    if (configured == null || string.IsNullOrWhiteSpace(configured.ConnectionString))
                        throw new InvalidOperationException(
                            "A cadeia de conexão 'GPFA' não foi configurada no Web.config.");
                    _connectionString = configured.ConnectionString;
                }
                return _connectionString;
            }
        }

        public static void Configure(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Cadeia de conexão inválida.", "connectionString");
            _connectionString = connectionString;
        }

        public static SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }

    internal static class DbValue
    {
        public static string String(SqlDataReader r, string column)
        {
            int i = r.GetOrdinal(column);
            return r.IsDBNull(i) ? null : r.GetString(i);
        }

        public static string StringTrim(SqlDataReader r, string column)
        {
            var value = String(r, column);
            return value == null ? null : value.Trim();
        }

        public static int Int(SqlDataReader r, string column)
        {
            int i = r.GetOrdinal(column);
            return r.IsDBNull(i) ? 0 : Convert.ToInt32(r.GetValue(i));
        }

        public static int? IntNull(SqlDataReader r, string column)
        {
            int i = r.GetOrdinal(column);
            return r.IsDBNull(i) ? (int?)null : Convert.ToInt32(r.GetValue(i));
        }

        public static bool Bool(SqlDataReader r, string column)
        {
            int i = r.GetOrdinal(column);
            return !r.IsDBNull(i) && r.GetBoolean(i);
        }

        public static decimal Decimal(SqlDataReader r, string column)
        {
            int i = r.GetOrdinal(column);
            return r.IsDBNull(i) ? 0m : r.GetDecimal(i);
        }

        public static DateTime Date(SqlDataReader r, string column)
        {
            int i = r.GetOrdinal(column);
            return r.IsDBNull(i) ? DateTime.MinValue : r.GetDateTime(i);
        }

        public static DateTime? DateNull(SqlDataReader r, string column)
        {
            int i = r.GetOrdinal(column);
            return r.IsDBNull(i) ? (DateTime?)null : r.GetDateTime(i);
        }

        public static char Char(SqlDataReader r, string column)
        {
            var s = String(r, column);
            return string.IsNullOrEmpty(s) ? ' ' : s[0];
        }

        public static object OrDBNull(object value)
        {
            return value ?? DBNull.Value;
        }

        public static object OrDBNull(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        }

        public static object OrDBNull(DateTime? value)
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        public static object OrDBNull(int? value)
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }
    }
}
