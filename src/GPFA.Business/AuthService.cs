using System;
using GPFA.Data;
using GPFA.Models;
using GPFA.Business.Security;

namespace GPFA.Business
{
    public class ResultadoLogin
    {
        public bool Sucesso { get; set; }
        public Usuario Usuario { get; set; }
        public string Mensagem { get; set; }
    }

    public class AuthService
    {
        private readonly UsuarioRepository _repository;

        public AuthService(UsuarioRepository repository)
        {
            _repository = repository;
        }

        public ResultadoLogin Autenticar(string login, string senha)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha))
                return new ResultadoLogin { Sucesso = false, Mensagem = "Informe login e senha." };

            var usuario = _repository.BuscarPorLogin(login.Trim());
            if (usuario == null)
            {
                // Verificação descartável para reduzir diferença de tempo entre
                // usuário inexistente e senha incorreta.
                PasswordHasher.Verificar(senha, PasswordHasher.HashFicticio);
                return new ResultadoLogin { Sucesso = false, Mensagem = "Login ou senha inválidos." };
            }

            if (!usuario.Ativo)
                return new ResultadoLogin { Sucesso = false, Mensagem = "Este usuário está inativo. Procure o administrador." };

            if (usuario.Bloqueado)
            {
                var minutos = Math.Max(1, (int)Math.Ceiling((usuario.BloqueadoAte.Value - DateTime.Now).TotalMinutes));
                return new ResultadoLogin
                {
                    Sucesso = false,
                    Mensagem = "Conta temporariamente bloqueada por tentativas inválidas. Tente novamente em " + minutos + " minuto(s)."
                };
            }

            if (!PasswordHasher.Verificar(senha, usuario.SenhaHash))
            {
                _repository.RegistrarFalha(usuario.UsuarioId);
                _repository.RegistrarAuditoria(usuario.UsuarioId, "LoginFalha", "Usuario", usuario.UsuarioId,
                    "Tentativa de autenticação com senha inválida.");
                var tentativas = usuario.TentativasFalhas + 1;
                var mensagem = tentativas >= 5
                    ? "Conta bloqueada por 15 minutos após 5 tentativas inválidas."
                    : "Login ou senha inválidos.";
                return new ResultadoLogin { Sucesso = false, Mensagem = mensagem };
            }

            _repository.RegistrarAcesso(usuario.UsuarioId);
            _repository.RegistrarAuditoria(usuario.UsuarioId, "Login", "Usuario", usuario.UsuarioId, "Acesso autenticado.");
            usuario.UltimoAcesso = DateTime.Now;
            return new ResultadoLogin { Sucesso = true, Usuario = usuario, Mensagem = "Login realizado." };
        }

        public void AlterarSenha(int usuarioId, string senhaAtual, string novaSenha, string confirmacao)
        {
            var usuario = _repository.Obter(usuarioId);
            if (usuario == null) throw new ValidacaoException("Usuário não encontrado.");
            if (!PasswordHasher.Verificar(senhaAtual, usuario.SenhaHash))
                throw new ValidacaoException("A senha atual informada está incorreta.");
            if (novaSenha != confirmacao)
                throw new ValidacaoException("A nova senha e a confirmação não coincidem.");
            if (PasswordHasher.Verificar(novaSenha, usuario.SenhaHash))
                throw new ValidacaoException("A nova senha deve ser diferente da senha atual.");

            PasswordHasher.ValidarForca(novaSenha);
            _repository.AlterarSenha(usuarioId, PasswordHasher.Hash(novaSenha));
            _repository.RegistrarAuditoria(usuarioId, "AlterarSenha", "Usuario", usuarioId, "Senha alterada pelo próprio usuário.");
        }
    }
}
