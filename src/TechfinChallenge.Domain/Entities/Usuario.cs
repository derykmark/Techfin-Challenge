namespace TechfinChallenge.Domain.Entities;

using TechfinChallenge.Domain.Exceptions;

public class Usuario : Entity
{
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public DateTime DataCriacao { get; private set; }

    private Usuario() { }

    public static Usuario Criar(string email, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("E-mail é obrigatório.");
        if (!email.Contains('@'))
            throw new DomainException("E-mail inválido.");

        return new Usuario
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant().Trim(),
            SenhaHash = senhaHash,
            DataCriacao = DateTime.UtcNow
        };
    }
}
