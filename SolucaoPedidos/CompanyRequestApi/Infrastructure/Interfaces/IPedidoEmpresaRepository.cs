using CompanyRequestApi.Models;

namespace CompanyRequestApi.Infrastructure.Interfaces
{
    public interface IPedidoEmpresaRepository
    {
        Task AdicionarAsync(PedidoEmpresa pedido);
        Task<PedidoEmpresa> ObterPorIdAsync(Guid id);
        Task AtualizarAsync(PedidoEmpresa pedido);
    }
}
