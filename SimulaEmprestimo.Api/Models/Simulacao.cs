namespace SimulaEmprestimo.Api.Models
{
    public class MemoriaCalculo
    {
        /// <summary>
        /// Mês da parcela
        /// </summary>
        /// <example>1</example>
        public int Mes { get; set; }
        /// <summary>
        /// Saldo devedor inicial do mês
        /// </summary>
        /// <example>10000.00</example>
        public decimal SaldoDevedorinicial { get; set; }
        /// <summary>
        /// Valor em R$ dos juros do mês
        /// </summary>
        /// <example>100.00</example>
        public decimal Juros { get; set; }
        /// <summary>
        /// Valor em R$ da amortização do mês
        /// </summary>
        /// <example>500.00</example>
        public decimal Amortizacao { get; set; }
        /// <summary>
        /// Saldo devedor final do mês
        /// </summary>
        /// <example>9000.00</example>
        public decimal SaldoDevedorFinal { get; set; }
    }
    public class Simulacao
    {
        /// <summary>
        /// Produto selecionado para a simulação
        /// </summary>
        public required Produto Produto { get; set; }
        /// <summary>
        /// Valor solicitado na simulação
        /// </summary>
        /// <example>10000.00</example>
        public decimal ValorSolicitado { get; set; }
        /// <summary>
        /// Prazo em meses para pagamento do empréstimo
        /// </summary>
        /// <example>24</example>
        public int PrazoMeses { get; set; }
        /// <summary>
        /// Taxa de juros efetiva mensal
        /// </summary>
        /// <example>1.3</example>
        public decimal TaxaJurosEfetivaMensal { get; set; }
        /// <summary>
        /// Valor total a ser pago ao final do empréstimo, incluindo juros
        /// </summary>
        /// <example>11000.00</example>
        public decimal ValorTotalComJuros { get; set; }
        /// <summary>
        /// Valor da parcela mensal
        /// </summary>
        /// <example>900.00</example>
        public decimal ParcelaMensal { get; set; }
        /// <summary>
        /// Lista com a memória de cálculo mês a mês
        /// </summary>
        public required List<MemoriaCalculo> MemoriaCalculo { get; set; }

    }
}
