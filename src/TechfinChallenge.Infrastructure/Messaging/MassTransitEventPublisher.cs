namespace TechfinChallenge.Infrastructure.Messaging;

using MassTransit;
using TechfinChallenge.Application.Interfaces;
using TechfinChallenge.Domain.Events;

public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublicarTransacaoAutorizadaAsync(Guid transacaoId, Guid clienteId, decimal valor)
    {
        var @event = new TransacaoAutorizadaEvent
        {
            TransacaoId = transacaoId,
            ClienteId = clienteId,
            Valor = valor
        };

        await _publishEndpoint.Publish(@event);
    }
}
