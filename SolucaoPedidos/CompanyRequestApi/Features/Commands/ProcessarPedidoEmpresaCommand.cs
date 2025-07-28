using Common.Events;
using Common.Models;
using MediatR;

namespace CompanyRequestApi.Features.Commands
{
    public class ProcessarPedidoEmpresaCommand : IRequest<bool>
    {
        public string IdPedidoClienteOriginal { get; set; }
        public string IdRevenda { get; set; }
        public List<EventItem> Itens { get; set; }
    }
}
