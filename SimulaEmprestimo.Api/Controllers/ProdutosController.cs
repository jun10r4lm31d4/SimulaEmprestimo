using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimulaEmprestimo.Api.Models;

namespace SimulaEmprestimo.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutoContexto _contexto;

        public ProdutosController(ProdutoContexto contexto)
        {
            _contexto = contexto;
        }

        /// <summary>
        /// Obtém todos os produtos cadastrados
        /// </summary>
        /// <response code="200">Dados dos produtos cadastrados</response>
        /// <response code="400">Requisição inválida</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> ListaProdutos()
        {
            return await _contexto.Produtos.ToListAsync();
        }

        /// <summary>
        /// Cria um novo produto
        /// </summary>
        /// <param name="produto">Dados do produto</param>
        /// <response code="200">Dados do produto criado</response>
        /// <response code="400">Requisição inválida</response>
        [HttpPost]
        public async Task<ActionResult<Produto>> IncluiProduto([FromBody]Produto produto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _contexto.Produtos.Add(produto);
            await _contexto.SaveChangesAsync();
            return Ok(produto);
        }

        /// <summary>
        /// Obtém um produto específico pelo ID
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <response code="200">Dados do produto encontrado</response>
        /// <response code="400">Requisição inválida</response>
        /// <response code="404">Produto não encontrado</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> ExibeProduto(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var produto = await _contexto.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound("Produto não encontrado");
            }

            return Ok(produto);
        }

        /// <summary>
        /// Altera um produto existente
        /// </summary>
        /// <param name="produto">Dados do produto</param>
        /// <param name="id">ID do produto</param>
        /// <response code="200">Dados do produto alterado</response>
        /// <response code="400">Requisição inválida</response>
        /// <response code="404">Produto não encontrado</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> AlteraProduto(int id, [FromBody]Produto produto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _contexto.Produtos
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(p => p
                    .SetProperty(p => p.Nome, produto.Nome)
                    .SetProperty(p => p.TaxaJurosAnual, produto.TaxaJurosAnual)
                    .SetProperty(p => p.PrazoMaximoMeses, produto.PrazoMaximoMeses)
                );
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _contexto.Produtos.AnyAsync(p => p.Id == id))
                {
                    return NotFound("Produto não encontrado");
                }
                else
                {
                    throw;
                }
            }

            return Ok(produto);
        }

        /// <summary>
        /// Deleta um produto existente
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <response code="200">Deletado com sucesso</response>
        /// <response code="400">Requisição inválida</response>
        /// <response code="404">Produto não encontrado</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletaProduto(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var produto = await _contexto.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado");
            }

            _contexto.Produtos.Remove(produto);
            await _contexto.SaveChangesAsync();
            return Ok("Deletado com sucesso");
        }
    }
}
