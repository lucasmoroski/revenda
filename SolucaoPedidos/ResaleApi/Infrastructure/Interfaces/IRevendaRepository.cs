using ResaleApi.Models;

namespace ResaleApi.Infrastructure.Interfaces
{
    public interface IRevendaRepository
    {
        Task AdicionarAsync(Revenda revenda);
        Task<Revenda> ObterPorIdAsync(string id);
        Task<Revenda> ObterPorCnpjAsync(string cnpj);
        Task AtualizarAsync(Revenda revenda);
        Task RemoverAsync(string id);
    }
}
