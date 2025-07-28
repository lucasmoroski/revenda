using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CompanyRequestApi.Models
{
    public class ItemPedidoEmpresa
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PedidoEmpresaId { get; set; } // Foreign key
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; } // Opcional, se a empresa usar
        [ForeignKey("PedidoEmpresaId")]
        public PedidoEmpresa PedidoEmpresa { get; set; }
    }
}
