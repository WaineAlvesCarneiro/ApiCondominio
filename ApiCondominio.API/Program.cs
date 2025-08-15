using ApiCondominio.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppServices(builder.Configuration)
    .AddSwaggerDocumentation();

var app = builder.Build();

app.UseAppMiddleware(app.Environment);

app.MapControllers();
app.Run();
