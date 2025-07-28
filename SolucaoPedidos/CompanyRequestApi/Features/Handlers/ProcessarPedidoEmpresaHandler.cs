using Common.Events;
using MediatR;
using CompanyRequestApi.Features.Commands;
using CompanyRequestApi.Models;
using System.Threading;
using System.Threading.Tasks;
using CompanyRequestApi.Infrastructure.Interfaces;
using Common.Interfaces;

namespace CompanyRequestApi.Features.Handlers
{

    public class ProcessarPedidoEmpresaHandler : IRequestHandler<ProcessarPedidoEmpresaCommand, bool>
    {
        private readonly IPedidoEmpresaRepository _pedidoEmpresaRepository;
        private readonly IEmpresaApiClient _empresaApiClient;
        private readonly IMessagingService _messagingService;
        private readonly IRevendaApiClient _revendaApiClient;

        public ProcessarPedidoEmpresaHandler(IPedidoEmpresaRepository pedidoEmpresaRepository,
                                             IEmpresaApiClient empresaApiClient,
                                             IMessagingService messagingService,
                                             IRevendaApiClient revendaApiClient)
        {
            _pedidoEmpresaRepository = pedidoEmpresaRepository;
            _empresaApiClient = empresaApiClient;
            _messagingService = messagingService;
            _revendaApiClient = revendaApiClient;
        }

        public async Task<bool> Handle(ProcessarPedidoEmpresaCommand request, CancellationToken cancellationToken)
        {

            var revenda = await _revendaApiClient.ObterRevendaPorId(request.IdRevenda);
            if (revenda == null)
            {

                Console.WriteLine($"Revenda com ID {request.IdRevenda} não encontrada. Não será possível emitir o pedido para a empresa.");
                return false;
            }

            var quantidadeTotal = request.Itens.Sum(item => item.Quantidade);
            if (quantidadeTotal < 1000)
            {
                Console.WriteLine($"Pedido {request.IdPedidoClienteOriginal} da revenda {request.IdRevenda} não atinge o pedido mínimo de 1000 unidades. Quantidade total: {quantidadeTotal}.");
                await _pedidoEmpresaRepository.AdicionarAsync(new PedidoEmpresa
                {
                    IdRevenda = request.IdRevenda,
                    IdPedidoClienteOriginal = request.IdPedidoClienteOriginal,
                    QuantidadeTotalItens = quantidadeTotal,
                    Status = "Rejeitado - Pedido Mínimo",
                    Itens = request.Itens.Select(i => new ItemPedidoEmpresa { NomeProduto = i.NomeProduto, Quantidade = i.Quantidade }).ToList()
                });
                return false;
            }

            var pedidoEmpresa = new PedidoEmpresa
            {
                IdRevenda = request.IdRevenda,
                IdPedidoClienteOriginal = request.IdPedidoClienteOriginal,
                QuantidadeTotalItens = quantidadeTotal,
                Status = "Pendente",
                Itens = request.Itens.Select(i => new ItemPedidoEmpresa { NomeProduto = i.NomeProduto, Quantidade = i.Quantidade }).ToList()
            };
            await _pedidoEmpresaRepository.AdicionarAsync(pedidoEmpresa);

            try
            {
                var apiResponse = await _empresaApiClient.EmitirPedidoAsync(new EmitirPedidoEmpresaRequest
                {
                    IdRevenda = request.IdRevenda,
                    Itens = request.Itens
                });

                pedidoEmpresa.NumeroPedidoEmpresa = apiResponse.NumeroPedidoEmpresa;
                pedidoEmpresa.Status = "Confirmado";
                await _pedidoEmpresaRepository.AtualizarAsync(pedidoEmpresa);

                await _messagingService.PublishAsync(new PedidoEmpresaEmitidoEvent
                {
                    IdPedidoEmpresa = pedidoEmpresa.Id.ToString(),
                    NumeroPedidoEmpresa = pedidoEmpresa.NumeroPedidoEmpresa,
                    IdRevenda = pedidoEmpresa.IdRevenda,
                    IdPedidoClienteOriginal = pedidoEmpresa.IdPedidoClienteOriginal,
                    Status = pedidoEmpresa.Status
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao emitir pedido para a empresa: {ex.Message}");
                pedidoEmpresa.Status = "Falha - Reagendado";
                await _pedidoEmpresaRepository.AtualizarAsync(pedidoEmpresa);

                await _messagingService.PublishAsync(new PedidoEmpresaFalhaEmissaoEvent
                {
                    IdPedidoEmpresa = pedidoEmpresa.Id.ToString(),
                    IdRevenda = pedidoEmpresa.IdRevenda,
                    IdPedidoClienteOriginal = pedidoEmpresa.IdPedidoClienteOriginal,
                    MotivoFalha = ex.Message,
                    Status = pedidoEmpresa.Status
                });
                return false;
            }
        }
    }
}
