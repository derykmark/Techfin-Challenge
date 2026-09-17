namespace TechfinChallenge.UnitTests.Application;

using FluentAssertions;
using Moq;
using TechfinChallenge.Application.DTOs.Requests;
using TechfinChallenge.Application.Interfaces;
using TechfinChallenge.Application.UseCases.Transacoes;
using TechfinChallenge.Domain.Entities;
using TechfinChallenge.Domain.Interfaces;

public class AutorizarTransacaoUseCaseTests
{
    private readonly Mock<IClienteRepository> _clienteRepoMock;
    private readonly Mock<ITransacaoRepository> _transacaoRepoMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly AutorizarTransacaoUseCase _useCase;

    public AutorizarTransacaoUseCaseTests()
    {
        _clienteRepoMock = new Mock<IClienteRepository>();
        _transacaoRepoMock = new Mock<ITransacaoRepository>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _useCase = new AutorizarTransacaoUseCase(
            _clienteRepoMock.Object,
            _transacaoRepoMock.Object,
            _eventPublisherMock.Object);
    }

    [Fact]
    public async Task Executar_ComLimiteSuficiente_DeveAprovar()
    {
        // Arrange
        var cliente = Cliente.Criar("João", "52998224725", 5000m);
        var request = new AutorizarTransacaoRequest(cliente.Id.ToString(), 1000m);
        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);

        // Act
        var result = await _useCase.ExecutarAsync(request);

        // Assert
        result.Status.Should().Be("APROVADO");
        result.IdTransacao.Should().NotBeNullOrEmpty();
        _transacaoRepoMock.Verify(r => r.CriarAsync(It.IsAny<Transacao>()), Times.Once);
        _eventPublisherMock.Verify(e => e.PublicarTransacaoAutorizadaAsync(
            It.IsAny<Guid>(), cliente.Id, 1000m), Times.Once);
    }

    [Fact]
    public async Task Executar_ComLimiteInsuficiente_DeveNegar()
    {
        // Arrange
        var cliente = Cliente.Criar("João", "52998224725", 500m);
        var request = new AutorizarTransacaoRequest(cliente.Id.ToString(), 1000m);
        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);

        // Act
        var result = await _useCase.ExecutarAsync(request);

        // Assert
        result.Status.Should().Be("NEGADO");
        result.IdTransacao.Should().BeNull();
        _transacaoRepoMock.Verify(r => r.CriarAsync(It.IsAny<Transacao>()), Times.Never);
    }

    [Fact]
    public async Task Executar_ComClienteInexistente_DeveLancarExcecao()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new AutorizarTransacaoRequest(id.ToString(), 1000m);
        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Cliente?)null);

        // Act
        var act = () => _useCase.ExecutarAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
}
