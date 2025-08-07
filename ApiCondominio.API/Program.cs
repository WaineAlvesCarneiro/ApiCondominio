using ApiCondominio.Application.Interfaces;
using ApiCondominio.Application.Services;
using ApiCondominio.Application.Validators;
using ApiCondominio.Domain.Interfaces;
using ApiCondominio.Infrastructure.Repositories;
using ApiCondominio.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext")
        ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Condomínio  API",
            Version = "v1",
            Description = "Documentação da API de Condomínio desenvolvida em ASP.NET Core 8",
            Contact = new OpenApiContact
            {
                Name = "Waine Alves Carneiro",
                Email = "carneirowaine@gmail.com",
                Url = new Uri("https://github.com/WaineAlvesCarneiro?tab=repositories")
            }
        });

        string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            opt.IncludeXmlComments(xmlPath);
        }

        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer {token}'",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] {}
            }
        });
    });

builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

builder.Services.AddScoped<IImovelRepository, ImovelRepository>();
builder.Services.AddScoped<IImovelService, ImovelService>();

builder.Services.AddScoped<IMoradorRepository, MoradorRepository>();
builder.Services.AddScoped<IMoradorService, MoradorService>();

byte[] key = Encoding.ASCII.GetBytes("super_secret_jwt_key_here_123456789");
builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

builder.Services.AddAuthorization();
builder.Services.AddValidatorsFromAssemblyContaining<ImovelValidator>();
builder.Services.AddAutoMapper(typeof(Program));

WebApplication app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Condomínio v1.0.0");
        c.DocumentTitle = "Documentação da API - Condomínio";
    });
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();