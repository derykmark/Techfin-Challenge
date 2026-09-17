namespace TechfinChallenge.Application.Interfaces;

public interface ITokenService
{
    string GerarToken(string userId, string email);
}
