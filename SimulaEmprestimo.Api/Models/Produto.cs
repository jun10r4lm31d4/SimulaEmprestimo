using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulaEmprestimo.Api.Models
{
    public class Produto
    {
        /// <summary>
        /// ID do produto
        /// </summary>
        /// <example>1</example>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        /// <summary>
        /// Nome do produto
        /// </summary>
        /// <example>Empréstimo Pessoal</example>
        public required string Nome { get; set; }
        /// <summary>
        /// Taxa de juros anual do produto
        /// </summary>
        /// <example>18</example>
        public decimal TaxaJurosAnual { get; set; }
        /// <summary>
        /// Prazo máximo do produto
        /// </summary>
        /// <example>24</example>
        public int PrazoMaximoMeses { get; set; }
    }
}
