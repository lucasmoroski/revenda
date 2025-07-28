using CompanyRequestApi.Models;

namespace CompanyRequestApi.Infrastructure.Interfaces
{
    public interface IRevendaApiClient
    {
        Task<RevendaDto> ObterRevendaPorId(string id);
    }
}
