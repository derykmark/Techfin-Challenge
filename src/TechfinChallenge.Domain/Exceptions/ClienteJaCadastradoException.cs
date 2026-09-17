namespace TechfinChallenge.Domain.Exceptions;

public class ClienteJaCadastradoException : DomainException
{
    public ClienteJaCadastradoException(string cpf)
        : base($"Já existe um cliente cadastrado com o CPF {cpf}.") { }
}
