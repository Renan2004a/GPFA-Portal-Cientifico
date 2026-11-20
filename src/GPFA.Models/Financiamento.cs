using System;

namespace GPFA.Models
{
    public class FonteFinanciadora
    {
        public int FonteId { get; set; }
        public string Nome { get; set; }
        public string Sigla { get; set; }
        public string Tipo { get; set; }
        public string SiteUrl { get; set; }
        public int QtdFinanciamentos { get; set; }
        public int QtdProjetos { get; set; }
        public decimal TotalCaptado { get; set; }

        public string NomeExibicao
        {
            get { return string.IsNullOrWhiteSpace(Sigla) ? Nome : Sigla; }
        }

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nome) || Nome.Trim().Length < 3)
                throw new ValidacaoException("O nome da fonte financiadora deve ter ao menos 3 caracteres.");
            if (string.IsNullOrWhiteSpace(Tipo) || Array.IndexOf(Dominio.TipoFonte.Todos, Tipo) < 0)
                throw new ValidacaoException("Tipo de fonte financiadora inválido.");
        }
    }

    /// <summary>
    /// Registro de verba captada por um projeto junto a uma fonte. O valor é
    /// sempre positivo (CHECK no banco) e a soma por projeto é feita uma única
    /// vez, evitando duplicidade.
    /// </summary>
    public class Financiamento
    {
        public int FinanciamentoId { get; set; }
        public int ProjetoId { get; set; }
        public string ProjetoTitulo { get; set; }
        public string ProjetoSigla { get; set; }
        public int FonteId { get; set; }
        public string FonteNome { get; set; }
        public string FonteSigla { get; set; }
        public string Descricao { get; set; }
        public string NumeroProcesso { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Moeda { get; set; }
        public DateTime CriadoEm { get; set; }

        public string ValorFormatado
        {
            get { return Valor.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")); }
        }

        public bool Vigente
        {
            get { return !DataFim.HasValue || DataFim.Value >= DateTime.Today; }
        }

        public void Validar()
        {
            if (ProjetoId <= 0)
                throw new ValidacaoException("Selecione o projeto financiado.");
            if (FonteId <= 0)
                throw new ValidacaoException("Selecione a fonte financiadora.");
            if (string.IsNullOrWhiteSpace(Descricao))
                throw new ValidacaoException("Informe a descrição do financiamento.");
            if (Valor <= 0)
                throw new ValidacaoException("O valor do financiamento deve ser maior que zero.");
            if (DataFim.HasValue && DataFim.Value < DataInicio)
                throw new ValidacaoException("A data final do financiamento não pode ser anterior ao início.");
            if (!string.IsNullOrWhiteSpace(Moeda) && Moeda != "BRL")
                throw new ValidacaoException("O portal contabiliza valores em reais (BRL).");
        }
    }
}
