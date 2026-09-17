namespace TechfinChallenge.Application.Validators;

using FluentValidation;
using TechfinChallenge.Application.DTOs.Requests;

public class AutorizarTransacaoValidator : AbstractValidator<AutorizarTransacaoRequest>
{
    public AutorizarTransacaoValidator()
    {
        RuleFor(x => x.IdCliente)
            .NotEmpty().WithMessage("ID do cliente é obrigatório.");

        RuleFor(x => x.ValorSimulacao)
            .GreaterThan(0).WithMessage("Valor da simulação deve ser maior que zero.");
    }
}
