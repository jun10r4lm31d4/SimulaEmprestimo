using Microsoft.EntityFrameworkCore;

namespace SimulaEmprestimo.Api.Models
{
    public class ProdutoContexto : DbContext
    {
        public ProdutoContexto(DbContextOptions<ProdutoContexto> options) : base(options) { }
        public DbSet<Produto> Produtos { get; set; }
    }
}
