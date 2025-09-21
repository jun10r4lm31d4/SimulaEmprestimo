using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimulaEmprestimo.Api.Controllers;
using SimulaEmprestimo.Api.Models;
using Xunit;

namespace SimulaEmprestimo.Api.Tests.Controllers
{
    public class ProdutosControllerTestes
    {
        private readonly ProdutoContexto _context;
        private readonly ProdutosController _controller;

        public ProdutosControllerTestes()
        {
            // Configurar o banco de dados em memória
            var options = new DbContextOptionsBuilder<ProdutoContexto>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ProdutoContexto(options);
            
            _context.Produtos.AddRange(
                new Produto { Nome = "Empréstimo Pessoal", TaxaJurosAnual = 5.0m, PrazoMaximoMeses = 12 },
                new Produto { Nome = "Empréstimo Consignado", TaxaJurosAnual = 3.5m, PrazoMaximoMeses = 24 },
                new Produto { Nome = "Financiamento Veicular", TaxaJurosAnual = 7.2m, PrazoMaximoMeses = 36 }
            );
            _context.SaveChanges();
            
            _controller = new ProdutosController(_context);
        }

        [Fact]
        public async Task ListaProdutos_QuandoChamado_DeveRetornarTodosOsProdutos()
        {
            // Act
            var result = await _controller.ListaProdutos();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Produto>>>(result);
            var returnValue = Assert.IsType<List<Produto>>(actionResult.Value);
            Assert.Equal(3, returnValue.Count);
        }

        [Fact]
        public async Task ListaProdutos_QuandoNaoExistemProdutos_DeveRetornarListaVazia()
        {
            // Arrange
            _context.Produtos.RemoveRange(_context.Produtos);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ListaProdutos();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Produto>>>(result);
            var returnValue = Assert.IsType<List<Produto>>(actionResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task IncluiProduto_QuandoDadosValidos_DeveCriarProduto()
        {
            // Arrange
            var produto = new Produto { Nome = "Novo Empréstimo", TaxaJurosAnual = 4.5m, PrazoMaximoMeses = 18 };

            // Act
            var result = await _controller.IncluiProduto(produto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Produto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            
            var returnedProduto = Assert.IsType<Produto>(okResult.Value);
            Assert.Equal("Novo Empréstimo", returnedProduto.Nome);
            Assert.Equal(4.5m, returnedProduto.TaxaJurosAnual);
            Assert.Equal(18, returnedProduto.PrazoMaximoMeses);
            
            var produtoNoBanco = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Nome == "Novo Empréstimo");
            Assert.NotNull(produtoNoBanco);
            Assert.True(produtoNoBanco.Id > 0);
            Assert.Equal(4.5m, produtoNoBanco.TaxaJurosAnual);
        }

        [Fact]
        public async Task IncluiProduto_QuandoModelStateInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var produto = new Produto { Nome = "", TaxaJurosAnual = -1, PrazoMaximoMeses = 0 };
            _controller.ModelState.AddModelError("Nome", "Nome é obrigatório");

            // Act
            var result = await _controller.IncluiProduto(produto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Produto>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task ExibeProduto_QuandoIdExistente_DeveRetornarProduto()
        {
            // Arrange
            var produtoExistente = await _context.Produtos.FirstAsync();
            var idExistente = produtoExistente.Id;

            // Act
            var result = await _controller.ExibeProduto(idExistente);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Produto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<Produto>(okResult.Value);
            Assert.Equal(produtoExistente.Nome, returnValue.Nome);
        }

        [Fact]
        public async Task ExibeProduto_QuandoIdInexistente_DeveRetornarNotFound()
        {
            // Act
            var result = await _controller.ExibeProduto(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Produto>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("Produto não encontrado", notFoundResult.Value);
        }

        [Fact]
        public async Task ExibeProduto_QuandoModelStateInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Id", "Id inválido");

            // Act
            var result = await _controller.ExibeProduto(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Produto>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task AlteraProduto_QuandoDadosValidos_DeveAtualizarProduto()
        {
            // Arrange
            var produtoExistente = await _context.Produtos.FirstAsync();
            var produtoAtualizado = new Produto { 
                Nome = "Empréstimo Atualizado", 
                TaxaJurosAnual = 4.5m, 
                PrazoMaximoMeses = 18 
            };

            // Act
            var result = await _controller.AlteraProduto(produtoExistente.Id, produtoAtualizado);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            
            var produtoNoBanco = await _context.Produtos.FindAsync(produtoExistente.Id);
            Assert.NotNull(produtoNoBanco);
            Assert.Equal("Empréstimo Atualizado", produtoNoBanco.Nome);
            Assert.Equal(4.5m, produtoNoBanco.TaxaJurosAnual);
            Assert.Equal(18, produtoNoBanco.PrazoMaximoMeses);
        }

        [Fact]
        public async Task AlteraProduto_QuandoIdInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var produto = new Produto { 
                Nome = "Produto Inexistente", 
                TaxaJurosAnual = 5.0m, 
                PrazoMaximoMeses = 12 
            };

            // Act
            var result = await _controller.AlteraProduto(999, produto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Produto não encontrado", notFoundResult.Value);
        }

        [Fact]
        public async Task AlteraProduto_QuandoModelStateInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var produto = new Produto { Nome = "", TaxaJurosAnual = -1, PrazoMaximoMeses = 0 };
            _controller.ModelState.AddModelError("Nome", "Nome é obrigatório");

            // Act
            var result = await _controller.AlteraProduto(1, produto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeletaProduto_QuandoIdExistente_DeveRemoverProduto()
        {
            // Arrange - Obter um produto existente
            var produtoExistente = await _context.Produtos.FirstAsync();
            var idExistente = produtoExistente.Id;

            // Act
            var result = await _controller.DeletaProduto(idExistente);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deletado com sucesso", okResult.Value);
            
            // Verificar se foi removido do banco
            var produtoRemovido = await _context.Produtos.FindAsync(idExistente);
            Assert.Null(produtoRemovido);
        }

        [Fact]
        public async Task DeletaProduto_QuandoIdInexistente_DeveRetornarNotFound()
        {
            // Act
            var result = await _controller.DeletaProduto(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Produto não encontrado", notFoundResult.Value);
        }

        [Fact]
        public async Task DeletaProduto_QuandoModelStateInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Id", "Id inválido");

            // Act
            var result = await _controller.DeletaProduto(1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}