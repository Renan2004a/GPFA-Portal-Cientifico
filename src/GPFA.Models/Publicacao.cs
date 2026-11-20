using System;
using System.Collections.Generic;

namespace GPFA.Models
{
    public class Publicacao
    {
        public Publicacao()
        {
            Tipo = Dominio.TipoPublicacao.Artigo;
            Autores = new List<PublicacaoAutor>();
        }

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
        public DateTime CriadoEm { get; set; }
        public List<PublicacaoAutor> Autores { get; set; }

        public bool PossuiDoi
        {
            get { return !string.IsNullOrWhiteSpace(Doi); }
        }

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

        public string Citacao
        {
            get
            {
                var veiculo = string.IsNullOrWhiteSpace(Veiculo) ? "Veículo não informado" : Veiculo;
                return veiculo + " · " + Ano;
            }
        }

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Titulo) || Titulo.Trim().Length < 5)
                throw new ValidacaoException("O título da publicação deve ter ao menos 5 caracteres.");
            if (string.IsNullOrWhiteSpace(Veiculo))
                throw new ValidacaoException("Informe o veículo de publicação.");
            if (Ano < 1900 || Ano > DateTime.Now.Year + 2)
                throw new ValidacaoException("Informe um ano de publicação válido.");
            if (string.IsNullOrWhiteSpace(Doi) && string.IsNullOrWhiteSpace(LinkAcesso))
                throw new ValidacaoException("Informe o DOI ou um link de acesso à publicação.");
            if (!string.IsNullOrEmpty(Tipo) && Array.IndexOf(Dominio.TipoPublicacao.Todos, Tipo) < 0)
                throw new ValidacaoException("Tipo de publicação inválido.");
            if (Autores == null || Autores.Count == 0)
                throw new ValidacaoException("Toda publicação deve possuir ao menos um autor pesquisador.");
        }
    }

    public class PublicacaoAutor
    {
        public int PublicacaoId { get; set; }
        public int PesquisadorId { get; set; }
        public string PesquisadorNome { get; set; }
        public int OrdemAutoria { get; set; }

        public bool PrimeiroAutor
        {
            get { return OrdemAutoria == 1; }
        }
    }

    public class DestaquePublicacao
    {
        public int DestaqueId { get; set; }
        public int PesquisadorId { get; set; }
        public string PesquisadorNome { get; set; }
        public int PublicacaoId { get; set; }
        public int Ordem { get; set; }
        public DateTime CriadoEm { get; set; }
        public string Titulo { get; set; }
        public string Veiculo { get; set; }
        public int Ano { get; set; }
        public string Doi { get; set; }
    }
}
