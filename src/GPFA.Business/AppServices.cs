using System;
using System.Configuration;
using GPFA.Data;

namespace GPFA.Business
{
    /// <summary>
    /// Raiz de composição simples (sem container externo). As instâncias não
    /// guardam estado por requisição: os repositórios abrem conexões próprias,
    /// portanto podem ser compartilhados com segurança entre requisições.
    /// </summary>
    public static class AppServices
    {
        private static readonly object Sync = new object();
        private static bool _inicializado;

        public static PesquisadorService Pesquisadores { get; private set; }
        public static EstudanteService Estudantes { get; private set; }
        public static ProjetoService Projetos { get; private set; }
        public static PublicacaoService Publicacoes { get; private set; }
        public static FinanciamentoService Financiamentos { get; private set; }
        public static UsuarioService Usuarios { get; private set; }
        public static AuthService Auth { get; private set; }
        public static RelatorioService Relatorios { get; private set; }

        public static void Initialize(string connectionString)
        {
            lock (Sync)
            {
                Database.Configure(connectionString);

                var pesquisadorRepository = new PesquisadorRepository();
                var estudanteRepository = new EstudanteRepository();
                var orientacaoRepository = new OrientacaoRepository();
                var projetoRepository = new ProjetoRepository();
                var publicacaoRepository = new PublicacaoRepository();
                var financiamentoRepository = new FinanciamentoRepository();
                var usuarioRepository = new UsuarioRepository();
                var relatorioRepository = new RelatorioRepository();

                Pesquisadores = new PesquisadorService(pesquisadorRepository);
                Estudantes = new EstudanteService(estudanteRepository, orientacaoRepository,
                    pesquisadorRepository, projetoRepository);
                Projetos = new ProjetoService(projetoRepository, pesquisadorRepository,
                    financiamentoRepository, publicacaoRepository, estudanteRepository);
                Publicacoes = new PublicacaoService(publicacaoRepository, pesquisadorRepository, projetoRepository);
                Financiamentos = new FinanciamentoService(financiamentoRepository, projetoRepository);
                Usuarios = new UsuarioService(usuarioRepository, pesquisadorRepository);
                Auth = new AuthService(usuarioRepository);
                Relatorios = new RelatorioService(relatorioRepository, financiamentoRepository);

                _inicializado = true;
            }
        }

        public static void Initialize()
        {
            var connection = ConfigurationManager.ConnectionStrings["GPFA"];
            if (connection == null)
                throw new ConfigurationErrorsException("A cadeia de conexão 'GPFA' não foi configurada.");
            Initialize(connection.ConnectionString);
        }

        public static void EnsureInitialized()
        {
            if (!_inicializado) Initialize();
        }
    }
}
