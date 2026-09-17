namespace TechfinChallenge.Infrastructure.Persistence.Repositories;

using Dapper;
using TechfinChallenge.Domain.Entities;
using TechfinChallenge.Domain.Interfaces;

public class TransacaoRepository : ITransacaoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TransacaoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CriarAsync(Transacao transacao)
    {
        var connection = _connectionFactory.GetConnection();
        var sql = "INSERT INTO Transacoes (Id, ClienteId, Valor, Status, DataCriacao) VALUES (@Id, @ClienteId, @Valor, @Status, @DataCriacao)";

        await connection.ExecuteAsync(sql, new
        {
            Id = transacao.Id.ToString(),
            ClienteId = transacao.ClienteId.ToString(),
            Valor = transacao.Valor,
            Status = (int)transacao.Status,
            DataCriacao = transacao.DataCriacao.ToString("O")
        });
    }
}
