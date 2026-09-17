namespace TechfinChallenge.Domain.Entities;

using TechfinChallenge.Domain.ValueObjects;
using TechfinChallenge.Domain.Exceptions;

public class Cliente : Entity
{
    public string Nome { get; private set; } = string.Empty;
    public Cpf Cpf { get; private set; } = null!;
    public decimal ValorLimite { get; private set; }

    private Cliente() { }

    public static Cliente Criar(string nome, string cpf, decimal valorLimite)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome é obrigatório.");

        if (valorLimite < 0)
            throw new DomainException("Valor de limite não pode ser negativo.");

        return new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = nome.Trim(),
            Cpf = new Cpf(cpf),
            ValorLimite = valorLimite
        };
    }

    public void DebitarLimite(decimal valor)
    {
        if (valor <= 0)
            throw new DomainException("Valor da transação deve ser positivo.");

        if (valor > ValorLimite)
            throw new LimiteInsuficienteException(ValorLimite, valor);

        ValorLimite -= valor;
    }

    // For Dapper hydration
    public static Cliente Carregar(Guid id, string nome, string cpf, decimal valorLimite)
    {
        return new Cliente
        {
            Id = id,
            Nome = nome,
            Cpf = new Cpf(cpf),
            ValorLimite = valorLimite
        };
    }
}
