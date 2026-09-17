namespace TechfinChallenge.UnitTests.Domain;

using FluentAssertions;
using TechfinChallenge.Domain.Exceptions;
using TechfinChallenge.Domain.ValueObjects;

public class CpfTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    public void Criar_ComCpfValido_DeveCriar(string cpf)
    {
        // Act
        var resultado = new Cpf(cpf);

        // Assert
        resultado.Numero.Should().Be("52998224725");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComCpfVazio_DeveLancarExcecao(string? cpf)
    {
        // Act
        var act = () => new Cpf(cpf!);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Criar_ComCpfRepetido_DeveLancarExcecao()
    {
        // Act
        var act = () => new Cpf("11111111111");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*inválido*");
    }

    [Fact]
    public void Criar_ComCpfInvalido_DeveLancarExcecao()
    {
        // Act
        var act = () => new Cpf("12345678901");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*inválido*");
    }
}
