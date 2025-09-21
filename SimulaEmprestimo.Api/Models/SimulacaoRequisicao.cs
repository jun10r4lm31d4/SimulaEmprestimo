namespace SimulaEmprestimo.Api.Models
{
    public class SimulacaoRequisicao
    {
        /// <summary>
        /// ID do produto usado na simulação
        /// </summary>
        /// <example>1</example>
        public int IdProduto { get; set; }
        /// <summary>
        /// Valor solicitado na simulação
        /// </summary>
        /// <example>10000.00</example>
        public decimal ValorSolicitado { get; set; }
        /// <summary>
        /// Prazo em meses para pagamento do empréstimo
        /// </summary>
        /// <example>12</example>
        public int PrazoMeses { get; set; }
    }

    public class SimulacaoResultado
    {
        /// <summary>
        /// Resultado da simulação
        /// </summary>
        public required Simulacao Resultado { get; set; }
    }
}
