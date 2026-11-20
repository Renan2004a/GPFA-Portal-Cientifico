using System;

namespace GPFA.Models
{
    /// <summary>
    /// Superclasse das pessoas do portal. Pesquisador e Estudante são
    /// especializações persistidas em tabelas próprias (TPT), compartilhando
    /// os dados comuns em Pessoa.
    /// </summary>
    public abstract class Pessoa
    {
        public int PessoaId { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public char TipoPessoa { get; set; }
        public string FotoUrl { get; set; }
        public string FormacaoAcademica { get; set; }
        public string AreaAtuacao { get; set; }
        public string MiniBiografia { get; set; }
        public string GoogleScholarUrl { get; set; }
        public string OrcidUrl { get; set; }
        public string LattesUrl { get; set; }
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime AtualizadoEm { get; set; }

        public abstract string TipoDescricao { get; }

        public string NomeExibicao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NomeCompleto)) return string.Empty;
                var partes = NomeCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length == 1) return partes[0];
                return partes[0] + " " + partes[partes.Length - 1];
            }
        }

        public string Iniciais
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NomeCompleto)) return "?";
                var partes = NomeCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var primeira = partes[0].Substring(0, 1);
                var ultima = partes.Length > 1 ? partes[partes.Length - 1].Substring(0, 1) : string.Empty;
                return (primeira + ultima).ToUpperInvariant();
            }
        }

        public bool PossuiLinksExternos
        {
            get
            {
                return !string.IsNullOrWhiteSpace(GoogleScholarUrl)
                    || !string.IsNullOrWhiteSpace(OrcidUrl)
                    || !string.IsNullOrWhiteSpace(LattesUrl);
            }
        }

        public virtual void ValidarDadosBasicos()
        {
            if (string.IsNullOrWhiteSpace(NomeCompleto) || NomeCompleto.Trim().Length < 3)
                throw new ValidacaoException("O nome completo deve ter ao menos 3 caracteres.");
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
                throw new ValidacaoException("Informe um e-mail válido.");
        }
    }

    public class Pesquisador : Pessoa
    {
        public Pesquisador()
        {
            TipoPessoa = Dominio.TipoPessoa.Pesquisador;
        }

        public int PesquisadorId
        {
            get { return PessoaId; }
            set { PessoaId = value; }
        }

        public string Titulacao { get; set; }
        public string Instituicao { get; set; }
        public string Departamento { get; set; }
        public string LinhaPesquisa { get; set; }
        public DateTime DataCadastro { get; set; }

        public override string TipoDescricao { get { return "Pesquisador"; } }

        public string AssinaturaInstitucional
        {
            get
            {
                var partes = new System.Collections.Generic.List<string>();
                if (!string.IsNullOrWhiteSpace(Titulacao)) partes.Add(Titulacao);
                if (!string.IsNullOrWhiteSpace(Departamento)) partes.Add(Departamento);
                if (!string.IsNullOrWhiteSpace(Instituicao)) partes.Add(Instituicao);
                return string.Join(" · ", partes);
            }
        }
    }

    public class Estudante : Pessoa
    {
        public Estudante()
        {
            TipoPessoa = Dominio.TipoPessoa.Estudante;
            Situacao = Dominio.SituacaoEstudante.Ativo;
        }

        public int EstudanteId
        {
            get { return PessoaId; }
            set { PessoaId = value; }
        }

        public string NivelAcademico { get; set; }
        public string Curso { get; set; }
        public string Situacao { get; set; }
        public DateTime DataIngresso { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string CoorientadoresExternos { get; set; }

        public override string TipoDescricao { get { return "Estudante"; } }

        public bool EstaAtivo
        {
            get { return string.Equals(Situacao, Dominio.SituacaoEstudante.Ativo, StringComparison.OrdinalIgnoreCase); }
        }

        public string DescricaoVinculo
        {
            get { return NivelAcademico + " em " + Curso; }
        }
    }
}
