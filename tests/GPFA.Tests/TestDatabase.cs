using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using GPFA.Business;

namespace GPFA.Tests
{
    /// <summary>
    /// Prepara um banco isolado (GPFA_Portal_Test) executando os mesmos scripts
    /// SQL versionados no repositório. Assim os testes exercitam o schema real,
    /// incluindo constraints, índices filtrados e triggers.
    /// </summary>
    internal static class TestDatabase
    {
        public const string NomeBanco = "GPFA_Portal_Test";
        private const string Servidor = @"(localdb)\MSSQLLocalDB";

        public static string ConnectionString
        {
            get
            {
                return "Data Source=" + Servidor + ";Initial Catalog=" + NomeBanco +
                       ";Integrated Security=True;MultipleActiveResultSets=True;Application Name=GPFA-Tests";
            }
        }

        public static string MasterConnectionString
        {
            get
            {
                return "Data Source=" + Servidor + ";Initial Catalog=master;Integrated Security=True;Application Name=GPFA-Tests";
            }
        }

        public static void Preparar()
        {
            var raiz = LocalizarRaiz();
            var pasta = Path.Combine(raiz, "database");

            using (var conexao = new SqlConnection(MasterConnectionString))
            {
                conexao.Open();

                Executar(conexao, "IF DB_ID(N'" + NomeBanco + "') IS NOT NULL BEGIN ALTER DATABASE [" + NomeBanco +
                    "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [" + NomeBanco + "]; END");

                foreach (var arquivo in new[]
                {
                    "00-create-database.sql",
                    "01-schema.sql",
                    "02-views.sql",
                    "03-seed.sql",
                    "04-seed-usuarios.sql"
                })
                {
                    var caminho = Path.Combine(pasta, arquivo);
                    if (!File.Exists(caminho))
                        throw new FileNotFoundException("Script SQL não encontrado: " + caminho);

                    var conteudo = File.ReadAllText(caminho);
                    conteudo = Regex.Replace(conteudo, @"(?m)^\s*:setvar.*$", string.Empty);
                    conteudo = conteudo.Replace("$(DbName)", NomeBanco);

                    foreach (var lote in Regex.Split(conteudo, @"(?m)^\s*GO\s*$"))
                    {
                        if (!string.IsNullOrWhiteSpace(lote))
                            Executar(conexao, lote);
                    }
                }
            }

            AppServices.Initialize(ConnectionString);
        }

        public static SqlConnection Abrir()
        {
            var conexao = new SqlConnection(ConnectionString);
            conexao.Open();
            return conexao;
        }

        public static object Escalar(string sql, params SqlParameter[] parametros)
        {
            using (var conexao = Abrir())
            using (var comando = new SqlCommand(sql, conexao))
            {
                if (parametros != null) comando.Parameters.AddRange(parametros);
                return comando.ExecuteScalar();
            }
        }

        private static void Executar(SqlConnection conexao, string sql)
        {
            using (var comando = new SqlCommand(sql, conexao))
            {
                comando.CommandTimeout = 120;
                comando.ExecuteNonQuery();
            }
        }

        private static string LocalizarRaiz()
        {
            var diretorio = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (diretorio != null)
            {
                if (File.Exists(Path.Combine(diretorio.FullName, "GPFA.sln")))
                    return diretorio.FullName;
                diretorio = diretorio.Parent;
            }
            throw new DirectoryNotFoundException("Não foi possível localizar a raiz da solução (GPFA.sln).");
        }
    }
}
