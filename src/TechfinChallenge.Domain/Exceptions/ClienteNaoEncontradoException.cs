namespace TechfinChallenge.Domain.Exceptions;

public class ClienteNaoEncontradoException : DomainException
{
    public ClienteNaoEncontradoException(string clienteId)
        : base($"Cliente com ID '{clienteId}' não encontrado.") { }
}
