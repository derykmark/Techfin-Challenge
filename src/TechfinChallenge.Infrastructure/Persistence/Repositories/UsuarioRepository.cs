namespace TechfinChallenge.Infrastructure.Persistence.Repositories;

using Dapper;
using TechfinChallenge.Domain.Entities;
using TechfinChallenge.Domain.Interfaces;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        var connection = _connectionFactory.GetConnection();
        var sql = "SELECT Id, Email, SenhaHash, DataCriacao FROM Usuarios WHERE LOWER(Email) = LOWER(@Email)";

        var result = await connection.QueryFirstOrDefaultAsync<UsuarioDto>(sql, new { Email = email });
        if (result is null) return null;

        return ReflectionHelper.CreateUsuario(Guid.Parse(result.Id), result.Email, result.SenhaHash, DateTime.Parse(result.DataCriacao));
    }

    public async Task CriarAsync(Usuario usuario)
    {
        var connection = _connectionFactory.GetConnection();
        var sql = "INSERT INTO Usuarios (Id, Email, SenhaHash, DataCriacao) VALUES (@Id, @Email, @SenhaHash, @DataCriacao)";

        await connection.ExecuteAsync(sql, new
        {
            Id = usuario.Id.ToString(),
            Email = usuario.Email,
            SenhaHash = usuario.SenhaHash,
            DataCriacao = usuario.DataCriacao.ToString("O")
        });
    }

    private record UsuarioDto(string Id, string Email, string SenhaHash, string DataCriacao);
}
