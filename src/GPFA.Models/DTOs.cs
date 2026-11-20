using System;
using System.Collections.Generic;
using System.Globalization;

namespace GPFA.Models
{
    internal static class Formato
    {
        public static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public static string Moeda(decimal valor)
        {
            return valor.ToString("C0", PtBr);
        }

        public static string MoedaDetalhada(decimal valor)
        {
            return valor.ToString("C2", PtBr);
        }

        public static string Data(DateTime? data)
        {
            return data.HasValue ? data.Value.ToString("dd/MM/yyyy") : "—";
        }

        public static string MesAno(DateTime? data)
        {
            return data.HasValue ? data.Value.ToString("MM/yyyy") : "—";
        }
    }

    public class ProjetoResumo
    {
        public int ProjetoId { get; set; }
        public string Titulo { get; set; }
        public string Sigla { get; set; }
        public string Resumo { get; set; }
        public string Descricao { get; set; }
        public string LinhaPesquisa { get; set; }
        public string Situacao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFimPrevista { get; set; }
        public DateTime? DataFim { get; set; }
        public int CoordenadorId { get; set; }
        public string CoordenadorNome { get; set; }
        public string CoordenadorFotoUrl { get; set; }
        public string ImagemUrl { get; set; }
        public bool Destaque { get; set; }
        public int QtdPesquisadores { get; set; }
        public int QtdEstudantes { get; set; }
        public int QtdPublicacoes { get; set; }
        public decimal TotalCaptado { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime AtualizadoEm { get; set; }

        public string TotalFormatado { get { return Formato.Moeda(TotalCaptado); } }
        public string InicioFormatado { get { return Formato.MesAno(DataInicio); } }
        public string FimFormatado
        {
            get
            {
                if (DataFim.HasValue) return Formato.MesAno(DataFim);
                if (DataFimPrevista.HasValue) return "previsão " + Formato.MesAno(DataFimPrevista);
                return "em aberto";
            }
        }
        public string PeriodoFormatado { get { return InicioFormatado + " — " + FimFormatado; } }
        public string SituacaoCss
        {
            get
            {
                switch (Situacao)
                {
                    case Dominio.SituacaoProjeto.EmAndamento: return "status-andamento";
                    case Dominio.SituacaoProjeto.Concluido: return "status-concluido";
                    case Dominio.SituacaoProjeto.Suspenso: return "status-suspenso";
                    default: return "status-planejamento";
                }
            }
        }
        public string CoordenadorIniciais
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CoordenadorNome)) return "?";
                var partes = CoordenadorNome.Split(' ');
                return (partes[0].Substring(0, 1) + (partes.Length > 1 ? partes[partes.Length - 1].Substring(0, 1) : string.Empty)).ToUpperInvariant();
            }
        }
    }

    public class PesquisadorResumo
    {
        public int PesquisadorId { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string FotoUrl { get; set; }
        public string FormacaoAcademica { get; set; }
        public string AreaAtuacao { get; set; }
        public string MiniBiografia { get; set; }
        public string GoogleScholarUrl { get; set; }
        public string OrcidUrl { get; set; }
        public string LattesUrl { get; set; }
        public bool Ativo { get; set; }
        public string Titulacao { get; set; }
        public string Instituicao { get; set; }
        public string Departamento { get; set; }
        public string LinhaPesquisa { get; set; }
        public int QtdProjetosCoordenados { get; set; }
        public int QtdProjetos { get; set; }
        public int QtdOrientandosAtuais { get; set; }
        public int QtdOrientandosPassados { get; set; }
        public int QtdPublicacoes { get; set; }
        public decimal TotalCaptado { get; set; }

        public string TotalFormatado { get { return Formato.Moeda(TotalCaptado); } }
        public string Iniciais
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NomeCompleto)) return "?";
                var partes = NomeCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return (partes[0].Substring(0, 1) + (partes.Length > 1 ? partes[partes.Length - 1].Substring(0, 1) : string.Empty)).ToUpperInvariant();
            }
        }
        public string NomeExibicao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NomeCompleto)) return string.Empty;
                var partes = NomeCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return partes.Length == 1 ? partes[0] : partes[0] + " " + partes[partes.Length - 1];
            }
        }
        public bool PossuiLinks
        {
            get { return !string.IsNullOrWhiteSpace(GoogleScholarUrl) || !string.IsNullOrWhiteSpace(OrcidUrl) || !string.IsNullOrWhiteSpace(LattesUrl); }
        }
        public string TitulacaoExibicao
        {
            get { return string.IsNullOrWhiteSpace(Titulacao) ? "Pesquisador(a)" : Titulacao; }
        }
    }

    public class EstudanteResumo
    {
        public int EstudanteId { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string FotoUrl { get; set; }
        public string FormacaoAcademica { get; set; }
        public string AreaAtuacao { get; set; }
        public string NivelAcademico { get; set; }
        public string Curso { get; set; }
        public string Situacao { get; set; }
        public DateTime DataIngresso { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string CoorientadoresExternos { get; set; }
        public int? OrientadorId { get; set; }
        public string OrientadorNome { get; set; }
        public int QtdProjetosAtivos { get; set; }
        public string ProjetosAtivos { get; set; }
        public List<ProjetoResumo> Projetos { get; set; }

        public EstudanteResumo()
        {
            Projetos = new List<ProjetoResumo>();
        }

        public string Iniciais
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NomeCompleto)) return "?";
                var partes = NomeCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return (partes[0].Substring(0, 1) + (partes.Length > 1 ? partes[partes.Length - 1].Substring(0, 1) : string.Empty)).ToUpperInvariant();
            }
        }
        public string IngresoFormatado { get { return Formato.Data(DataIngresso); } }
        public string SituacaoCss
        {
            get
            {
                switch (Situacao)
                {
                    case Dominio.SituacaoEstudante.Ativo: return "status-andamento";
                    case Dominio.SituacaoEstudante.Concluido: return "status-concluido";
                    case Dominio.SituacaoEstudante.Trancado: return "status-suspenso";
                    default: return "status-neutro";
                }
            }
        }
        public string DescricaoVinculo
        {
            get { return NivelAcademico + " em " + Curso; }
        }
    }

    public class OrientandoResumo
    {
        public int OrientacaoId { get; set; }
        public int EstudanteId { get; set; }
        public string EstudanteNome { get; set; }
        public string EstudanteEmail { get; set; }
        public string EstudanteFotoUrl { get; set; }
        public string NivelAcademico { get; set; }
        public string Curso { get; set; }
        public string Situacao { get; set; }
        public string CoorientadoresExternos { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Vinculo { get; set; }
        public string PeriodoFormatado
        {
            get { return Formato.Data(DataInicio) + " — " + (DataFim.HasValue ? Formato.Data(DataFim) : "atual"); }
        }
    }

    public class PublicacaoResumo
    {
        public int PublicacaoId { get; set; }
        public string Titulo { get; set; }
        public string Veiculo { get; set; }
        public int Ano { get; set; }
        public string Tipo { get; set; }
        public string Doi { get; set; }
        public string LinkAcesso { get; set; }
        public int? ProjetoId { get; set; }
        public string ProjetoTitulo { get; set; }
        public string ProjetoSigla { get; set; }
        public string Autores { get; set; }
        public DateTime CriadoEm { get; set; }

        public string UrlAcesso
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(LinkAcesso)) return LinkAcesso;
                if (!string.IsNullOrWhiteSpace(Doi))
                {
                    var doi = Doi.Trim();
                    if (doi.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return doi;
                    if (doi.StartsWith("10.", StringComparison.Ordinal)) return "https://doi.org/" + doi;
                }
                return null;
            }
        }
        public string Citacao { get { return Veiculo + " · " + Ano; } }
    }

    public class FinanciamentoResumo
    {
        public int FinanciamentoId { get; set; }
        public int ProjetoId { get; set; }
        public string ProjetoTitulo { get; set; }
        public int FonteId { get; set; }
        public string FonteNome { get; set; }
        public string FonteSigla { get; set; }
        public string Descricao { get; set; }
        public string NumeroProcesso { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public string ValorFormatado { get { return Formato.MoedaDetalhada(Valor); } }
        public string PeriodoFormatado
        {
            get { return Formato.MesAno(DataInicio) + " — " + (DataFim.HasValue ? Formato.MesAno(DataFim) : "vigente"); }
        }
    }

    public class ProjetoDetalhe
    {
        public ProjetoDetalhe()
        {
            Participantes = new List<PesquisadorResumo>();
            Estudantes = new List<EstudanteResumo>();
            Financiamentos = new List<FinanciamentoResumo>();
            Publicacoes = new List<PublicacaoResumo>();
        }

        public ProjetoResumo Projeto { get; set; }
        public PesquisadorResumo Coordenador { get; set; }
        public List<PesquisadorResumo> Participantes { get; set; }
        public List<EstudanteResumo> Estudantes { get; set; }
        public List<FinanciamentoResumo> Financiamentos { get; set; }
        public List<PublicacaoResumo> Publicacoes { get; set; }
        public decimal TotalCaptado
        {
            get
            {
                decimal total = 0;
                foreach (var f in Financiamentos) total += f.Valor;
                return total;
            }
        }
    }

    public class PesquisadorPerfil
    {
        public PesquisadorPerfil()
        {
            Projetos = new List<ProjetoResumo>();
            ProjetosCoordenados = new List<ProjetoResumo>();
            OrientandosAtuais = new List<OrientandoResumo>();
            OrientandosPassados = new List<OrientandoResumo>();
            Publicacoes = new List<PublicacaoResumo>();
            Destaques = new List<PublicacaoResumo>();
        }

        public PesquisadorResumo Pesquisador { get; set; }
        public List<ProjetoResumo> Projetos { get; set; }
        public List<ProjetoResumo> ProjetosCoordenados { get; set; }
        public List<OrientandoResumo> OrientandosAtuais { get; set; }
        public List<OrientandoResumo> OrientandosPassados { get; set; }
        public List<PublicacaoResumo> Publicacoes { get; set; }
        public List<PublicacaoResumo> Destaques { get; set; }
    }

    public class EstudantePerfil
    {
        public EstudantePerfil()
        {
            Orientacoes = new List<Orientacao>();
            Projetos = new List<ProjetoResumo>();
        }

        public EstudanteResumo Estudante { get; set; }
        public Orientacao OrientacaoAtual { get; set; }
        public List<Orientacao> Orientacoes { get; set; }
        public List<ProjetoResumo> Projetos { get; set; }
    }

    public class DashboardIndicadores
    {
        public int TotalPesquisadores { get; set; }
        public int TotalEstudantes { get; set; }
        public int TotalEstudantesAtivos { get; set; }
        public int TotalProjetos { get; set; }
        public int TotalProjetosEmAndamento { get; set; }
        public int TotalPublicacoes { get; set; }
        public int TotalFinanciamentos { get; set; }
        public decimal TotalCaptado { get; set; }
        public int TotalFontes { get; set; }

        public string TotalCaptadoFormatado { get { return Formato.Moeda(TotalCaptado); } }
    }

    public class RelatorioContagem
    {
        public string Rotulo { get; set; }
        public int Quantidade { get; set; }
        public decimal? Valor { get; set; }
        public string ValorFormatado
        {
            get { return Valor.HasValue ? Formato.MoedaDetalhada(Valor.Value) : "—"; }
        }
        public decimal Percentual { get; set; }
    }

    public class RelatorioFinanceiroLinha
    {
        public string Grupo { get; set; }
        public string Projeto { get; set; }
        public string Papel { get; set; }
        public decimal Valor { get; set; }
        public string ValorFormatado { get { return Formato.MoedaDetalhada(Valor); } }
    }

    public class DestaqueAdminResumo
    {
        public int DestaqueId { get; set; }
        public int PesquisadorId { get; set; }
        public string PesquisadorNome { get; set; }
        public int PublicacaoId { get; set; }
        public string Titulo { get; set; }
        public string Veiculo { get; set; }
        public int Ano { get; set; }
        public int Ordem { get; set; }
    }

    public class OpcaoSelecao
    {
        public int Id { get; set; }
        public string Texto { get; set; }

        public override string ToString()
        {
            return Texto;
        }
    }

    public class FiltroPublicacoes
    {
        public int? Ano { get; set; }
        public int? PesquisadorId { get; set; }
        public int? ProjetoId { get; set; }
        public string Termo { get; set; }
    }

    public class ResultadoOperacao
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public int? Id { get; set; }

        public static ResultadoOperacao Ok(string mensagem, int? id = null)
        {
            return new ResultadoOperacao { Sucesso = true, Mensagem = mensagem, Id = id };
        }
    }

    public class OrientadorContagem
    {
        public int OrientadorId { get; set; }
        public string OrientadorNome { get; set; }
        public int Atuais { get; set; }
        public int Passados { get; set; }
        public int Total { get { return Atuais + Passados; } }
    }

    public class AlertaIntegridade
    {
        public string Alerta { get; set; }
        public int RegistroId { get; set; }
        public string Detalhe { get; set; }
    }

    public class VinculosPesquisador
    {
        public int ProjetosCoordenados { get; set; }
        public int Participacoes { get; set; }
        public int Orientacoes { get; set; }
        public int Publicacoes { get; set; }
        public int Usuarios { get; set; }
        public int Total
        {
            get { return ProjetosCoordenados + Participacoes + Orientacoes + Publicacoes + Usuarios; }
        }
    }

    public class VinculosProjeto
    {
        public int Publicacoes { get; set; }
        public int Financiamentos { get; set; }
        public int Participacoes { get; set; }
        public int Estudantes { get; set; }
        public int Total
        {
            get { return Publicacoes + Financiamentos + Participacoes + Estudantes; }
        }
    }
}