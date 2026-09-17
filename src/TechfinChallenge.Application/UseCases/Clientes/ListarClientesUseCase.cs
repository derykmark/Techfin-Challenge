namespace TechfinChallenge.Application.UseCases.Clientes;

using Microsoft.Extensions.Caching.Memory;
using TechfinChallenge.Domain.Entities;
using TechfinChallenge.Domain.Interfaces;

public class ListarClientesUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "clientes_todos";

    public ListarClientesUseCase(
        IClienteRepository clienteRepository,
        IMemoryCache cache)
    {
        _clienteRepository = clienteRepository;
        _cache = cache;
    }

    public async Task<IEnumerable<Cliente>> ExecutarAsync()
    {
        if (_cache.TryGetValue(CacheKey, out IEnumerable<Cliente>? clientesCache) && clientesCache is not null)
            return clientesCache;

        var clientes = await _clienteRepository.ObterTodosAsync();

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

        _cache.Set(CacheKey, clientes, cacheOptions);

        return clientes;
    }
}
