using MeuAcervo.API.Configurations;
using MeuAcervo.API.Middlewares;
using MeuAcervo.Application;
using MeuAcervo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiFoundation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

await app.ApplyDatabaseMigrationsAsync();

if (app.Configuration.GetValue("Swagger:Enabled", app.Environment.IsDevelopment()))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Meu Acervo API v1");
        options.DocumentTitle = "Meu Acervo API";
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
