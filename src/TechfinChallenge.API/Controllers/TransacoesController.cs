namespace TechfinChallenge.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechfinChallenge.Application.DTOs.Requests;
using TechfinChallenge.Application.DTOs.Responses;
using TechfinChallenge.Application.UseCases.Transacoes;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransacoesController : ControllerBase
{
    private readonly AutorizarTransacaoUseCase _autorizarTransacaoUseCase;

    public TransacoesController(AutorizarTransacaoUseCase autorizarTransacaoUseCase)
    {
        _autorizarTransacaoUseCase = autorizarTransacaoUseCase;
    }

    /// <summary>
    /// Simula a autorização de uma transação.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TransacaoResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Autorizar([FromBody] AutorizarTransacaoRequest request)
    {
        var response = await _autorizarTransacaoUseCase.ExecutarAsync(request);
        return Ok(response);
    }
}
