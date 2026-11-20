using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using GPFA.Models;

namespace GPFA.Data
{
    public class EstudanteRepository
    {
        public List<EstudanteResumo> Listar(string termo = null, string nivel = null,
            string situacao = null, int? orientadorId = null)
        {
            var sql = "SELECT * FROM dbo.vw_EstudanteResumo WHERE 1 = 1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(termo))
            {
                sql += @" AND (NomeCompleto LIKE @Termo OR Curso LIKE @Termo
                          OR OrientadorNome LIKE @Termo OR ProjetosAtivos LIKE @Termo)";
                parameters.Add(SqlHelper.Param("@Termo", "%" + termo.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(nivel))
            {
                sql += " AND NivelAcademico = @Nivel";
                parameters.Add(SqlHelper.Param("@Nivel", nivel));
            }
            if (!string.IsNullOrWhiteSpace(situacao))
            {
                sql += " AND Situacao = @Situacao";
                parameters.Add(SqlHelper.Param("@Situacao", situacao));
            }
            if (orientadorId.HasValue)
            {
                sql += " AND OrientadorId = @Orientador";
                parameters.Add(SqlHelper.Param("@Orientador", orientadorId.Value));
            }

            sql += " ORDER BY CASE WHEN Situacao = N'Ativo' THEN 0 ELSE 1 END, NomeCompleto";
            return SqlHelper.Query(sql, Mappers.EstudanteResumo, parameters.ToArray());
        }

        public EstudanteResumo ObterResumo(int estudanteId)
        {
            return SqlHelper.QuerySingle("SELECT * FROM dbo.vw_EstudanteResumo WHERE EstudanteId = @Id",
                Mappers.EstudanteResumo, SqlHelper.Param("@Id", estudanteId));
        }

        public Estudante ObterEntidade(int estudanteId)
        {
            const string sql = @"
SELECT p.PessoaId, p.NomeCompleto, p.Email, p.FotoUrl, p.FormacaoAcademica, p.AreaAtuacao,
       p.MiniBiografia, p.GoogleScholarUrl, p.OrcidUrl, p.LattesUrl, p.Ativo,
       e.NivelAcademico, e.Curso, e.Situacao, e.DataIngresso, e.DataConclusao, e.CoorientadoresExternos
FROM dbo.Estudante e
JOIN dbo.Pessoa p ON p.PessoaId = e.EstudanteId
WHERE e.EstudanteId = @Id";
            return SqlHelper.QuerySingle(sql, Mappers.EstudanteEntity, SqlHelper.Param("@Id", estudanteId));
        }

        /// <summary>
        /// Cria pessoa, estudante, orientação principal e vínculos com projetos
        /// em uma única transação, garantindo RN01 e RN04 de forma atômica.
        /// </summary>
        public int Inserir(Estudante estudante, int orientadorId, IList<int> projetoIds,
            DateTime dataInicioOrientacao, string tipoVinculo)
        {
            if (projetoIds == null || projetoIds.Count == 0)
                throw new ValidacaoException("Todo estudante deve ser vinculado a ao menos um projeto de pesquisa.");
            if (orientadorId <= 0)
                throw new ValidacaoException("Todo estudante deve possuir um orientador principal cadastrado.");

            int novoId = 0;
            SqlHelper.InTransaction((cn, tx) =>
            {
                const string sqlPessoa = @"
INSERT dbo.Pessoa (NomeCompleto, Email, TipoPessoa, FotoUrl, FormacaoAcademica, AreaAtuacao,
                   MiniBiografia, GoogleScholarUrl, OrcidUrl, LattesUrl, Ativo)
VALUES (@Nome, @Email, 'E', @Foto, @Formacao, @Area, @Bio, @Scholar, @Orcid, @Lattes, @Ativo);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
                using (var cmd = SqlHelper.Command(cn, tx, sqlPessoa,
                    SqlHelper.Param("@Nome", estudante.NomeCompleto.Trim()),
                    SqlHelper.Param("@Email", estudante.Email.Trim()),
                    SqlHelper.Param("@Foto", DbValue.OrDBNull(estudante.FotoUrl)),
                    SqlHelper.Param("@Formacao", DbValue.OrDBNull(estudante.FormacaoAcademica)),
                    SqlHelper.Param("@Area", DbValue.OrDBNull(estudante.AreaAtuacao)),
                    SqlHelper.Param("@Bio", DbValue.OrDBNull(estudante.MiniBiografia)),
                    SqlHelper.Param("@Scholar", DbValue.OrDBNull(estudante.GoogleScholarUrl)),
                    SqlHelper.Param("@Orcid", DbValue.OrDBNull(estudante.OrcidUrl)),
                    SqlHelper.Param("@Lattes", DbValue.OrDBNull(estudante.LattesUrl)),
                    SqlHelper.Param("@Ativo", estudante.Ativo)))
                {
                    novoId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                const string sqlEstudante = @"
INSERT dbo.Estudante (EstudanteId, NivelAcademico, Curso, Situacao, DataIngresso,
                       DataConclusao, CoorientadoresExternos)
VALUES (@Id, @Nivel, @Curso, N'Ativo', @Ingresso, NULL, @Coorientadores);";
                using (var cmd = SqlHelper.Command(cn, tx, sqlEstudante,
                    SqlHelper.Param("@Id", novoId),
                    SqlHelper.Param("@Nivel", estudante.NivelAcademico),
                    SqlHelper.Param("@Curso", estudante.Curso.Trim()),
                    SqlHelper.Param("@Ingresso", estudante.DataIngresso),
                    SqlHelper.Param("@Coorientadores", DbValue.OrDBNull(estudante.CoorientadoresExternos))))
                {
                    cmd.ExecuteNonQuery();
                }

                const string sqlOrientacao = @"
INSERT dbo.Orientacao (EstudanteId, OrientadorId, Principal, DataInicio, DataFim, Observacao)
VALUES (@Estudante, @Orientador, 1, @Inicio, NULL, N'Orientação principal criada no cadastro do estudante.');";
                using (var cmd = SqlHelper.Command(cn, tx, sqlOrientacao,
                    SqlHelper.Param("@Estudante", novoId),
                    SqlHelper.Param("@Orientador", orientadorId),
                    SqlHelper.Param("@Inicio", dataInicioOrientacao)))
                {
                    cmd.ExecuteNonQuery();
                }

                const string sqlProjeto = @"
INSERT dbo.EstudanteProjeto (EstudanteId, ProjetoId, DataEntrada, DataSaida, TipoVinculo)
VALUES (@Estudante, @Projeto, @Entrada, NULL, @Tipo);";
                foreach (var projetoId in projetoIds)
                {
                    using (var cmd = SqlHelper.Command(cn, tx, sqlProjeto,
                        SqlHelper.Param("@Estudante", novoId),
                        SqlHelper.Param("@Projeto", projetoId),
                        SqlHelper.Param("@Entrada", estudante.DataIngresso),
                        SqlHelper.Param("@Tipo", DbValue.OrDBNull(tipoVinculo))))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            });
            return novoId;
        }

        public void Atualizar(Estudante estudante)
        {
            SqlHelper.InTransaction((cn, tx) =>
            {
                const string sqlPessoa = @"
UPDATE dbo.Pessoa
SET NomeCompleto = @Nome, Email = @Email, FotoUrl = @Foto, FormacaoAcademica = @Formacao,
    AreaAtuacao = @Area, MiniBiografia = @Bio, GoogleScholarUrl = @Scholar,
    OrcidUrl = @Orcid, LattesUrl = @Lattes, Ativo = @Ativo, AtualizadoEm = SYSDATETIME()
WHERE PessoaId = @Id;";
                using (var cmd = SqlHelper.Command(cn, tx, sqlPessoa,
                    SqlHelper.Param("@Nome", estudante.NomeCompleto.Trim()),
                    SqlHelper.Param("@Email", estudante.Email.Trim()),
                    SqlHelper.Param("@Foto", DbValue.OrDBNull(estudante.FotoUrl)),
                    SqlHelper.Param("@Formacao", DbValue.OrDBNull(estudante.FormacaoAcademica)),
                    SqlHelper.Param("@Area", DbValue.OrDBNull(estudante.AreaAtuacao)),
                    SqlHelper.Param("@Bio", DbValue.OrDBNull(estudante.MiniBiografia)),
                    SqlHelper.Param("@Scholar", DbValue.OrDBNull(estudante.GoogleScholarUrl)),
                    SqlHelper.Param("@Orcid", DbValue.OrDBNull(estudante.OrcidUrl)),
                    SqlHelper.Param("@Lattes", DbValue.OrDBNull(estudante.LattesUrl)),
                    SqlHelper.Param("@Ativo", estudante.Ativo),
                    SqlHelper.Param("@Id", estudante.PessoaId)))
                {
                    cmd.ExecuteNonQuery();
                }

                const string sqlEstudante = @"
UPDATE dbo.Estudante
SET NivelAcademico = @Nivel, Curso = @Curso, Situacao = @Situacao,
    DataIngresso = @Ingresso, DataConclusao = @Conclusao, CoorientadoresExternos = @Coorientadores
WHERE EstudanteId = @Id;";
                using (var cmd = SqlHelper.Command(cn, tx, sqlEstudante,
                    SqlHelper.Param("@Nivel", estudante.NivelAcademico),
                    SqlHelper.Param("@Curso", estudante.Curso.Trim()),
                    SqlHelper.Param("@Situacao", estudante.Situacao),
                    SqlHelper.Param("@Ingresso", estudante.DataIngresso),
                    SqlHelper.Param("@Conclusao", DbValue.OrDBNull(estudante.DataConclusao)),
                    SqlHelper.Param("@Coorientadores", DbValue.OrDBNull(estudante.CoorientadoresExternos)),
                    SqlHelper.Param("@Id", estudante.PessoaId)))
                {
                    cmd.ExecuteNonQuery();
                }
            });
        }

        public void Concluir(int estudanteId, DateTime dataConclusao)
        {
            SqlHelper.InTransaction((cn, tx) =>
            {
                using (var cmd = SqlHelper.Command(cn, tx, @"
UPDATE dbo.Estudante SET Situacao = N'Concluído', DataConclusao = @Data WHERE EstudanteId = @Id;",
                    SqlHelper.Param("@Data", dataConclusao), SqlHelper.Param("@Id", estudanteId)))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = SqlHelper.Command(cn, tx, @"
UPDATE dbo.Orientacao SET DataFim = @Data
WHERE EstudanteId = @Id AND DataFim IS NULL;",
                    SqlHelper.Param("@Data", dataConclusao), SqlHelper.Param("@Id", estudanteId)))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = SqlHelper.Command(cn, tx, @"
UPDATE dbo.EstudanteProjeto SET DataSaida = @Data
WHERE EstudanteId = @Id AND DataSaida IS NULL;",
                    SqlHelper.Param("@Data", dataConclusao), SqlHelper.Param("@Id", estudanteId)))
                {
                    cmd.ExecuteNonQuery();
                }
            });
        }

        public void Excluir(int estudanteId)
        {
            SqlHelper.InTransaction((cn, tx) =>
            {
                // Desativa antes de remover para que os triggers de proteção
                // (estudante ativo sem projeto/orientador) permitam a exclusão.
                using (var cmd = SqlHelper.Command(cn, tx,
                    "UPDATE dbo.Estudante SET Situacao = N'Desligado' WHERE EstudanteId = @Id;",
                    SqlHelper.Param("@Id", estudanteId)))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = SqlHelper.Command(cn, tx,
                    "DELETE dbo.Orientacao WHERE EstudanteId = @Id; DELETE dbo.EstudanteProjeto WHERE EstudanteId = @Id;",
                    SqlHelper.Param("@Id", estudanteId)))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = SqlHelper.Command(cn, tx,
                    "DELETE dbo.Estudante WHERE EstudanteId = @Id;",
                    SqlHelper.Param("@Id", estudanteId)))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = SqlHelper.Command(cn, tx,
                    "DELETE dbo.Pessoa WHERE PessoaId = @Id;",
                    SqlHelper.Param("@Id", estudanteId)))
                {
                    cmd.ExecuteNonQuery();
                }
            });
        }

        public bool EmailEmUso(string email, int? ignorarPessoaId = null)
        {
            const string sql = @"
SELECT COUNT(*) FROM dbo.Pessoa
WHERE Email = @Email AND (@Ignorar IS NULL OR PessoaId <> @Ignorar)";
            return SqlHelper.ScalarInt(sql,
                SqlHelper.Param("@Email", email.Trim()),
                SqlHelper.Param("@Ignorar", DbValue.OrDBNull(ignorarPessoaId))) > 0;
        }

        public List<ProjetoResumo> ListarProjetos(int estudanteId)
        {
            const string sql = @"
SELECT pr.*
FROM dbo.vw_ProjetoResumo pr
JOIN dbo.EstudanteProjeto ep ON ep.ProjetoId = pr.ProjetoId
WHERE ep.EstudanteId = @Id
ORDER BY CASE WHEN ep.DataSaida IS NULL THEN 0 ELSE 1 END, pr.DataInicio DESC;";
            return SqlHelper.Query(sql, Mappers.ProjetoResumo, SqlHelper.Param("@Id", estudanteId));
        }

        public List<Orientacao> ListarOrientacoes(int estudanteId)
        {
            const string sql = @"
SELECT o.OrientacaoId, o.EstudanteId, epe.NomeCompleto AS EstudanteNome,
       o.OrientadorId, po.NomeCompleto AS OrientadorNome, o.Principal,
       o.DataInicio, o.DataFim, o.Observacao,
       e.NivelAcademico AS EstudanteNivel, e.Situacao AS EstudanteSituacao
FROM dbo.Orientacao o
JOIN dbo.Estudante e ON e.EstudanteId = o.EstudanteId
JOIN dbo.Pessoa epe ON epe.PessoaId = o.EstudanteId
JOIN dbo.Pessoa po ON po.PessoaId = o.OrientadorId
WHERE o.EstudanteId = @Id
ORDER BY o.DataInicio DESC;";
            return SqlHelper.Query(sql, Mappers.Orientacao, SqlHelper.Param("@Id", estudanteId));
        }

        public List<EstudanteResumo> ListarSemVinculoCompleto()
        {
            return SqlHelper.Query(@"
SELECT * FROM dbo.vw_EstudanteResumo
WHERE Situacao = N'Ativo' AND (OrientadorId IS NULL OR QtdProjetosAtivos = 0)
ORDER BY NomeCompleto", Mappers.EstudanteResumo);
        }
    }
}
