using System;
using System.Collections.Generic;
using System.Linq;
using GPFA.Data;
using GPFA.Models;
using GPFA.Business.Security;

namespace GPFA.Business
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _repository;
        private readonly PesquisadorRepository _pesquisadorRepository;

        public UsuarioService(UsuarioRepository repository, PesquisadorRepository pesquisadorRepository)
        {
            _repository = repository;
            _pesquisadorRepository = pesquisadorRepository;
        }

        public List<Usuario> Listar()
        {
            return _repository.Listar();
        }

        public Usuario Obter(int id)
        {
            return _repository.Obter(id);
        }

        public List<PerfilAcesso> ListarPerfis()
        {
            return _repository.ListarPerfis();
        }

        public int Salvar(Usuario usuario, string senha, UsuarioSessao usuarioAtual)
        {
            if (usuarioAtual == null || !usuarioAtual.EhAdministrador)
                throw new RegraNegocioException("Somente o administrador pode gerenciar usuários.");

            if (usuario == null)
                throw new ValidacaoException("Dados do usuário não informados.");

            var erros = new List<string>();
            Validacao.Coletar(erros, () => usuario.Login = Validacao.Login(usuario.Login));
            Validacao.Coletar(erros, () => usuario.NomeCompleto = Validacao.TextoObrigatorio(usuario.NomeCompleto, "nome completo", 3, 150));
            Validacao.Coletar(erros, () => usuario.Email = Validacao.Email(usuario.Email));
            Validacao.Coletar(erros, () =>
            {
                if (usuario.PerfilId < 1 || usuario.PerfilId > 3)
                    throw new ValidacaoException("Selecione um perfil de acesso válido.");
            });
            Validacao.Coletar(erros, () =>
            {
                if (usuario.PerfilId != Dominio.Perfil.Administrador && !usuario.PesquisadorId.HasValue)
                    throw new ValidacaoException("Usuários coordenadores e pesquisadores devem estar vinculados a um pesquisador.");
            });
            if (usuario.UsuarioId == 0)
            {
                Validacao.Coletar(erros, () => PasswordHasher.ValidarForca(senha));
            }
            else if (!string.IsNullOrWhiteSpace(senha))
            {
                Validacao.Coletar(erros, () => PasswordHasher.ValidarForca(senha));
            }
            Validacao.LancarSeHouver(erros);

            if (usuario.PesquisadorId.HasValue && _pesquisadorRepository.ObterResumo(usuario.PesquisadorId.Value) == null)
                throw new ValidacaoException("O pesquisador vinculado ao usuário não existe.");

            if (_repository.LoginEmUso(usuario.Login, usuario.UsuarioId > 0 ? (int?)usuario.UsuarioId : null))
                throw new ValidacaoException("Já existe um usuário com este login.");
            if (_repository.EmailEmUso(usuario.Email, usuario.UsuarioId > 0 ? (int?)usuario.UsuarioId : null))
                throw new ValidacaoException("Já existe um usuário com este e-mail.");

            if (usuario.UsuarioId > 0)
            {
                var atual = _repository.Obter(usuario.UsuarioId);
                if (atual == null) throw new ValidacaoException("Usuário não encontrado.");

                // Evita remover o último administrador ativo do sistema.
                if (atual.EhAdministrador && (!usuario.Ativo || usuario.PerfilId != Dominio.Perfil.Administrador))
                {
                    if (_repository.ContarAdminsAtivos(usuario.UsuarioId) == 0)
                        throw new RegraNegocioException("O sistema deve manter ao menos um administrador ativo.");
                }

                if (!string.IsNullOrWhiteSpace(senha))
                    usuario.SenhaHash = PasswordHasher.Hash(senha);

                _repository.Atualizar(usuario, !string.IsNullOrWhiteSpace(senha));
                return usuario.UsuarioId;
            }

            usuario.SenhaHash = PasswordHasher.Hash(senha);
            return _repository.Inserir(usuario);
        }

        public void Excluir(int usuarioId, UsuarioSessao usuarioAtual)
        {
            if (usuarioAtual == null || !usuarioAtual.EhAdministrador)
                throw new RegraNegocioException("Somente o administrador pode excluir usuários.");
            if (usuarioAtual.UsuarioId == usuarioId)
                throw new RegraNegocioException("Você não pode excluir o próprio usuário autenticado.");

            var usuario = _repository.Obter(usuarioId);
            if (usuario == null) throw new ValidacaoException("Usuário não encontrado.");
            if (usuario.EhAdministrador && usuario.Ativo && _repository.ContarAdminsAtivos(usuarioId) == 0)
                throw new RegraNegocioException("O sistema deve manter ao menos um administrador ativo.");

            _repository.Excluir(usuarioId);
        }

        public void RegistrarAuditoria(UsuarioSessao usuario, string acao, string entidade,
            int? entidadeId = null, string detalhe = null)
        {
            _repository.RegistrarAuditoria(usuario == null ? (int?)null : usuario.UsuarioId,
                acao, entidade, entidadeId, detalhe);
        }

        public List<LogAuditoria> ListarAuditoria(int limite = 30)
        {
            return _repository.ListarAuditoria(limite);
        }
    }
}
