namespace TechfinChallenge.Application.Validators;

using FluentValidation;
using TechfinChallenge.Application.DTOs.Requests;

public class CadastrarClienteValidator : AbstractValidator<CadastrarClienteRequest>
{
    public CadastrarClienteValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Length(11, 14).WithMessage("CPF deve ter entre 11 e 14 caracteres.");

        RuleFor(x => x.ValorLimite)
            .GreaterThanOrEqualTo(0).WithMessage("Valor de limite não pode ser negativo.");
    }
}
