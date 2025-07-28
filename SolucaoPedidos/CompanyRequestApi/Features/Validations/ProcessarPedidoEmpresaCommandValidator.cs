using CompanyRequestApi.Features.Commands;
using FluentValidation;

namespace CompanyRequestApi.Features.Validations
{
    public class ProcessarPedidoEmpresaCommandValidator : AbstractValidator<ProcessarPedidoEmpresaCommand>
    {
        public ProcessarPedidoEmpresaCommandValidator()
        {
            RuleFor(x => x.IdRevenda)
                .NotEmpty().WithMessage("O ID da Revenda é obrigatório.");

            RuleFor(x => x.IdPedidoClienteOriginal)
                .NotEmpty().WithMessage("O ID do Pedido do Cliente Original é obrigatório.");

            RuleFor(x => x.Itens)
                .NotEmpty().WithMessage("A lista de itens não pode ser vazia.")
                .Must(items => items != null && items.All(item => item.Quantidade > 0))
                .WithMessage("Todos os itens devem ter quantidade maior que zero.");

            RuleFor(x => x.Itens.Sum(item => item.Quantidade))
                .GreaterThanOrEqualTo(1000)
                .WithMessage("A quantidade total de itens deve ser no mínimo 1000 unidades.");
        }
    }
}
