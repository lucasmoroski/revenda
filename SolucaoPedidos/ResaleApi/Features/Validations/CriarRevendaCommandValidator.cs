using FluentValidation;
using ResaleApi.Features.Commands;

namespace ResaleApi.Features.Validations
{
    public class CriarRevendaCommandValidator : AbstractValidator<CriarRevendaCommand>
    {
        public CriarRevendaCommandValidator()
        {
            RuleFor(x => x.Cnpj).NotEmpty().WithMessage("CNPJ é obrigatório.");
            // Adicionar validação de CNPJ válido (pode usar uma biblioteca externa ou regex)
            RuleFor(x => x.RazaoSocial).NotEmpty().WithMessage("Razão Social é obrigatória.");
            RuleFor(x => x.NomeFantasia).NotEmpty().WithMessage("Nome Fantasia é obrigatório.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email é obrigatório.").EmailAddress().WithMessage("Email inválido.");
            RuleFor(x => x.Contatos).NotEmpty().WithMessage("Deve haver pelo menos um contato.").Must(c => c.Any(ct => ct.Principal)).WithMessage("Deve haver um contato principal.");
            RuleFor(x => x.EnderecosEntrega).NotEmpty().WithMessage("Deve haver pelo menos um endereço de entrega.");
        }
    }
}
