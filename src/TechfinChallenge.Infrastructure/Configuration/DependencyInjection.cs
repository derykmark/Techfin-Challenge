namespace TechfinChallenge.Infrastructure.Configuration;

using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TechfinChallenge.Application.Interfaces;
using TechfinChallenge.Application.UseCases.Auth;
using TechfinChallenge.Application.UseCases.Clientes;
using TechfinChallenge.Application.UseCases.Transacoes;
using TechfinChallenge.Domain.Interfaces;
using TechfinChallenge.Infrastructure.Auth;
using TechfinChallenge.Infrastructure.Messaging;
using TechfinChallenge.Infrastructure.Messaging.Consumers;
using TechfinChallenge.Infrastructure.Persistence;
using TechfinChallenge.Infrastructure.Persistence.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        // Repositories
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ITransacaoRepository, TransacaoRepository>();

        // Auth
        var jwtSettings = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettings);
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Cache
        services.AddMemoryCache();

        // Messaging - MassTransit + RabbitMQ
        var rabbitMqSettings = configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>() ?? new RabbitMqSettings();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<TransacaoAutorizadaConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSettings.Host, "/", h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        // Use Cases
        services.AddScoped<CadastrarUsuarioUseCase>();
        services.AddScoped<AutenticarUsuarioUseCase>();
        services.AddScoped<CadastrarClienteUseCase>();
        services.AddScoped<ListarClientesUseCase>();
        services.AddScoped<AtualizarLimiteClienteUseCase>();
        services.AddScoped<AutorizarTransacaoUseCase>();

        return services;
    }
}
