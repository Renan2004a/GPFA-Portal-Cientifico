using System;

namespace GPFA.Models
{
    /// <summary>
    /// Valores de domínio persistidos no banco. Centralizá-los evita "strings
    /// mágicas" espalhadas pelas camadas e mantém os CHECK constraints alinhados.
    /// </summary>
    public static class Dominio
    {
        public static class SituacaoProjeto
        {
            public const string EmAndamento = "Em andamento";
            public const string Concluido = "Concluído";
            public const string Suspenso = "Suspenso";
            public const string Planejamento = "Planejamento";

            public static readonly string[] Todos =
                { EmAndamento, Concluido, Suspenso, Planejamento };
        }

        public static class SituacaoEstudante
        {
            public const string Ativo = "Ativo";
            public const string Concluido = "Concluído";
            public const string Desligado = "Desligado";
            public const string Trancado = "Trancado";

            public static readonly string[] Todas =
                { Ativo, Concluido, Desligado, Trancado };
        }

        public static class NivelAcademico
        {
            public const string IniciacaoCientifica = "Iniciação Científica";
            public const string Graduacao = "Graduação";
            public const string Mestrado = "Mestrado";
            public const string Doutorado = "Doutorado";
            public const string PosDoutorado = "Pós-doutorado";
            public const string Tecnico = "Técnico";

            public static readonly string[] Todos =
                { IniciacaoCientifica, Graduacao, Mestrado, Doutorado, PosDoutorado, Tecnico };
        }

        public static class TipoPublicacao
        {
            public const string Artigo = "Artigo";
            public const string ArtigoConferencia = "Artigo de conferência";
            public const string Livro = "Livro";
            public const string CapituloLivro = "Capítulo de livro";
            public const string Patente = "Patente";
            public const string Outro = "Outro";

            public static readonly string[] Todos =
                { Artigo, ArtigoConferencia, Livro, CapituloLivro, Patente, Outro };
        }

        public static class TipoFonte
        {
            public const string AgenciaPublica = "Agência pública";
            public const string Fundacao = "Fundação";
            public const string ParceriaPrivada = "Parceria privada";
            public const string Internacional = "Internacional";
            public const string Outra = "Outra";

            public static readonly string[] Todos =
                { AgenciaPublica, Fundacao, ParceriaPrivada, Internacional, Outra };
        }

        public static class TipoPessoa
        {
            public const char Pesquisador = 'P';
            public const char Estudante = 'E';
        }

        public static class Perfil
        {
            public const int Administrador = 1;
            public const int Coordenador = 2;
            public const int Pesquisador = 3;

            public const string AdministradorNome = "Administrador";
            public const string CoordenadorNome = "Coordenador";
            public const string PesquisadorNome = "Pesquisador";
        }
    }

    public class RegraNegocioException : Exception
    {
        public RegraNegocioException(string message) : base(message) { }
        public RegraNegocioException(string message, Exception inner) : base(message, inner) { }
    }

    public class ValidacaoException : RegraNegocioException
    {
        public System.Collections.Generic.IList<string> Erros { get; private set; }

        public ValidacaoException(string message) : base(message)
        {
            Erros = new System.Collections.Generic.List<string> { message };
        }

        public ValidacaoException(System.Collections.Generic.IEnumerable<string> erros)
            : base(string.Join(" ", erros))
        {
            Erros = new System.Collections.Generic.List<string>(erros);
        }
    }

    public class IntegridadeDadosException : RegraNegocioException
    {
        public IntegridadeDadosException(string message) : base(message) { }
        public IntegridadeDadosException(string message, Exception inner) : base(message, inner) { }
    }
}
