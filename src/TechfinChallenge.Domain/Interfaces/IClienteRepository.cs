namespace TechfinChallenge.Domain.Interfaces;

using TechfinChallenge.Domain.Entities;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(Guid id);
    Task<Cliente?> ObterPorCpfAsync(string cpf);
    Task<IEnumerable<Cliente>> ObterTodosAsync();
    Task CriarAsync(Cliente cliente);
    Task AtualizarLimiteAsync(Guid id, decimal novoLimite);
}
