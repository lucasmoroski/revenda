using ResaleApi.Models;

namespace ResaleApi.Infrastructure.Interfaces
{
    public interface IPedidoClienteRepository
    {
        Task AdicionarAsync(PedidoCliente pedidoCliente);
        Task<PedidoCliente> ObterPorIdAsync(string id);
        Task AtualizarAsync(PedidoCliente pedidoCliente);
    }
}
