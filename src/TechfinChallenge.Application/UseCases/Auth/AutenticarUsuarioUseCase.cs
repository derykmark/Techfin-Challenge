namespace TechfinChallenge.Application.UseCases.Auth;

using TechfinChallenge.Application.DTOs.Requests;
using TechfinChallenge.Application.DTOs.Responses;
using TechfinChallenge.Application.Interfaces;
using TechfinChallenge.Domain.Exceptions;
using TechfinChallenge.Domain.Interfaces;

public class AutenticarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AutenticarUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<TokenResponse> ExecutarAsync(AutenticarUsuarioRequest request)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);
        if (usuario is null)
            throw new DomainException("E-mail ou senha inválidos.");

        if (!_passwordHasher.Verify(request.Senha, usuario.SenhaHash))
            throw new DomainException("E-mail ou senha inválidos.");

        var token = _tokenService.GerarToken(usuario.Id.ToString(), usuario.Email);
        return new TokenResponse(token, DateTime.UtcNow.AddHours(1));
    }
}
