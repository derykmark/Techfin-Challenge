namespace TechfinChallenge.Application.DTOs.Responses;

public record ErroResponse(string Status, string DetalheErro)
{
    public static ErroResponse Criar(string detalhe) => new("ERRO", detalhe);
}
