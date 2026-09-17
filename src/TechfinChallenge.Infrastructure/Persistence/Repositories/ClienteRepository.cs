namespace TechfinChallenge.Infrastructure.Persistence.Repositories;

using Dapper;
using TechfinChallenge.Domain.Entities;
using TechfinChallenge.Domain.Interfaces;

public class ClienteRepository : IClienteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ClienteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Cliente?> ObterPorIdAsync(Guid id)
    {
        var connection = _connectionFactory.GetConnection();
        var sql = "SELECT Id, Nome, Cpf, ValorLimite FROM Clientes WHERE Id = @Id";

        var result = await connection.QueryFirstOrDefaultAsync<ClienteDto>(sql, new { Id = id.ToString() });
        if (result is null) return null;

        return Cliente.Carregar(Guid.Parse(result.Id), result.Nome, result.Cpf, result.ValorLimite);
    }

    public async Task<Cliente?> ObterPorCpfAsync(string cpf)
    {
        var connection = _connectionFactory.GetConnection();
        var sql = "SELECT Id, Nome, Cpf, ValorLimite FROM Clientes WHERE Cpf = @Cpf";

        var result = await connection.QueryFirstOrDefaultAsync<ClienteDto>(sql, new { Cpf = cpf });
        if (result is null) return null;

        return Cliente.Carregar(Guid.Parse(result.Id), result.Nome, result.Cpf, result.ValorLimite);
    }

    public async Task<IEnumerable<Cliente>> ObterTodosAsync()
    {
        var connection = _connectionFactory.GetConnection();
        var sql = "SELECT Id, Nome, Cpf, ValorLimite FROM Clientes";

        var results = await connection.QueryAsync<ClienteDto>(sql);
        return results.Select(r => Cliente.Carregar(Guid.Parse(r.Id), r.Nome, r.Cpf, r.ValorLimite));
    }

    public async Task CriarAsync(Cliente cliente)
    {
        var connection = _connectionFactory.GetConnection();
        var sql = "INSERT INTO Clientes (Id, Nome, Cpf, ValorLimite) VALUES (@Id, @Nome, @Cpf, @ValorLimite)";

        await connection.ExecuteAsync(sql, new
        {
            Id = cliente.Id.ToString(),
            Nome = cliente.Nome,
            Cpf = cliente.Cpf.Numero,
            ValorLimite = cliente.ValorLimite
        });
    }

    public async Task AtualizarLimiteAsync(Guid id, decimal novoLimite)
    {
        var connection = _connectionFactory.GetConnection();
        var sql = "UPDATE Clientes SET ValorLimite = @NovoLimite WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new { Id = id.ToString(), NovoLimite = novoLimite });
    }

    private record ClienteDto(string Id, string Nome, string Cpf, decimal ValorLimite);
}
