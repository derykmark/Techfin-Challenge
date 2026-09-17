namespace TechfinChallenge.Application.Interfaces;

public interface IEventPublisher
{
    Task PublicarTransacaoAutorizadaAsync(Guid transacaoId, Guid clienteId, decimal valor);
}
