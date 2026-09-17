namespace TechfinChallenge.Domain.Exceptions;

public class LimiteInsuficienteException : DomainException
{
    public LimiteInsuficienteException(decimal limiteAtual, decimal valorSolicitado)
        : base($"Limite insuficiente. Limite atual: {limiteAtual:C}, valor solicitado: {valorSolicitado:C}.") { }
}
