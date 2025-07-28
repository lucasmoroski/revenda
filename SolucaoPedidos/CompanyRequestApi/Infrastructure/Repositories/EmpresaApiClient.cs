using CompanyRequestApi.Infrastructure.Interfaces;
using CompanyRequestApi.Models;
using System.Net;

namespace CompanyRequestApi.Infrastructure.Repositories
{
    public class EmpresaApiClient : IEmpresaApiClient
    {
        private readonly HttpClient _httpClient;
        private int _callCount = 0;

        public EmpresaApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EmitirPedidoEmpresaResponse> EmitirPedidoAsync(EmitirPedidoEmpresaRequest request)
        {

            _callCount++;
            if (_callCount % 3 == 0)
            {
                throw new HttpRequestException("Simulated API instability: Internal Server Error", null, HttpStatusCode.InternalServerError);
            }

            await Task.Delay(500);
            var numeroPedido = $"EMP-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            var itensRetornados = request.Itens.Select(i => new ItemPedidoEmpresa { NomeProduto = i.NomeProduto, Quantidade = i.Quantidade }).ToList();

            return new EmitirPedidoEmpresaResponse
            {
                NumeroPedidoEmpresa = numeroPedido,
                ItensSolicitados = itensRetornados
            };
        }
    }
}
