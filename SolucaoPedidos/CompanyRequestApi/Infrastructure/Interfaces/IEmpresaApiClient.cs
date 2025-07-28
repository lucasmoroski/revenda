using CompanyRequestApi.Models;

namespace CompanyRequestApi.Infrastructure.Interfaces
{
    public interface IEmpresaApiClient
    {
        Task<EmitirPedidoEmpresaResponse> EmitirPedidoAsync(EmitirPedidoEmpresaRequest request);
    }
}
