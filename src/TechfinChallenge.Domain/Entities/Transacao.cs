namespace TechfinChallenge.Domain.Entities;

using TechfinChallenge.Domain.Enums;
using TechfinChallenge.Domain.Exceptions;

public class Transacao : Entity
{
    public Guid ClienteId { get; private set; }
    public decimal Valor { get; private set; }
    public StatusTransacao Status { get; private set; }
    public DateTime DataCriacao { get; private set; }

    private Transacao() { }

    public static Transacao Criar(Guid clienteId, decimal valor)
    {
        if (valor <= 0)
            throw new DomainException("Valor da simulação deve ser positivo.");

        return new Transacao
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            Valor = valor,
            Status = StatusTransacao.Aprovado,
            DataCriacao = DateTime.UtcNow
        };
    }
}
