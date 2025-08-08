using ApiCondominio.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços
builder.Services
    .AddAppServices(builder.Configuration)   // DI, DbContext, Serviços, CORS, JWT etc.
    .AddSwaggerDocumentation();              // Swagger

var app = builder.Build();

// Configuração de middlewares
app.UseAppMiddleware(app.Environment);

app.MapControllers();
app.Run();
