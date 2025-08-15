using ApiCondominio.Application.Interfaces;
using ApiCondominio.Application.Services;
using ApiCondominio.Application.Validators;
using ApiCondominio.Domain.Interfaces;
using ApiCondominio.Infrastructure;
using ApiCondominio.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiCondominio.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Banco de Dados
        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("ApplicationDbContext")
                ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")));

        // Controllers & Validators
        services.AddControllers();
        services.AddValidatorsFromAssemblyContaining<ImovelValidator>();

        // Endpoints
        services.AddEndpointsApiExplorer();

        // Repositórios
        services.AddScoped<IImovelRepository, ImovelRepository>();
        services.AddScoped<IMoradorRepository, MoradorRepository>();

        // Serviços
        services.AddScoped<IImovelService, ImovelService>();
        services.AddScoped<IMoradorService, MoradorService>();

        // AutoMapper
        services.AddAutoMapper(typeof(ImovelService).Assembly);
        services.AddAutoMapper(typeof(MoradorService).Assembly);

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost", policy =>
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        // JWT
        byte[] key = Encoding.ASCII.GetBytes("super_secret_jwt_key_here_123456789");
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = "Bearer";
            opt.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(opt =>
        {
            opt.RequireHttpsMetadata = false;
            opt.SaveToken = true;
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();

        return services;
    }
}
