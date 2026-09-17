namespace TechfinChallenge.UnitTests.Domain;

using FluentAssertions;
using TechfinChallenge.Domain.Entities;
using TechfinChallenge.Domain.Enums;
using TechfinChallenge.Domain.Exceptions;

public class TransacaoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarTransacao()
    {
        // Arrange
        var clienteId = Guid.NewGuid();

        // Act
        var transacao = Transacao.Criar(clienteId, 1000m);

        // Assert
        transacao.Should().NotBeNull();
        transacao.Id.Should().NotBeEmpty();
        transacao.ClienteId.Should().Be(clienteId);
        transacao.Valor.Should().Be(1000m);
        transacao.Status.Should().Be(StatusTransacao.Aprovado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Criar_ComValorInvalido_DeveLancarExcecao(decimal valor)
    {
        // Act
        var act = () => Transacao.Criar(Guid.NewGuid(), valor);

        // Assert
        act.Should().Throw<DomainException>();
    }
}
