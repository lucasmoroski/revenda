using ResaleApi.Infrastructure.Contextos;
using ResaleApi.Models;
using MongoDB.Driver;
using ResaleApi.Infrastructure.Interfaces;

namespace ResaleApi.Infrastructure.Repositories
{
    public class RevendaRepository : IRevendaRepository
    {
        private readonly MongoDbContext _context;

        public RevendaRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Revenda revenda)
        {
            await _context.Revendas.InsertOneAsync(revenda);
        }

        public async Task<Revenda> ObterPorIdAsync(string id)
        {
            return await _context.Revendas.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Revenda> ObterPorCnpjAsync(string cnpj)
        {
            return await _context.Revendas.Find(r => r.Cnpj == cnpj).FirstOrDefaultAsync();
        }

        public async Task AtualizarAsync(Revenda revenda)
        {
            await _context.Revendas.ReplaceOneAsync(r => r.Id == revenda.Id, revenda);
        }

        public async Task RemoverAsync(string id)
        {
            await _context.Revendas.DeleteOneAsync(r => r.Id == id);
        }
    }
}