namespace TechfinChallenge.UnitTests.Domain;

using FluentAssertions;
using TechfinChallenge.Domain.Entities;
using TechfinChallenge.Domain.Exceptions;

public class ClienteTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarCliente()
    {
        // Arrange & Act
        var cliente = Cliente.Criar("João Silva", "52998224725", 5000m);

        // Assert
        cliente.Should().NotBeNull();
        cliente.Id.Should().NotBeEmpty();
        cliente.Nome.Should().Be("João Silva");
        cliente.Cpf.Numero.Should().Be("52998224725");
        cliente.ValorLimite.Should().Be(5000m);
    }

    [Fact]
    public void Criar_ComLimiteNegativo_DeveLancarExcecao()
    {
        // Act
        var act = () => Cliente.Criar("João Silva", "52998224725", -100m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*negativo*");
    }

    [Fact]
    public void Criar_ComNomeVazio_DeveLancarExcecao()
    {
        // Act
        var act = () => Cliente.Criar("", "52998224725", 5000m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*obrigatório*");
    }

    [Fact]
    public void DebitarLimite_ComValorValido_DeveDebitarLimite()
    {
        // Arrange
        var cliente = Cliente.Criar("João Silva", "52998224725", 5000m);

        // Act
        cliente.DebitarLimite(1000m);

        // Assert
        cliente.ValorLimite.Should().Be(4000m);
    }

    [Fact]
    public void DebitarLimite_ComValorSuperiorAoLimite_DeveLancarExcecao()
    {
        // Arrange
        var cliente = Cliente.Criar("João Silva", "52998224725", 5000m);

        // Act
        var act = () => cliente.DebitarLimite(6000m);

        // Assert
        act.Should().Throw<LimiteInsuficienteException>();
    }

    [Fact]
    public void DebitarLimite_ComValorZeroOuNegativo_DeveLancarExcecao()
    {
        // Arrange
        var cliente = Cliente.Criar("João Silva", "52998224725", 5000m);

        // Act
        var act = () => cliente.DebitarLimite(0m);

        // Assert
        act.Should().Throw<DomainException>();
    }
}
