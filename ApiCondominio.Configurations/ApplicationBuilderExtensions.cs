using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace ApiCondominio.Configurations;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAppMiddleware(this IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseCors("AllowLocalhost");

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Condomínio v1.0.0");
                c.DocumentTitle = "Documentação da API - Condomínio";
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
