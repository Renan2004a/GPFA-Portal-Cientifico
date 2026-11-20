using System;
using System.Collections.Generic;
using GPFA.Models;

namespace GPFA.Data
{
    public class UsuarioRepository
    {
        private const string SelectBase = @"
SELECT u.UsuarioId, u.Login, u.NomeCompleto, u.Email, u.SenhaHash,
       u.PerfilId, pa.Nome AS PerfilNome, u.PesquisadorId,
       p.NomeCompleto AS PesquisadorNome, u.Ativo, u.UltimoAcesso,
       u.TentativasFalhas, u.BloqueadoAte, u.CriadoEm
FROM dbo.Usuario u
JOIN dbo.PerfilAcesso pa ON pa.PerfilId = u.PerfilId
LEFT JOIN dbo.Pessoa p ON p.PessoaId = u.PesquisadorId";

        public Usuario BuscarPorLogin(string login)
        {
            return SqlHelper.QuerySingle(SelectBase + " WHERE u.Login = @Login",
                Mappers.Usuario, SqlHelper.Param("@Login", login.Trim()));
        }

        public Usuario Obter(int usuarioId)
        {
            return SqlHelper.QuerySingle(SelectBase + " WHERE u.UsuarioId = @Id",
                Mappers.Usuario, SqlHelper.Param("@Id", usuarioId));
        }

        public List<Usuario> Listar()
        {
            return SqlHelper.Query(SelectBase + " ORDER BY u.NomeCompleto", Mappers.Usuario);
        }

        public List<PerfilAcesso> ListarPerfis()
        {
            return SqlHelper.Query("SELECT PerfilId, Nome, Descricao FROM dbo.PerfilAcesso ORDER BY PerfilId",
                r => new PerfilAcesso
                {
                    PerfilId = DbValue.Int(r, "PerfilId"),
                    Nome = DbValue.String(r, "Nome"),
                    Descricao = DbValue.String(r, "Descricao")
                });
        }

        public int Inserir(Usuario usuario)
        {
            return Convert.ToInt32(SqlHelper.Scalar(@"
INSERT dbo.Usuario (Login, NomeCompleto, Email, SenhaHash, PerfilId, PesquisadorId, Ativo)
VALUES (@Login, @Nome, @Email, @Hash, @Perfil, @Pesquisador, @Ativo);
SELECT CAST(SCOPE_IDENTITY() AS INT);",
                SqlHelper.Param("@Login", usuario.Login.Trim()),
                SqlHelper.Param("@Nome", usuario.NomeCompleto.Trim()),
                SqlHelper.Param("@Email", usuario.Email.Trim()),
                SqlHelper.Param("@Hash", usuario.SenhaHash),
                SqlHelper.Param("@Perfil", usuario.PerfilId),
                SqlHelper.Param("@Pesquisador", DbValue.OrDBNull(usuario.PesquisadorId)),
                SqlHelper.Param("@Ativo", usuario.Ativo)));
        }

        public void Atualizar(Usuario usuario, bool atualizarSenha)
        {
            var sql = @"
UPDATE dbo.Usuario
SET Login = @Login, NomeCompleto = @Nome, Email = @Email, PerfilId = @Perfil,
    PesquisadorId = @Pesquisador, Ativo = @Ativo" +
                (atualizarSenha ? ", SenhaHash = @Hash, TentativasFalhas = 0, BloqueadoAte = NULL" : "") +
                " WHERE UsuarioId = @Id";

            var parameters = new List<System.Data.SqlClient.SqlParameter>
            {
                SqlHelper.Param("@Login", usuario.Login.Trim()),
                SqlHelper.Param("@Nome", usuario.NomeCompleto.Trim()),
                SqlHelper.Param("@Email", usuario.Email.Trim()),
                SqlHelper.Param("@Perfil", usuario.PerfilId),
                SqlHelper.Param("@Pesquisador", DbValue.OrDBNull(usuario.PesquisadorId)),
                SqlHelper.Param("@Ativo", usuario.Ativo),
                SqlHelper.Param("@Id", usuario.UsuarioId)
            };
            if (atualizarSenha)
                parameters.Add(SqlHelper.Param("@Hash", usuario.SenhaHash));

            SqlHelper.Execute(sql, parameters.ToArray());
        }

        public void Excluir(int usuarioId)
        {
            SqlHelper.Execute("DELETE dbo.Usuario WHERE UsuarioId = @Id",
                SqlHelper.Param("@Id", usuarioId));
        }

        public bool LoginEmUso(string login, int? ignorarUsuarioId = null)
        {
            return SqlHelper.ScalarInt(@"
SELECT COUNT(*) FROM dbo.Usuario
WHERE Login = @Login AND (@Ignorar IS NULL OR UsuarioId <> @Ignorar)",
                SqlHelper.Param("@Login", login.Trim()),
                SqlHelper.Param("@Ignorar", DbValue.OrDBNull(ignorarUsuarioId))) > 0;
        }

        public bool EmailEmUso(string email, int? ignorarUsuarioId = null)
        {
            return SqlHelper.ScalarInt(@"
SELECT COUNT(*) FROM dbo.Usuario
WHERE Email = @Email AND (@Ignorar IS NULL OR UsuarioId <> @Ignorar)",
                SqlHelper.Param("@Email", email.Trim()),
                SqlHelper.Param("@Ignorar", DbValue.OrDBNull(ignorarUsuarioId))) > 0;
        }

        public void RegistrarAcesso(int usuarioId)
        {
            SqlHelper.Execute(@"
UPDATE dbo.Usuario
SET UltimoAcesso = SYSDATETIME(), TentativasFalhas = 0, BloqueadoAte = NULL
WHERE UsuarioId = @Id", SqlHelper.Param("@Id", usuarioId));
        }

        public void RegistrarFalha(int usuarioId)
        {
            SqlHelper.Execute(@"
UPDATE dbo.Usuario
SET TentativasFalhas = TentativasFalhas + 1,
    BloqueadoAte = CASE WHEN TentativasFalhas + 1 >= 5 THEN DATEADD(MINUTE, 15, SYSDATETIME()) ELSE BloqueadoAte END
WHERE UsuarioId = @Id", SqlHelper.Param("@Id", usuarioId));
        }

        public void AlterarSenha(int usuarioId, string senhaHash)
        {
            SqlHelper.Execute(@"
UPDATE dbo.Usuario
SET SenhaHash = @Hash, TentativasFalhas = 0, BloqueadoAte = NULL
WHERE UsuarioId = @Id",
                SqlHelper.Param("@Hash", senhaHash),
                SqlHelper.Param("@Id", usuarioId));
        }

        public int ContarAdminsAtivos(int? ignorarUsuarioId = null)
        {
            return SqlHelper.ScalarInt(@"
SELECT COUNT(*) FROM dbo.Usuario
WHERE PerfilId = 1 AND Ativo = 1 AND (@Ignorar IS NULL OR UsuarioId <> @Ignorar)",
                SqlHelper.Param("@Ignorar", DbValue.OrDBNull(ignorarUsuarioId)));
        }

        public void RegistrarAuditoria(int? usuarioId, string acao, string entidade,
            int? entidadeId, string detalhe)
        {
            SqlHelper.Execute(@"
INSERT dbo.LogAuditoria (UsuarioId, Acao, Entidade, EntidadeId, Detalhe)
VALUES (@Usuario, @Acao, @Entidade, @EntidadeId, @Detalhe)",
                SqlHelper.Param("@Usuario", DbValue.OrDBNull(usuarioId)),
                SqlHelper.Param("@Acao", acao),
                SqlHelper.Param("@Entidade", entidade),
                SqlHelper.Param("@EntidadeId", DbValue.OrDBNull(entidadeId)),
                SqlHelper.Param("@Detalhe", DbValue.OrDBNull(detalhe)));
        }

        public List<LogAuditoria> ListarAuditoria(int limite)
        {
            return SqlHelper.Query(@"
SELECT TOP (@Limite) l.LogId, l.UsuarioId, u.NomeCompleto AS UsuarioNome,
       l.Acao, l.Entidade, l.EntidadeId, l.Detalhe, l.DataHora
FROM dbo.LogAuditoria l
LEFT JOIN dbo.Usuario u ON u.UsuarioId = l.UsuarioId
ORDER BY l.DataHora DESC, l.LogId DESC", Mappers.Log, SqlHelper.Param("@Limite", limite));
        }
    }
}
