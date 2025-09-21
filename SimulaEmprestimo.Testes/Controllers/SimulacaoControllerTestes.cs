using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimulaEmprestimo.Api.Controllers;
using SimulaEmprestimo.Api.Models;
using Xunit;

namespace SimulaEmprestimo.Api.Tests.Controllers
{
    public class SimulacaoControllerTests : IDisposable
    {
        private readonly ProdutoContexto _context;
        private readonly SimulacaoController _controller;

        public SimulacaoControllerTests()
        {
            // Configurar o banco de dados em memória
            var options = new DbContextOptionsBuilder<ProdutoContexto>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ProdutoContexto(options);
            
            // Popular o banco com produtos de teste
            _context.Produtos.AddRange(
                new Produto { Nome = "Empréstimo Pessoal", TaxaJurosAnual = 12.0m, PrazoMaximoMeses = 24 },
                new Produto { Nome = "Empréstimo Consignado", TaxaJurosAnual = 8.0m, PrazoMaximoMeses = 36 }
            );
            _context.SaveChanges();
            
            _controller = new SimulacaoController(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

       
        [Fact]
        public void SimularEmprestimo_QuandoProdutoNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var requisicao = new SimulacaoRequisicao
            {
                IdProduto = 999, // ID inexistente
                ValorSolicitado = 10000.00m,
                PrazoMeses = 12
            };

            // Act
            var result = _controller.SimularEmprestimo(requisicao);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SimulacaoResultado>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("Produto não encontrado.", notFoundResult.Value);
        }

        [Fact]
        public void SimularEmprestimo_QuandoPrazoExcedeMaximo_DeveRetornarBadRequest()
        {
            // Arrange
            var produto = _context.Produtos.First();
            var requisicao = new SimulacaoRequisicao
            {
                IdProduto = produto.Id,
                ValorSolicitado = 10000.00m,
                PrazoMeses = produto.PrazoMaximoMeses + 1 // Prazo maior que o máximo
            };

            // Act
            var result = _controller.SimularEmprestimo(requisicao);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SimulacaoResultado>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Contains($"O prazo máximo para o produto {produto.Nome} é de {produto.PrazoMaximoMeses} meses.", 
                badRequestResult.Value.ToString());
        }

        [Fact]
        public void SimularEmprestimo_QuandoModelStateInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var requisicao = new SimulacaoRequisicao
            {
                IdProduto = 1,
                ValorSolicitado = -100.00m, // Valor inválido
                PrazoMeses = 0 // Prazo inválido
            };

            _controller.ModelState.AddModelError("ValorSolicitado", "Valor deve ser positivo");
            _controller.ModelState.AddModelError("PrazoMeses", "Prazo deve ser maior que zero");

            // Act
            var result = _controller.SimularEmprestimo(requisicao);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SimulacaoResultado>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public void SimularEmprestimo_QuandoValorZero_DeveRetornarBadRequest()
        {
            // Arrange
            var requisicao = new SimulacaoRequisicao
            {
                IdProduto = 1,
                ValorSolicitado = 0.00m, // Valor zero
                PrazoMeses = 12
            };

            _controller.ModelState.AddModelError("ValorSolicitado", "Valor deve ser maior que zero");

            // Act
            var result = _controller.SimularEmprestimo(requisicao);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SimulacaoResultado>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }
    }
}