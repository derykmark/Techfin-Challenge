namespace TechfinChallenge.Application.UseCases.Clientes;

using Microsoft.Extensions.Caching.Memory;
using TechfinChallenge.Application.DTOs.Requests;
using TechfinChallenge.Application.DTOs.Responses;
using TechfinChallenge.Domain.Entities;
using TechfinChallenge.Domain.Exceptions;
using TechfinChallenge.Domain.Interfaces;

public class CadastrarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IMemoryCache _cache;

    public CadastrarClienteUseCase(
        IClienteRepository clienteRepository,
        IMemoryCache cache)
    {
        _clienteRepository = clienteRepository;
        _cache = cache;
    }

    public async Task<CadastroClienteResponse> ExecutarAsync(CadastrarClienteRequest request)
    {
        var clienteExistente = await _clienteRepository.ObterPorCpfAsync(
            request.Cpf.Replace(".", "").Replace("-", ""));

        if (clienteExistente is not null)
            throw new ClienteJaCadastradoException(request.Cpf);

        var cliente = Cliente.Criar(request.Nome, request.Cpf, request.ValorLimite);
        await _clienteRepository.CriarAsync(cliente);

        // Invalidar cache da listagem
        _cache.Remove("clientes_todos");

        return new CadastroClienteResponse(cliente.Id.ToString(), "OK");
    }
}
