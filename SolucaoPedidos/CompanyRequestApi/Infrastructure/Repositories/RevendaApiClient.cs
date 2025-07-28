using CompanyRequestApi.Infrastructure.Interfaces;
using CompanyRequestApi.Models;

namespace CompanyRequestApi.Infrastructure.Repositories
{
    public class RevendaApiClient : IRevendaApiClient
    {
        private readonly HttpClient _httpClient;

        public RevendaApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RevendaDto> ObterRevendaPorId(string id)
        {
            var response = await _httpClient.GetAsync($"/api/revendas/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<RevendaDto>();
            }
            return null;
        }
    }
}
