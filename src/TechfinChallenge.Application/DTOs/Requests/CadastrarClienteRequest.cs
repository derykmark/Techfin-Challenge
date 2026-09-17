namespace TechfinChallenge.Application.DTOs.Requests;

public record CadastrarClienteRequest(string Nome, string Cpf, decimal ValorLimite);
