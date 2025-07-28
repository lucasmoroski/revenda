using MediatR;
using ResaleApi.Features.Commands;
using ResaleApi.Infrastructure.Interfaces;
using ResaleApi.Models;

namespace ResaleApi.Features.Handlers
{
    public class CriarRevendaHandler : IRequestHandler<CriarRevendaCommand, string>
    {
        private readonly IRevendaRepository _revendaRepository;

        public CriarRevendaHandler(IRevendaRepository revendaRepository)
        {
            _revendaRepository = revendaRepository;
        }

        public async Task<string> Handle(CriarRevendaCommand request, CancellationToken cancellationToken)
        {

            var revenda = new Revenda
            {
                Cnpj = request.Cnpj,
                RazaoSocial = request.RazaoSocial,
                NomeFantasia = request.NomeFantasia,
                Email = request.Email,
                Telefones = request.Telefones,
                Contatos = request.Contatos.Select(c => new Contato { Nome = c.Nome, Principal = c.Principal }).ToList(),
                EnderecosEntrega = request.EnderecosEntrega.Select(e => new Endereco
                {
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Complemento = e.Complemento,
                    Bairro = e.Bairro,
                    Cidade = e.Cidade,
                    Estado = e.Estado,
                    Cep = e.Cep,
                    Observacoes = e.Observacoes
                }).ToList()
            };

            await _revendaRepository.AdicionarAsync(revenda);
            return revenda.Id;
        }
    }
}
