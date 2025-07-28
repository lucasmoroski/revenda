using CompanyRequestApi.Infrastructure.Contextos;
using CompanyRequestApi.Infrastructure.Interfaces;
using CompanyRequestApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyRequestApi.Infrastructure.Repositories
{
    public class PedidoEmpresaRepository : IPedidoEmpresaRepository
    {
        private readonly AppDbContext _context;

        public PedidoEmpresaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(PedidoEmpresa pedido)
        {
            await _context.PedidosEmpresa.AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task<PedidoEmpresa> ObterPorIdAsync(Guid id)
        {
            return await _context.PedidosEmpresa
                                 .Include(p => p.Itens)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AtualizarAsync(PedidoEmpresa pedido)
        {
            _context.PedidosEmpresa.Update(pedido);
            await _context.SaveChangesAsync();
        }
    }
}
