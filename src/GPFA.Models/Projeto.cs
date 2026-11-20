using System;

namespace GPFA.Models
{
    /// <summary>
    /// Projeto científico. O coordenador é único (FK CoordenadorId) e é
    /// considerado participante implícito, não sendo repetido na tabela de
    /// participações.
    /// </summary>
    public class Projeto
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
        public string ImagemUrl { get; set; }
        public bool Destaque { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime AtualizadoEm { get; set; }

        public bool EmAndamento
        {
            get { return string.Equals(Situacao, Dominio.SituacaoProjeto.EmAndamento, StringComparison.OrdinalIgnoreCase); }
        }

        public bool Concluido
        {
            get { return string.Equals(Situacao, Dominio.SituacaoProjeto.Concluido, StringComparison.OrdinalIgnoreCase); }
        }

        public string PeriodoFormatado
        {
            get
            {
                var inicio = DataInicio.ToString("MM/yyyy");
                var fim = DataFim.HasValue ? DataFim.Value.ToString("MM/yyyy")
                        : DataFimPrevista.HasValue ? "previsão " + DataFimPrevista.Value.ToString("MM/yyyy")
                        : "em aberto";
                return inicio + " — " + fim;
            }
        }

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Titulo) || Titulo.Trim().Length < 5)
                throw new ValidacaoException("O título do projeto deve ter ao menos 5 caracteres.");
            if (string.IsNullOrWhiteSpace(Resumo))
                throw new ValidacaoException("Informe um resumo para o projeto.");
            if (CoordenadorId <= 0)
                throw new ValidacaoException("Selecione o pesquisador coordenador do projeto.");
            if (DataFimPrevista.HasValue && DataFimPrevista.Value < DataInicio)
                throw new ValidacaoException("A previsão de término não pode ser anterior ao início.");
            if (DataFim.HasValue && DataFim.Value < DataInicio)
                throw new ValidacaoException("A data de término não pode ser anterior ao início.");
            if (Concluido && !DataFim.HasValue)
                throw new ValidacaoException("Projetos concluídos devem informar a data de término.");
            if (!string.IsNullOrEmpty(Situacao) && Array.IndexOf(Dominio.SituacaoProjeto.Todos, Situacao) < 0)
                throw new ValidacaoException("Situação de projeto inválida.");
        }
    }

    public class ParticipacaoProjeto
    {
        public int ParticipacaoId { get; set; }
        public int ProjetoId { get; set; }
        public int PesquisadorId { get; set; }
        public string PesquisadorNome { get; set; }
        public string PesquisadorFotoUrl { get; set; }
        public string Papel { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? DataSaida { get; set; }

        public bool Ativa
        {
            get { return !DataSaida.HasValue; }
        }

        public string EntradaFormatada
        {
            get { return DataEntrada.ToString("MM/yyyy"); }
        }
    }

    public class EstudanteProjeto
    {
        public int EstudanteProjetoId { get; set; }
        public int EstudanteId { get; set; }
        public int ProjetoId { get; set; }
        public string EstudanteNome { get; set; }
        public string ProjetoTitulo { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? DataSaida { get; set; }
        public string TipoVinculo { get; set; }

        public bool Ativo
        {
            get { return !DataSaida.HasValue; }
        }
    }
}
