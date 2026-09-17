namespace TechfinChallenge.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechfinChallenge.Application.DTOs.Requests;
using TechfinChallenge.Application.DTOs.Responses;
using TechfinChallenge.Application.UseCases.Clientes;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly CadastrarClienteUseCase _cadastrarClienteUseCase;
    private readonly ListarClientesUseCase _listarClientesUseCase;

    public ClientesController(
        CadastrarClienteUseCase cadastrarClienteUseCase,
        ListarClientesUseCase listarClientesUseCase)
    {
        _cadastrarClienteUseCase = cadastrarClienteUseCase;
        _listarClientesUseCase = listarClientesUseCase;
    }

    /// <summary>
    /// Cadastra um novo cliente.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CadastroClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarClienteRequest request)
    {
        var response = await _cadastrarClienteUseCase.ExecutarAsync(request);
        return Created(string.Empty, response);
    }

    /// <summary>
    /// Retorna a lista de todos os clientes cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var clientes = await _listarClientesUseCase.ExecutarAsync();
        return Ok(clientes.Select(c => new
        {
            idCliente = c.Id.ToString(),
            nome = c.Nome,
            cpf = c.Cpf.Numero,
            valorLimite = c.ValorLimite
        }));
    }
}
