using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GPFA.Data
{
    /// <summary>
    /// Pequeno helper ADO.NET. Todas as consultas usam SqlParameter — não há
    /// concatenação de valores de entrada em comandos SQL.
    /// </summary>
    public static class SqlHelper
    {
        public static SqlParameter Param(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        public static List<T> Query<T>(string sql, Func<SqlDataReader, T> mapper,
            params SqlParameter[] parameters)
        {
            return Query(sql, mapper, CommandType.Text, parameters);
        }

        public static List<T> Query<T>(string sql, Func<SqlDataReader, T> mapper,
            CommandType commandType, params SqlParameter[] parameters)
        {
            var list = new List<T>();
            try
            {
                using (var connection = Database.OpenConnection())
                using (var command = BuildCommand(connection, null, sql, commandType, parameters))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(mapper(reader));
                }
            }
            catch (SqlException ex)
            {
                throw SqlErrorTranslator.Translate(ex);
            }
            return list;
        }

        public static T QuerySingle<T>(string sql, Func<SqlDataReader, T> mapper,
            params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = Database.OpenConnection())
                using (var command = BuildCommand(connection, null, sql, CommandType.Text, parameters))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        return mapper(reader);
                }
            }
            catch (SqlException ex)
            {
                throw SqlErrorTranslator.Translate(ex);
            }
            return default(T);
        }

        public static object Scalar(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = Database.OpenConnection())
                using (var command = BuildCommand(connection, null, sql, CommandType.Text, parameters))
                {
                    return command.ExecuteScalar();
                }
            }
            catch (SqlException ex)
            {
                throw SqlErrorTranslator.Translate(ex);
            }
        }

        public static int ScalarInt(string sql, params SqlParameter[] parameters)
        {
            var value = Scalar(sql, parameters);
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        public static decimal ScalarDecimal(string sql, params SqlParameter[] parameters)
        {
            var value = Scalar(sql, parameters);
            return value == null || value == DBNull.Value ? 0m : Convert.ToDecimal(value);
        }

        public static int Execute(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = Database.OpenConnection())
                using (var command = BuildCommand(connection, null, sql, CommandType.Text, parameters))
                {
                    return command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw SqlErrorTranslator.Translate(ex);
            }
        }

        /// <summary>
        /// Executa uma unidade de trabalho em transação. A ação recebe a conexão e
        /// a transação e deve usar <see cref="Command"/> para criar comandos.
        /// </summary>
        public static void InTransaction(Action<SqlConnection, SqlTransaction> action)
        {
            try
            {
                using (var connection = Database.OpenConnection())
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        action(connection, transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        try { transaction.Rollback(); } catch { }
                        throw;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw SqlErrorTranslator.Translate(ex);
            }
        }

        public static SqlCommand Command(SqlConnection connection, SqlTransaction transaction,
            string sql, params SqlParameter[] parameters)
        {
            return BuildCommand(connection, transaction, sql, CommandType.Text, parameters);
        }

        private static SqlCommand BuildCommand(SqlConnection connection, SqlTransaction transaction,
            string sql, CommandType commandType, SqlParameter[] parameters)
        {
            var command = new SqlCommand(sql, connection);
            command.CommandType = commandType;
            command.CommandTimeout = 60;
            if (transaction != null) command.Transaction = transaction;
            if (parameters != null && parameters.Length > 0)
            {
                foreach (var parameter in parameters)
                {
                    if (parameter.Value == null) parameter.Value = DBNull.Value;
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }
    }
}
