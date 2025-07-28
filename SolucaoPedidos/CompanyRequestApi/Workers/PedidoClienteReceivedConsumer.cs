using Common.Events;
using Common.Interfaces;
using CompanyRequestApi.Features.Commands;
using MediatR;

namespace CompanyRequestApi.Workers
{
    public class PedidoClienteReceivedConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessagingService _messagingService;

        public PedidoClienteReceivedConsumer(IServiceProvider serviceProvider, IMessagingService messagingService)
        {
            _serviceProvider = serviceProvider;
            _messagingService = messagingService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messagingService.Subscribe<PedidoClienteRecebidoEvent>("pedido-cliente-recebido-queue", async (message, token) =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var command = new ProcessarPedidoEmpresaCommand
                    {
                        IdPedidoClienteOriginal = message.IdPedidoCliente,
                        IdRevenda = message.IdRevenda,
                        Itens = message.Itens
                    };
                    await mediator.Send(command, token);
                }
            }, stoppingToken);

            await Task.CompletedTask;
        }
    }
}
