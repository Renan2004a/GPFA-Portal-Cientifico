using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using GPFA.Models;

namespace GPFA.Data
{
    public class PesquisadorRepository
    {
        private const string SelectResumo = @"
SELECT * FROM dbo.vw_PesquisadorResumo";

        public List<PesquisadorResumo> Listar(string termo = null, string area = null, bool somenteAtivos = true)
        {
            var sql = SelectResumo + " WHERE 1 = 1";
            var parameters = new List<SqlParameter>();

            if (somenteAtivos)
                sql += " AND Ativo = 1";

            if (!string.IsNullOrWhiteSpace(termo))
            {
                sql += @" AND (NomeCompleto LIKE @Termo OR AreaAtuacao LIKE @Termo
                          OR LinhaPesquisa LIKE @Termo OR FormacaoAcademica LIKE @Termo)";
                parameters.Add(SqlHelper.Param("@Termo", "%" + termo.Trim() + "%"));
            }

            if (!string.IsNullOrWhiteSpace(area))
            {
                sql += " AND AreaAtuacao = @Area";
                parameters.Add(SqlHelper.Param("@Area", area.Trim()));
            }

            sql += " ORDER BY NomeCompleto";
            return SqlHelper.Query(sql, Mappers.PesquisadorResumo, parameters.ToArray());
        }

        public PesquisadorResumo ObterResumo(int pesquisadorId)
        {
            return SqlHelper.QuerySingle(SelectResumo + " WHERE PesquisadorId = @Id",
                Mappers.PesquisadorResumo, SqlHelper.Param("@Id", pesquisadorId));
        }

        public Pesquisador ObterEntidade(int pesquisadorId)
        {
            const string sql = @"
SELECT p.PessoaId, p.NomeCompleto, p.Email, p.FotoUrl, p.FormacaoAcademica, p.AreaAtuacao,
       p.MiniBiografia, p.GoogleScholarUrl, p.OrcidUrl, p.LattesUrl, p.Ativo,
       pe.Titulacao, pe.Instituicao, pe.Departamento, pe.LinhaPesquisa, pe.DataCadastro
FROM dbo.Pesquisador pe
JOIN dbo.Pessoa p ON p.PessoaId = pe.PesquisadorId
WHERE pe.PesquisadorId = @Id";
            return SqlHelper.QuerySingle(sql, Mappers.PesquisadorEntity, SqlHelper.Param("@Id", pesquisadorId));
        }

        public int Inserir(Pesquisador pesquisador)
        {
            int novoId = 0;
            SqlHelper.InTransaction((cn, tx) =>
            {
                const string sqlPessoa = @"
INSERT dbo.Pessoa (NomeCompleto, Email, TipoPessoa, FotoUrl, FormacaoAcademica, AreaAtuacao,
                   MiniBiografia, GoogleScholarUrl, OrcidUrl, LattesUrl, Ativo)
VALUES (@Nome, @Email, 'P', @Foto, @Formacao, @Area, @Bio, @Scholar, @Orcid, @Lattes, @Ativo);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
                using (var cmd = SqlHelper.Command(cn, tx, sqlPessoa,
                    SqlHelper.Param("@Nome", pesquisador.NomeCompleto.Trim()),
                    SqlHelper.Param("@Email", pesquisador.Email.Trim()),
                    SqlHelper.Param("@Foto", DbValue.OrDBNull(pesquisador.FotoUrl)),
                    SqlHelper.Param("@Formacao", DbValue.OrDBNull(pesquisador.FormacaoAcademica)),
                    SqlHelper.Param("@Area", DbValue.OrDBNull(pesquisador.AreaAtuacao)),
                    SqlHelper.Param("@Bio", DbValue.OrDBNull(pesquisador.MiniBiografia)),
                    SqlHelper.Param("@Scholar", DbValue.OrDBNull(pesquisador.GoogleScholarUrl)),
                    SqlHelper.Param("@Orcid", DbValue.OrDBNull(pesquisador.OrcidUrl)),
                    SqlHelper.Param("@Lattes", DbValue.OrDBNull(pesquisador.LattesUrl)),
                    SqlHelper.Param("@Ativo", pesquisador.Ativo)))
                {
                    novoId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                const string sqlPesquisador = @"
INSERT dbo.Pesquisador (PesquisadorId, Titulacao, Instituicao, Departamento, LinhaPesquisa, DataCadastro)
VALUES (@Id, @Titulacao, @Instituicao, @Departamento, @Linha, @DataCadastro);";
                using (var cmd = SqlHelper.Command(cn, tx, sqlPesquisador,
                    SqlHelper.Param("@Id", novoId),
                    SqlHelper.Param("@Titulacao", DbValue.OrDBNull(pesquisador.Titulacao)),
                    SqlHelper.Param("@Instituicao", DbValue.OrDBNull(pesquisador.Instituicao)),
                    SqlHelper.Param("@Departamento", DbValue.OrDBNull(pesquisador.Departamento)),
                    SqlHelper.Param("@Linha", DbValue.OrDBNull(pesquisador.LinhaPesquisa)),
                    SqlHelper.Param("@DataCadastro", pesquisador.DataCadastro == DateTime.MinValue
                        ? (object)DateTime.Today : pesquisador.DataCadastro)))
                {
                    cmd.ExecuteNonQuery();
                }
            });
            return novoId;
        }

        public void Atualizar(Pesquisador pesquisador)
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
                    SqlHelper.Param("@Nome", pesquisador.NomeCompleto.Trim()),
                    SqlHelper.Param("@Email", pesquisador.Email.Trim()),
                    SqlHelper.Param("@Foto", DbValue.OrDBNull(pesquisador.FotoUrl)),
                    SqlHelper.Param("@Formacao", DbValue.OrDBNull(pesquisador.FormacaoAcademica)),
                    SqlHelper.Param("@Area", DbValue.OrDBNull(pesquisador.AreaAtuacao)),
                    SqlHelper.Param("@Bio", DbValue.OrDBNull(pesquisador.MiniBiografia)),
                    SqlHelper.Param("@Scholar", DbValue.OrDBNull(pesquisador.GoogleScholarUrl)),
                    SqlHelper.Param("@Orcid", DbValue.OrDBNull(pesquisador.OrcidUrl)),
                    SqlHelper.Param("@Lattes", DbValue.OrDBNull(pesquisador.LattesUrl)),
                    SqlHelper.Param("@Ativo", pesquisador.Ativo),
                    SqlHelper.Param("@Id", pesquisador.PessoaId)))
                {
                    cmd.ExecuteNonQuery();
                }

                const string sqlPesquisador = @"
UPDATE dbo.Pesquisador
SET Titulacao = @Titulacao, Instituicao = @Instituicao, Departamento = @Departamento,
    LinhaPesquisa = @Linha
WHERE PesquisadorId = @Id;";
                using (var cmd = SqlHelper.Command(cn, tx, sqlPesquisador,
                    SqlHelper.Param("@Titulacao", DbValue.OrDBNull(pesquisador.Titulacao)),
                    SqlHelper.Param("@Instituicao", DbValue.OrDBNull(pesquisador.Instituicao)),
                    SqlHelper.Param("@Departamento", DbValue.OrDBNull(pesquisador.Departamento)),
                    SqlHelper.Param("@Linha", DbValue.OrDBNull(pesquisador.LinhaPesquisa)),
                    SqlHelper.Param("@Id", pesquisador.PessoaId)))
                {
                    cmd.ExecuteNonQuery();
                }
            });
        }

        public void Excluir(int pesquisadorId)
        {
            SqlHelper.InTransaction((cn, tx) =>
            {
                using (var cmd = SqlHelper.Command(cn, tx,
                    "DELETE dbo.Pesquisador WHERE PesquisadorId = @Id;",
                    SqlHelper.Param("@Id", pesquisadorId)))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = SqlHelper.Command(cn, tx,
                    "DELETE dbo.Pessoa WHERE PessoaId = @Id;",
                    SqlHelper.Param("@Id", pesquisadorId)))
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

        public List<PesquisadorResumo> ListarAtivosResumo()
        {
            return SqlHelper.Query(SelectResumo + " WHERE Ativo = 1 ORDER BY NomeCompleto",
                Mappers.PesquisadorResumo);
        }

        public List<ProjetoResumo> ListarProjetos(int pesquisadorId)
        {
            const string sql = @"
SELECT pr.*
FROM dbo.vw_ProjetoResumo pr
JOIN dbo.vw_PesquisadorProjeto pp ON pp.ProjetoId = pr.ProjetoId
WHERE pp.PesquisadorId = @Id
ORDER BY CASE WHEN pr.Situacao = N'Em andamento' THEN 0 ELSE 1 END, pr.DataInicio DESC;";
            return SqlHelper.Query(sql, Mappers.ProjetoResumo, SqlHelper.Param("@Id", pesquisadorId));
        }

        public List<ProjetoResumo> ListarProjetosCoordenados(int pesquisadorId)
        {
            return SqlHelper.Query(
                "SELECT * FROM dbo.vw_ProjetoResumo WHERE CoordenadorId = @Id ORDER BY DataInicio DESC",
                Mappers.ProjetoResumo, SqlHelper.Param("@Id", pesquisadorId));
        }

        public List<PublicacaoResumo> ListarPublicacoes(int pesquisadorId)
        {
            const string sql = @"
SELECT d.*
FROM dbo.vw_PublicacaoDetalhe d
JOIN dbo.PublicacaoAutor pa ON pa.PublicacaoId = d.PublicacaoId
WHERE pa.PesquisadorId = @Id
ORDER BY d.Ano DESC, d.Titulo;";
            return SqlHelper.Query(sql, Mappers.PublicacaoResumo, SqlHelper.Param("@Id", pesquisadorId));
        }

        public List<PublicacaoResumo> ListarDestaques(int pesquisadorId)
        {
            const string sql = @"
SELECT d.*
FROM dbo.DestaquePublicacao dp
JOIN dbo.vw_PublicacaoDetalhe d ON d.PublicacaoId = dp.PublicacaoId
WHERE dp.PesquisadorId = @Id
ORDER BY dp.Ordem;";
            return SqlHelper.Query(sql, Mappers.PublicacaoResumo, SqlHelper.Param("@Id", pesquisadorId));
        }

        public List<OrientandoResumo> ListarOrientandos(int pesquisadorId, bool atuais)
        {
            var sql = "SELECT * FROM dbo.vw_Orientandos WHERE OrientadorId = @Id AND Vinculo = @Vinculo ORDER BY EstudanteNome";
            return SqlHelper.Query(sql, Mappers.Orientando,
                SqlHelper.Param("@Id", pesquisadorId),
                SqlHelper.Param("@Vinculo", atuais ? "Atual" : "Passado"));
        }

        public VinculosPesquisador ObterVinculos(int pesquisadorId)
        {
            return SqlHelper.QuerySingle(@"
SELECT
  (SELECT COUNT(*) FROM dbo.Projeto WHERE CoordenadorId = @Id)          AS ProjetosCoordenados,
  (SELECT COUNT(*) FROM dbo.ParticipacaoProjeto WHERE PesquisadorId = @Id) AS Participacoes,
  (SELECT COUNT(*) FROM dbo.Orientacao WHERE OrientadorId = @Id)        AS Orientacoes,
  (SELECT COUNT(*) FROM dbo.PublicacaoAutor WHERE PesquisadorId = @Id)  AS Publicacoes,
  (SELECT COUNT(*) FROM dbo.Usuario WHERE PesquisadorId = @Id)          AS Usuarios",
                r => new VinculosPesquisador
                {
                    ProjetosCoordenados = DbValue.Int(r, "ProjetosCoordenados"),
                    Participacoes = DbValue.Int(r, "Participacoes"),
                    Orientacoes = DbValue.Int(r, "Orientacoes"),
                    Publicacoes = DbValue.Int(r, "Publicacoes"),
                    Usuarios = DbValue.Int(r, "Usuarios")
                }, SqlHelper.Param("@Id", pesquisadorId));
        }
    }
}
