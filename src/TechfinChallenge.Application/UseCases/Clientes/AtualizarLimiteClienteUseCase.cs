namespace TechfinChallenge.Application.UseCases.Clientes;

using Microsoft.Extensions.Caching.Memory;
using TechfinChallenge.Domain.Exceptions;
using TechfinChallenge.Domain.Interfaces;

public class AtualizarLimiteClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IMemoryCache _cache;

    public AtualizarLimiteClienteUseCase(
        IClienteRepository clienteRepository,
        IMemoryCache cache)
    {
        _clienteRepository = clienteRepository;
        _cache = cache;
    }

    public async Task ExecutarAsync(Guid clienteId, decimal valorDebito)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId)
            ?? throw new ClienteNaoEncontradoException(clienteId.ToString());

        cliente.DebitarLimite(valorDebito);
        await _clienteRepository.AtualizarLimiteAsync(clienteId, cliente.ValorLimite);

        // Invalidar cache
        _cache.Remove("clientes_todos");
    }
}
