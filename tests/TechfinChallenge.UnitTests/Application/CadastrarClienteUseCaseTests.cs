namespace TechfinChallenge.UnitTests.Application;

using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using TechfinChallenge.Application.DTOs.Requests;
using TechfinChallenge.Application.UseCases.Clientes;
using TechfinChallenge.Domain.Entities;
using TechfinChallenge.Domain.Exceptions;
using TechfinChallenge.Domain.Interfaces;

public class CadastrarClienteUseCaseTests
{
    private readonly Mock<IClienteRepository> _clienteRepoMock;
    private readonly IMemoryCache _cache;
    private readonly CadastrarClienteUseCase _useCase;

    public CadastrarClienteUseCaseTests()
    {
        _clienteRepoMock = new Mock<IClienteRepository>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _useCase = new CadastrarClienteUseCase(_clienteRepoMock.Object, _cache);
    }

    [Fact]
    public async Task Executar_ComDadosValidos_DeveCadastrarCliente()
    {
        // Arrange
        var request = new CadastrarClienteRequest("João Silva", "52998224725", 5000m);
        _clienteRepoMock.Setup(r => r.ObterPorCpfAsync(It.IsAny<string>())).ReturnsAsync((Cliente?)null);

        // Act
        var result = await _useCase.ExecutarAsync(request);

        // Assert
        result.Status.Should().Be("OK");
        result.IdCliente.Should().NotBeNullOrEmpty();
        _clienteRepoMock.Verify(r => r.CriarAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public async Task Executar_ComClienteJaCadastrado_DeveLancarExcecao()
    {
        // Arrange
        var clienteExistente = Cliente.Criar("João", "52998224725", 5000m);
        var request = new CadastrarClienteRequest("João Silva", "52998224725", 5000m);
        _clienteRepoMock.Setup(r => r.ObterPorCpfAsync("52998224725")).ReturnsAsync(clienteExistente);

        // Act
        var act = () => _useCase.ExecutarAsync(request);

        // Assert
        await act.Should().ThrowAsync<ClienteJaCadastradoException>();
    }
}
