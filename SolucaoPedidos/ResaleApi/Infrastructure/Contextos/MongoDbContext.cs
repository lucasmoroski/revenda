using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ResaleApi.Models;

namespace ResaleApi.Infrastructure.Contextos
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Revenda> Revendas => _database.GetCollection<Revenda>("Revendas");
        public IMongoCollection<PedidoCliente> PedidosCliente => _database.GetCollection<PedidoCliente>("PedidosCliente");
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
