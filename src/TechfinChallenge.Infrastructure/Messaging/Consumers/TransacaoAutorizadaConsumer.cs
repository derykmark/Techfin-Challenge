namespace TechfinChallenge.Infrastructure.Messaging.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using TechfinChallenge.Application.UseCases.Clientes;
using TechfinChallenge.Domain.Events;

public class TransacaoAutorizadaConsumer : IConsumer<TransacaoAutorizadaEvent>
{
    private readonly AtualizarLimiteClienteUseCase _atualizarLimiteUseCase;
    private readonly ILogger<TransacaoAutorizadaConsumer> _logger;

    public TransacaoAutorizadaConsumer(
        AtualizarLimiteClienteUseCase atualizarLimiteUseCase,
        ILogger<TransacaoAutorizadaConsumer> logger)
    {
        _atualizarLimiteUseCase = atualizarLimiteUseCase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TransacaoAutorizadaEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation(
            "Processando evento TransacaoAutorizada: TransacaoId={TransacaoId}, ClienteId={ClienteId}, Valor={Valor}",
            message.TransacaoId, message.ClienteId, message.Valor);

        try
        {
            await _atualizarLimiteUseCase.ExecutarAsync(message.ClienteId, message.Valor);
            _logger.LogInformation("Limite do cliente {ClienteId} atualizado com sucesso.", message.ClienteId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar limite do cliente {ClienteId}.", message.ClienteId);
            throw;
        }
    }
}
