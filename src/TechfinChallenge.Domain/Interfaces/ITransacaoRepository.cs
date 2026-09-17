namespace TechfinChallenge.Domain.Interfaces;

using TechfinChallenge.Domain.Entities;

public interface ITransacaoRepository
{
    Task CriarAsync(Transacao transacao);
}
