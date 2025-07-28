using Microsoft.EntityFrameworkCore;
using CompanyRequestApi.Models;

namespace CompanyRequestApi.Infrastructure.Contextos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PedidoEmpresa> PedidosEmpresa { get; set; }
        public DbSet<ItemPedidoEmpresa> ItensPedidoEmpresa { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PedidoEmpresa>()
                .HasMany(p => p.Itens)
                .WithOne(i => i.PedidoEmpresa)
                .HasForeignKey(i => i.PedidoEmpresaId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
