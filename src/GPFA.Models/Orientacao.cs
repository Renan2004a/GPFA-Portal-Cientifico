using System;

namespace GPFA.Models
{
    /// <summary>
    /// Orientação acadêmica. O histórico é preservado: DataFim nula indica
    /// orientação em curso. Uma única orientação principal ativa por estudante
    /// é garantida por índice filtrado no banco e pelo serviço de negócio.
    /// </summary>
    public class Orientacao
    {
        public int OrientacaoId { get; set; }
        public int EstudanteId { get; set; }
        public string EstudanteNome { get; set; }
        public int OrientadorId { get; set; }
        public string OrientadorNome { get; set; }
        public bool Principal { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Observacao { get; set; }
        public string EstudanteNivel { get; set; }
        public string EstudanteSituacao { get; set; }

        public bool Ativa
        {
            get { return !DataFim.HasValue; }
        }

        public string Vinculo
        {
            get { return Ativa ? "Atual" : "Passado"; }
        }

        public void Validar()
        {
            if (EstudanteId <= 0)
                throw new ValidacaoException("Selecione o estudante.");
            if (OrientadorId <= 0)
                throw new ValidacaoException("O orientador principal deve ser um pesquisador cadastrado.");
            if (DataFim.HasValue && DataFim.Value < DataInicio)
                throw new ValidacaoException("A data de término da orientação não pode ser anterior ao início.");
        }
    }
}
