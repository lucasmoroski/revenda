using CompanyRequestApi.Infrastructure.Interfaces;
using CompanyRequestApi.Models;
using System.Net;

namespace CompanyRequestApi.Infrastructure.Repositories
{
    public class EmpresaApiClient : IEmpresaApiClient
    {
        private readonly HttpClient _httpClient;
        private int _callCount = 0; // Para simular instabilidade

        public EmpresaApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EmitirPedidoEmpresaResponse> EmitirPedidoAsync(EmitirPedidoEmpresaRequest request)
        {
            // Simulação de instabilidade: a cada 3 chamadas, retorna erro temporário
            _callCount++;
            if (_callCount % 3 == 0)
            {
                // Simulate a transient error (e.g., 500 Internal Server Error, or network issue)
                throw new HttpRequestException("Simulated API instability: Internal Server Error", null, HttpStatusCode.InternalServerError);
            }

            // Simular chamada à API da Empresa
            await Task.Delay(500); // Latência da rede
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
