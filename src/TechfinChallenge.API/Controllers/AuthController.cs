namespace TechfinChallenge.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using TechfinChallenge.Application.DTOs.Requests;
using TechfinChallenge.Application.DTOs.Responses;
using TechfinChallenge.Application.UseCases.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly CadastrarUsuarioUseCase _cadastrarUsuarioUseCase;
    private readonly AutenticarUsuarioUseCase _autenticarUsuarioUseCase;

    public AuthController(
        CadastrarUsuarioUseCase cadastrarUsuarioUseCase,
        AutenticarUsuarioUseCase autenticarUsuarioUseCase)
    {
        _cadastrarUsuarioUseCase = cadastrarUsuarioUseCase;
        _autenticarUsuarioUseCase = autenticarUsuarioUseCase;
    }

    /// <summary>
    /// Cadastra um novo usuário.
    /// </summary>
    [HttpPost("registrar")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] CadastrarUsuarioRequest request)
    {
        await _cadastrarUsuarioUseCase.ExecutarAsync(request);
        return Created();
    }

    /// <summary>
    /// Autentica um usuário e retorna um token JWT.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AutenticarUsuarioRequest request)
    {
        var response = await _autenticarUsuarioUseCase.ExecutarAsync(request);
        return Ok(response);
    }
}
