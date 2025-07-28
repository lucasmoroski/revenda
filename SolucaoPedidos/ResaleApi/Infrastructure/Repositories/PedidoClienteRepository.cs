using MongoDB.Driver;
using ResaleApi.Infrastructure.Contextos;
using ResaleApi.Infrastructure.Interfaces;
using ResaleApi.Models;

namespace ResaleApi.Infrastructure.Repositories
{
    public class PedidoClienteRepository : IPedidoClienteRepository
    {
        private readonly MongoDbContext _context;

        public PedidoClienteRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(PedidoCliente pedidoCliente)
        {
            await _context.PedidosCliente.InsertOneAsync(pedidoCliente);
        }

        public async Task<PedidoCliente> ObterPorIdAsync(string id)
        {
            return await _context.PedidosCliente.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task AtualizarAsync(PedidoCliente pedidoCliente)
        {
            await _context.PedidosCliente.ReplaceOneAsync(p => p.Id == pedidoCliente.Id, pedidoCliente);
        }
    }
}
