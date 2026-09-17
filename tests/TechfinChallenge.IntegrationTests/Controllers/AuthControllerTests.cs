namespace TechfinChallenge.IntegrationTests.Controllers;

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TechfinChallenge.Application.DTOs.Requests;
using TechfinChallenge.Application.DTOs.Responses;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Registrar_ComDadosValidos_DeveRetornar201()
    {
        // Arrange
        var request = new CadastrarUsuarioRequest("test@email.com", "senha123");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/registrar", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornarToken()
    {
        // Arrange
        var email = $"login-test-{Guid.NewGuid():N}@email.com";
        var registerRequest = new CadastrarUsuarioRequest(email, "senha123");
        await _client.PostAsJsonAsync("/api/auth/registrar", registerRequest);

        var loginRequest = new AutenticarUsuarioRequest(email, "senha123");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        tokenResponse.Should().NotBeNull();
        tokenResponse!.Token.Should().NotBeNullOrEmpty();
    }
}
