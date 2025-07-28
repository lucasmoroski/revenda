using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ResaleApi.Models
{
    public class PedidoCliente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string IdRevenda { get; set; }
        public string IdClienteFinal { get; set; }
        public List<ItemPedidoCliente> Itens { get; set; } = new List<ItemPedidoCliente>();
        public DateTime DataPedido { get; set; } = DateTime.UtcNow;
        public string Status { get; set; }
    }
}
