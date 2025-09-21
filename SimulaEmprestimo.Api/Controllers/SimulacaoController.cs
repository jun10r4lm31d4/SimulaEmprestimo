using Microsoft.AspNetCore.Mvc;
using SimulaEmprestimo.Api.Models;

namespace SimulaEmprestimo.Api.Controllers
{

    [Route("api/simulador")]
    public class SimulacaoController : ControllerBase
    {
        private readonly ProdutoContexto _context;

        public SimulacaoController(ProdutoContexto context)
        {
            _context = context;
        }

        /// <summary>
        /// Executa a simulação de empréstimo com base no produto, valor solicitado e prazo em meses
        /// </summary>
        /// <param name="requisicao">Dados para simulação</param>
        /// <response code="200">Resultado da simulação e memória de calculo</response>
        /// <response code="400">Requisição inválida ou prazo excede o máximo permitido pelo produto</response>
        /// <response code="404">Produto não encontrado.</response>
        [HttpPost]
        public ActionResult<SimulacaoResultado> SimularEmprestimo([FromBody] SimulacaoRequisicao requisicao)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var produto = _context.Produtos.Find(requisicao.IdProduto);
            if (produto == null)
            {
                return NotFound($"Produto não encontrado.");
            }

            if (requisicao.PrazoMeses > produto.PrazoMaximoMeses)
            {
                return BadRequest($"O prazo máximo para o produto {produto.Nome} é de {produto.PrazoMaximoMeses} meses.");
            }

            decimal taxaJurosMensal = Math.Round((decimal)Math.Pow(1.0 + (double)(produto.TaxaJurosAnual / 100m), 1.0 / 12.0) - 1.0m, 6);
            decimal valorParcela = Math.Round(requisicao.ValorSolicitado * ((decimal)Math.Pow(1.0 + decimal.ToDouble(taxaJurosMensal), requisicao.PrazoMeses) * taxaJurosMensal / ((decimal)Math.Pow(1 + decimal.ToDouble(taxaJurosMensal), decimal.ToDouble(requisicao.PrazoMeses)) - 1)), 2);
            decimal valorTotalComJuros = Math.Round(valorParcela * requisicao.PrazoMeses, 2);

            var memoriaCalculo = new List<MemoriaCalculo>();
            decimal saldoDevedor = Math.Round(requisicao.ValorSolicitado, 2);

            for (int mes = 1; mes <= requisicao.PrazoMeses; mes++)
            {
                decimal juros = Math.Round(saldoDevedor * taxaJurosMensal, 2);
                decimal amortizacao = Math.Round(valorParcela - juros, 2);
                decimal saldoDevedorFinal = Math.Round(saldoDevedor - amortizacao, 2);

                memoriaCalculo.Add(new MemoriaCalculo
                {
                    Mes = mes,
                    SaldoDevedorinicial = saldoDevedor,
                    Juros = juros,
                    Amortizacao = amortizacao,
                    SaldoDevedorFinal = saldoDevedorFinal
                });
                saldoDevedor = saldoDevedorFinal;
            }

            var resultadoSimulacao = new Simulacao
            {
                Produto = produto,
                ValorSolicitado = Math.Round(requisicao.ValorSolicitado, 2),
                PrazoMeses = requisicao.PrazoMeses,
                TaxaJurosEfetivaMensal = taxaJurosMensal,
                ValorTotalComJuros = valorTotalComJuros,
                ParcelaMensal = valorParcela,
                MemoriaCalculo = memoriaCalculo
            };

            return Ok(resultadoSimulacao);
        }
    }
}
