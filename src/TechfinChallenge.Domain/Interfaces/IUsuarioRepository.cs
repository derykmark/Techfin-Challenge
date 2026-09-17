namespace TechfinChallenge.Domain.Interfaces;

using TechfinChallenge.Domain.Entities;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task CriarAsync(Usuario usuario);
}
