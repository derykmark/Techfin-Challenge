namespace TechfinChallenge.Application.UseCases.Transacoes;

using TechfinChallenge.Application.DTOs.Requests;
using TechfinChallenge.Application.DTOs.Responses;
using TechfinChallenge.Application.Interfaces;
using TechfinChallenge.Domain.Entities;
using TechfinChallenge.Domain.Exceptions;
using TechfinChallenge.Domain.Interfaces;

public class AutorizarTransacaoUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IEventPublisher _eventPublisher;

    public AutorizarTransacaoUseCase(
        IClienteRepository clienteRepository,
        ITransacaoRepository transacaoRepository,
        IEventPublisher eventPublisher)
    {
        _clienteRepository = clienteRepository;
        _transacaoRepository = transacaoRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<TransacaoResponse> ExecutarAsync(AutorizarTransacaoRequest request)
    {
        if (!Guid.TryParse(request.IdCliente, out var clienteId))
            throw new DomainException("ID do cliente inválido.");

        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId)
            ?? throw new ClienteNaoEncontradoException(request.IdCliente);

        if (request.ValorSimulacao > cliente.ValorLimite)
            return new TransacaoResponse("NEGADO");

        var transacao = Transacao.Criar(clienteId, request.ValorSimulacao);
        await _transacaoRepository.CriarAsync(transacao);

        // Publicar evento para atualizar limite via RabbitMQ
        await _eventPublisher.PublicarTransacaoAutorizadaAsync(
            transacao.Id, clienteId, request.ValorSimulacao);

        return new TransacaoResponse("APROVADO", transacao.Id.ToString());
    }
}
