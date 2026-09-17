namespace TechfinChallenge.Domain.ValueObjects;

using TechfinChallenge.Domain.Exceptions;

public record Cpf
{
    public string Numero { get; }

    public Cpf(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new DomainException("CPF é obrigatório.");

        var limpo = numero.Replace(".", "").Replace("-", "").Trim();

        if (limpo.Length != 11 || !limpo.All(char.IsDigit))
            throw new DomainException("CPF deve conter 11 dígitos.");

        if (limpo.Distinct().Count() == 1)
            throw new DomainException("CPF inválido.");

        if (!ValidarDigitos(limpo))
            throw new DomainException("CPF inválido.");

        Numero = limpo;
    }

    private static bool ValidarDigitos(string cpf)
    {
        var multiplicador1 = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCpf = cpf[..9];
        var soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;
        var digito = resto < 2 ? 0 : 11 - resto;
        tempCpf += digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        digito = resto < 2 ? 0 : 11 - resto;
        tempCpf += digito;

        return cpf.EndsWith(tempCpf[9..]);
    }

    public override string ToString() => Numero;
}
