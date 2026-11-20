using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using GPFA.Models;

namespace GPFA.Data
{
    public class PublicacaoRepository
    {
        public List<PublicacaoResumo> Listar(FiltroPublicacoes filtro)
        {
            filtro = filtro ?? new FiltroPublicacoes();
            var sql = @"
SELECT DISTINCT d.*
FROM dbo.vw_PublicacaoDetalhe d";
            var parameters = new List<SqlParameter>();

            if (filtro.PesquisadorId.HasValue)
                sql += " JOIN dbo.PublicacaoAutor pa ON pa.PublicacaoId = d.PublicacaoId";

            sql += " WHERE 1 = 1";

            if (filtro.PesquisadorId.HasValue)
            {
                sql += " AND pa.PesquisadorId = @Pesquisador";
                parameters.Add(SqlHelper.Param("@Pesquisador", filtro.PesquisadorId.Value));
            }
            if (filtro.Ano.HasValue)
            {
                sql += " AND d.Ano = @Ano";
                parameters.Add(SqlHelper.Param("@Ano", filtro.Ano.Value));
            }
            if (filtro.ProjetoId.HasValue)
            {
                sql += " AND d.ProjetoId = @Projeto";
                parameters.Add(SqlHelper.Param("@Projeto", filtro.ProjetoId.Value));
            }
            if (!string.IsNullOrWhiteSpace(filtro.Termo))
            {
                sql += " AND (d.Titulo LIKE @Termo OR d.Veiculo LIKE @Termo OR d.Autores LIKE @Termo)";
                parameters.Add(SqlHelper.Param("@Termo", "%" + filtro.Termo.Trim() + "%"));
            }

            sql += " ORDER BY d.Ano DESC, d.Titulo";
            return SqlHelper.Query(sql, Mappers.PublicacaoResumo, parameters.ToArray());
        }

        public PublicacaoResumo ObterResumo(int publicacaoId)
        {
            return SqlHelper.QuerySingle(
                "SELECT * FROM dbo.vw_PublicacaoDetalhe WHERE PublicacaoId = @Id",
                Mappers.PublicacaoResumo, SqlHelper.Param("@Id", publicacaoId));
        }

        public Publicacao ObterEntidade(int publicacaoId)
        {
            const string sql = @"
SELECT pu.PublicacaoId, pu.Titulo, pu.Veiculo, pu.Ano, pu.Tipo, pu.Doi, pu.LinkAcesso,
       pu.ProjetoId, pr.Titulo AS ProjetoTitulo, pr.Sigla AS ProjetoSigla, pu.CriadoEm
FROM dbo.Publicacao pu
LEFT JOIN dbo.Projeto pr ON pr.ProjetoId = pu.ProjetoId
WHERE pu.PublicacaoId = @Id";
            var publicacao = SqlHelper.QuerySingle(sql, Mappers.PublicacaoEntity,
                SqlHelper.Param("@Id", publicacaoId));
            if (publicacao == null) return null;

            publicacao.Autores = SqlHelper.Query(@"
SELECT pa.PublicacaoId, pa.PesquisadorId, p.NomeCompleto AS PesquisadorNome, pa.OrdemAutoria
FROM dbo.PublicacaoAutor pa
JOIN dbo.Pessoa p ON p.PessoaId = pa.PesquisadorId
WHERE pa.PublicacaoId = @Id
ORDER BY pa.OrdemAutoria", r => new PublicacaoAutor
            {
                PublicacaoId = DbValue.Int(r, "PublicacaoId"),
                PesquisadorId = DbValue.Int(r, "PesquisadorId"),
                PesquisadorNome = DbValue.String(r, "PesquisadorNome"),
                OrdemAutoria = DbValue.Int(r, "OrdemAutoria")
            }, SqlHelper.Param("@Id", publicacaoId));
            return publicacao;
        }

        public int Inserir(Publicacao publicacao)
        {
            var id = 0;
            SqlHelper.InTransaction((cn, tx) =>
            {
                const string sql = @"
INSERT dbo.Publicacao (Titulo, Veiculo, Ano, Tipo, Doi, LinkAcesso, ProjetoId)
VALUES (@Titulo, @Veiculo, @Ano, @Tipo, @Doi, @Link, @Projeto);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
                using (var cmd = SqlHelper.Command(cn, tx, sql,
                    SqlHelper.Param("@Titulo", publicacao.Titulo.Trim()),
                    SqlHelper.Param("@Veiculo", publicacao.Veiculo.Trim()),
                    SqlHelper.Param("@Ano", publicacao.Ano),
                    SqlHelper.Param("@Tipo", publicacao.Tipo),
                    SqlHelper.Param("@Doi", DbValue.OrDBNull(publicacao.Doi)),
                    SqlHelper.Param("@Link", DbValue.OrDBNull(publicacao.LinkAcesso)),
                    SqlHelper.Param("@Projeto", DbValue.OrDBNull(publicacao.ProjetoId))))
                {
                    id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                InserirAutores(cn, tx, id, publicacao.Autores);
            });
            return id;
        }

        public void Atualizar(Publicacao publicacao)
        {
            SqlHelper.InTransaction((cn, tx) =>
            {
                const string sql = @"
UPDATE dbo.Publicacao
SET Titulo = @Titulo, Veiculo = @Veiculo, Ano = @Ano, Tipo = @Tipo,
    Doi = @Doi, LinkAcesso = @Link, ProjetoId = @Projeto
WHERE PublicacaoId = @Id";
                using (var cmd = SqlHelper.Command(cn, tx, sql,
                    SqlHelper.Param("@Titulo", publicacao.Titulo.Trim()),
                    SqlHelper.Param("@Veiculo", publicacao.Veiculo.Trim()),
                    SqlHelper.Param("@Ano", publicacao.Ano),
                    SqlHelper.Param("@Tipo", publicacao.Tipo),
                    SqlHelper.Param("@Doi", DbValue.OrDBNull(publicacao.Doi)),
                    SqlHelper.Param("@Link", DbValue.OrDBNull(publicacao.LinkAcesso)),
                    SqlHelper.Param("@Projeto", DbValue.OrDBNull(publicacao.ProjetoId)),
                    SqlHelper.Param("@Id", publicacao.PublicacaoId)))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = SqlHelper.Command(cn, tx,
                    "DELETE dbo.PublicacaoAutor WHERE PublicacaoId = @Id",
                    SqlHelper.Param("@Id", publicacao.PublicacaoId)))
                {
                    cmd.ExecuteNonQuery();
                }

                InserirAutores(cn, tx, publicacao.PublicacaoId, publicacao.Autores);
            });
        }

        private static void InserirAutores(SqlConnection cn, SqlTransaction tx, int publicacaoId,
            IList<PublicacaoAutor> autores)
        {
            const string sql = @"
INSERT dbo.PublicacaoAutor (PublicacaoId, PesquisadorId, OrdemAutoria)
VALUES (@Publicacao, @Pesquisador, @Ordem);";
            int ordem = 1;
            foreach (var autor in autores)
            {
                using (var cmd = SqlHelper.Command(cn, tx, sql,
                    SqlHelper.Param("@Publicacao", publicacaoId),
                    SqlHelper.Param("@Pesquisador", autor.PesquisadorId),
                    SqlHelper.Param("@Ordem", ordem++)))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Excluir(int publicacaoId)
        {
            SqlHelper.Execute("DELETE dbo.Publicacao WHERE PublicacaoId = @Id",
                SqlHelper.Param("@Id", publicacaoId));
        }

        public List<int> ListarAnos()
        {
            return SqlHelper.Query("SELECT DISTINCT Ano FROM dbo.Publicacao ORDER BY Ano DESC",
                r => DbValue.Int(r, "Ano"));
        }

        public List<PublicacaoResumo> ListarRecentes(int limite)
        {
            return SqlHelper.Query(@"
SELECT TOP (@Limite) * FROM dbo.vw_PublicacaoDetalhe
ORDER BY Ano DESC, PublicacaoId DESC",
                Mappers.PublicacaoResumo, SqlHelper.Param("@Limite", limite));
        }

        /* ------------------------------- Destaques ------------------------------- */

        public List<DestaqueAdminResumo> ListarDestaques(int? pesquisadorId = null)
        {
            var sql = @"
SELECT dp.DestaqueId, dp.PesquisadorId, p.NomeCompleto AS PesquisadorNome,
       dp.PublicacaoId, pu.Titulo, pu.Veiculo, pu.Ano, dp.Ordem
FROM dbo.DestaquePublicacao dp
JOIN dbo.Pessoa p ON p.PessoaId = dp.PesquisadorId
JOIN dbo.Publicacao pu ON pu.PublicacaoId = dp.PublicacaoId
WHERE 1 = 1";
            var parameters = new List<SqlParameter>();
            if (pesquisadorId.HasValue)
            {
                sql += " AND dp.PesquisadorId = @Id";
                parameters.Add(SqlHelper.Param("@Id", pesquisadorId.Value));
            }
            sql += " ORDER BY p.NomeCompleto, dp.Ordem";
            return SqlHelper.Query(sql, Mappers.DestaqueAdmin, parameters.ToArray());
        }

        public int ContarDestaques(int pesquisadorId)
        {
            return SqlHelper.ScalarInt(
                "SELECT COUNT(*) FROM dbo.DestaquePublicacao WHERE PesquisadorId = @Id",
                SqlHelper.Param("@Id", pesquisadorId));
        }

        public int InserirDestaque(int pesquisadorId, int publicacaoId, int ordem)
        {
            return Convert.ToInt32(SqlHelper.Scalar(@"
INSERT dbo.DestaquePublicacao (PesquisadorId, PublicacaoId, Ordem)
VALUES (@Pesquisador, @Publicacao, @Ordem);
SELECT CAST(SCOPE_IDENTITY() AS INT);",
                SqlHelper.Param("@Pesquisador", pesquisadorId),
                SqlHelper.Param("@Publicacao", publicacaoId),
                SqlHelper.Param("@Ordem", ordem)));
        }

        public void ExcluirDestaque(int destaqueId)
        {
            SqlHelper.Execute("DELETE dbo.DestaquePublicacao WHERE DestaqueId = @Id",
                SqlHelper.Param("@Id", destaqueId));
        }

        public bool PesquisadorEhAutor(int pesquisadorId, int publicacaoId)
        {
            return SqlHelper.ScalarInt(@"
SELECT COUNT(*) FROM dbo.PublicacaoAutor
WHERE PesquisadorId = @Pesquisador AND PublicacaoId = @Publicacao",
                SqlHelper.Param("@Pesquisador", pesquisadorId),
                SqlHelper.Param("@Publicacao", publicacaoId)) > 0;
        }
    }
}
