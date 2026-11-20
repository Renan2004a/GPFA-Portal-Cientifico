using System;

namespace GPFA.Models
{
    public class PerfilAcesso
    {
        public int PerfilId { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }

    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Login { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        public int PerfilId { get; set; }
        public string PerfilNome { get; set; }
        public int? PesquisadorId { get; set; }
        public string PesquisadorNome { get; set; }
        public bool Ativo { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public int TentativasFalhas { get; set; }
        public DateTime? BloqueadoAte { get; set; }
        public DateTime CriadoEm { get; set; }

        public bool EhAdministrador
        {
            get { return PerfilId == Dominio.Perfil.Administrador; }
        }

        public bool EhCoordenador
        {
            get { return PerfilId == Dominio.Perfil.Coordenador; }
        }

        public bool EhPesquisador
        {
            get { return PerfilId == Dominio.Perfil.Pesquisador; }
        }

        public bool PodeGerenciarCatalogo
        {
            get { return EhAdministrador; }
        }

        public bool PodeGerenciarProjetos
        {
            get { return EhAdministrador || EhCoordenador; }
        }

        public bool Bloqueado
        {
            get { return BloqueadoAte.HasValue && BloqueadoAte.Value > DateTime.Now; }
        }
    }

    /// <summary>
    /// Fotografia mínima do usuário mantida em Session durante a requisição.
    /// Não contém hash nem dados sensíveis.
    /// </summary>
    public class UsuarioSessao
    {
        public int UsuarioId { get; set; }
        public string Login { get; set; }
        public string NomeCompleto { get; set; }
        public int PerfilId { get; set; }
        public string PerfilNome { get; set; }
        public int? PesquisadorId { get; set; }

        public bool EhAdministrador
        {
            get { return PerfilId == Dominio.Perfil.Administrador; }
        }

        public bool EhCoordenador
        {
            get { return PerfilId == Dominio.Perfil.Coordenador; }
        }

        public bool PodeGerenciarProjetos
        {
            get { return EhAdministrador || EhCoordenador; }
        }

        public string PrimeiroNome
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NomeCompleto)) return Login;
                return NomeCompleto.Split(' ')[0];
            }
        }
    }

    public class LogAuditoria
    {
        public long LogId { get; set; }
        public int? UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public string Acao { get; set; }
        public string Entidade { get; set; }
        public int? EntidadeId { get; set; }
        public string Detalhe { get; set; }
        public DateTime DataHora { get; set; }
    }
}
