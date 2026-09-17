namespace TechfinChallenge.Application.UseCases.Auth;

using TechfinChallenge.Application.DTOs.Requests;
using TechfinChallenge.Application.Interfaces;
using TechfinChallenge.Domain.Entities;
using TechfinChallenge.Domain.Exceptions;
using TechfinChallenge.Domain.Interfaces;

public class CadastrarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CadastrarUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task ExecutarAsync(CadastrarUsuarioRequest request)
    {
        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(request.Email);
        if (usuarioExistente is not null)
            throw new DomainException("Já existe um usuário cadastrado com este e-mail.");

        var senhaHash = _passwordHasher.Hash(request.Senha);
        var usuario = Usuario.Criar(request.Email, senhaHash);

        await _usuarioRepository.CriarAsync(usuario);
    }
}
