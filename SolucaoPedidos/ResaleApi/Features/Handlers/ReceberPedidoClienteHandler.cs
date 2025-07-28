using Common.Events;
using Common.Interfaces;
using Common.Models;
using MediatR;
using ResaleApi.Features.Commands;
using ResaleApi.Infrastructure.Interfaces;
using ResaleApi.Models;

namespace ResaleApi.Features.Handlers
{
    public class ReceberPedidoClienteHandler : IRequestHandler<ReceberPedidoClienteCommand, PedidoClienteResponse>
    {
        private readonly IPedidoClienteRepository _pedidoClienteRepository;
        private readonly IMessagingService _messagingService;

        public ReceberPedidoClienteHandler(IPedidoClienteRepository pedidoClienteRepository, IMessagingService messagingService)
        {
            _pedidoClienteRepository = pedidoClienteRepository;
            _messagingService = messagingService;
        }

        public async Task<PedidoClienteResponse> Handle(ReceberPedidoClienteCommand request, CancellationToken cancellationToken)
        {
            var pedidoCliente = new PedidoCliente
            {
                IdRevenda = request.IdRevenda,
                IdClienteFinal = request.IdClienteFinal,
                Itens = request.Itens.Select(i => new ItemPedidoCliente { NomeProduto = i.NomeProduto, Quantidade = i.Quantidade, ValorUnitario = i.ValorUnitario }).ToList(),
                Status = "Recebido"
            };

            await _pedidoClienteRepository.AdicionarAsync(pedidoCliente);

            await _messagingService.PublishAsync(new PedidoClienteRecebidoEvent
            {
                IdPedidoCliente = pedidoCliente.Id,
                IdRevenda = pedidoCliente.IdRevenda,
                Itens = pedidoCliente.Itens.Select(i => new EventItem { NomeProduto = i.NomeProduto, Quantidade = i.Quantidade }).ToList()
            });

            return new PedidoClienteResponse
            {
                IdPedido = pedidoCliente.Id,
                ItensSolicitados = pedidoCliente.Itens
            };
        }
    }
}
