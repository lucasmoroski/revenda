using MediatR;
using ResaleApi.Models;

namespace ResaleApi.Features.Commands
{
    public class ReceberPedidoClienteCommand : IRequest<PedidoClienteResponse>
    {
        public string IdRevenda { get; set; }
        public string IdClienteFinal { get; set; }
        public List<ItemPedidoClienteDto> Itens { get; set; } = new List<ItemPedidoClienteDto>();
    }

    public class ItemPedidoClienteDto
    {
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
    }

    public class PedidoClienteResponse
    {
        public string IdPedido { get; set; }
        public List<ItemPedidoCliente> ItensSolicitados { get; set; }
    }
}
