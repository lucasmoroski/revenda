using MediatR;
using ResaleApi.Models;

namespace ResaleApi.Features.Commands
{
    public class CriarRevendaCommand : IRequest<string>
    {
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string Email { get; set; }
        public List<string> Telefones { get; set; } = new List<string>();
        public List<Contato> Contatos { get; set; } = new List<Contato>();
        public List<Endereco> EnderecosEntrega { get; set; } = new List<Endereco>();
    }
}
