using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ResaleApi.Models
{
    public class Revenda
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string Email { get; set; }
        public List<string> Telefones { get; set; } = new List<string>();
        public List<Contato> Contatos { get; set; } = new List<Contato>();
        public List<Endereco> EnderecosEntrega { get; set; } = new List<Endereco>();
    }
}
