namespace TechfinChallenge.Domain.Events;

public record TransacaoAutorizadaEvent
{
    public Guid TransacaoId { get; init; }
    public Guid ClienteId { get; init; }
    public decimal Valor { get; init; }
    public DateTime DataEvento { get; init; } = DateTime.UtcNow;
}
