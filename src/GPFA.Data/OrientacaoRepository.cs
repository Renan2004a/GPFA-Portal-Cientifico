using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using GPFA.Models;

namespace GPFA.Data
{
    public class OrientacaoRepository
    {
        private const string SelectBase = @"
SELECT o.OrientacaoId, o.EstudanteId, epe.NomeCompleto AS EstudanteNome,
       o.OrientadorId, po.NomeCompleto AS OrientadorNome, o.Principal,
       o.DataInicio, o.DataFim, o.Observacao,
       e.NivelAcademico AS EstudanteNivel, e.Situacao AS EstudanteSituacao
FROM dbo.Orientacao o
JOIN dbo.Estudante e ON e.EstudanteId = o.EstudanteId
JOIN dbo.Pessoa epe ON epe.PessoaId = o.EstudanteId
JOIN dbo.Pessoa po ON po.PessoaId = o.OrientadorId";

        public List<Orientacao> Listar(string termo = null, int? orientadorId = null, string vinculo = null)
        {
            var sql = SelectBase + " WHERE 1 = 1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(termo))
            {
                sql += " AND (epe.NomeCompleto LIKE @Termo OR po.NomeCompleto LIKE @Termo)";
                parameters.Add(SqlHelper.Param("@Termo", "%" + termo.Trim() + "%"));
            }
            if (orientadorId.HasValue)
            {
                sql += " AND o.OrientadorId = @Orientador";
                parameters.Add(SqlHelper.Param("@Orientador", orientadorId.Value));
            }
            if (!string.IsNullOrWhiteSpace(vinculo))
            {
                if (vinculo == "Atual")
                    sql += " AND o.DataFim IS NULL AND e.Situacao = N'Ativo'";
                else
                    sql += " AND (o.DataFim IS NOT NULL OR e.Situacao <> N'Ativo')";
            }

            sql += " ORDER BY CASE WHEN o.DataFim IS NULL THEN 0 ELSE 1 END, epe.NomeCompleto";
            return SqlHelper.Query(sql, Mappers.Orientacao, parameters.ToArray());
        }

        public List<Orientacao> ListarPorEstudante(int estudanteId)
        {
            return SqlHelper.Query(SelectBase + " WHERE o.EstudanteId = @Id ORDER BY o.DataInicio DESC",
                Mappers.Orientacao, SqlHelper.Param("@Id", estudanteId));
        }

        public Orientacao Obter(int orientacaoId)
        {
            return SqlHelper.QuerySingle(SelectBase + " WHERE o.OrientacaoId = @Id",
                Mappers.Orientacao, SqlHelper.Param("@Id", orientacaoId));
        }

        public bool ExistePrincipalAtiva(int estudanteId)
        {
            return SqlHelper.ScalarInt(@"
SELECT COUNT(*) FROM dbo.Orientacao
WHERE EstudanteId = @Id AND Principal = 1 AND DataFim IS NULL",
                SqlHelper.Param("@Id", estudanteId)) > 0;
        }

        /// <summary>
        /// Define uma nova orientação principal, encerrando a anterior na mesma
        /// data de início da nova, dentro de uma transação.
        /// </summary>
        public int DefinirPrincipal(int estudanteId, int orientadorId, DateTime dataInicio,
            string observacao)
        {
            var id = 0;
            SqlHelper.InTransaction((cn, tx) =>
            {
                using (var cmd = SqlHelper.Command(cn, tx, @"
UPDATE dbo.Orientacao SET DataFim = @Inicio
WHERE EstudanteId = @Estudante AND Principal = 1 AND DataFim IS NULL;",
                    SqlHelper.Param("@Inicio", dataInicio),
                    SqlHelper.Param("@Estudante", estudanteId)))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = SqlHelper.Command(cn, tx, @"
INSERT dbo.Orientacao (EstudanteId, OrientadorId, Principal, DataInicio, DataFim, Observacao)
VALUES (@Estudante, @Orientador, 1, @Inicio, NULL, @Observacao);
SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    SqlHelper.Param("@Estudante", estudanteId),
                    SqlHelper.Param("@Orientador", orientadorId),
                    SqlHelper.Param("@Inicio", dataInicio),
                    SqlHelper.Param("@Observacao", DbValue.OrDBNull(observacao))))
                {
                    id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            });
            return id;
        }

        /// <summary>
        /// Registra uma coorientação interna (pesquisador do grupo). O estudante
        /// pode ter vários coorientadores internos além do principal.
        /// </summary>
        public int InserirCoorientacao(int estudanteId, int orientadorId, DateTime dataInicio,
            string observacao)
        {
            return Convert.ToInt32(SqlHelper.Scalar(@"
INSERT dbo.Orientacao (EstudanteId, OrientadorId, Principal, DataInicio, DataFim, Observacao)
VALUES (@Estudante, @Orientador, 0, @Inicio, NULL, @Observacao);
SELECT CAST(SCOPE_IDENTITY() AS INT);",
                SqlHelper.Param("@Estudante", estudanteId),
                SqlHelper.Param("@Orientador", orientadorId),
                SqlHelper.Param("@Inicio", dataInicio),
                SqlHelper.Param("@Observacao", DbValue.OrDBNull(observacao))));
        }

        public void Encerrar(int orientacaoId, DateTime dataFim)
        {
            SqlHelper.Execute(@"
UPDATE dbo.Orientacao SET DataFim = @Fim
WHERE OrientacaoId = @Id AND DataFim IS NULL;",
                SqlHelper.Param("@Fim", dataFim),
                SqlHelper.Param("@Id", orientacaoId));
        }

        public void Excluir(int orientacaoId)
        {
            SqlHelper.Execute("DELETE dbo.Orientacao WHERE OrientacaoId = @Id",
                SqlHelper.Param("@Id", orientacaoId));
        }

        public List<Orientacao> ListarDoOrientador(int orientadorId, bool atuais)
        {
            var sql = SelectBase + (atuais
                ? " WHERE o.OrientadorId = @Id AND o.DataFim IS NULL AND e.Situacao = N'Ativo'"
                : " WHERE o.OrientadorId = @Id AND (o.DataFim IS NOT NULL OR e.Situacao <> N'Ativo')");
            return SqlHelper.Query(sql + " ORDER BY epe.NomeCompleto", Mappers.Orientacao,
                SqlHelper.Param("@Id", orientadorId));
        }
    }
}
