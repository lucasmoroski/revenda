using System.ComponentModel.DataAnnotations;

namespace CompanyRequestApi.Models
{
    public class PedidoEmpresa
    {
        [Key]
        public Guid Id { get; set; }
        public string IdRevenda { get; set; }
        public string IdPedidoClienteOriginal { get; set; }
        public int QuantidadeTotalItens { get; set; }
        public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
        public string Status { get; set; }
        public string NumeroPedidoEmpresa { get; set; }
        public List<ItemPedidoEmpresa> Itens { get; set; } = new List<ItemPedidoEmpresa>();
    }
}
