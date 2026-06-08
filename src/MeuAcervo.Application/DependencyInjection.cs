using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MeuAcervo.Application.Abstractions.Books;
using MeuAcervo.Application.Services.Auth;
using MeuAcervo.Application.Services.Books;
using MeuAcervo.Application.Services.Library;
using MeuAcervo.Application.Services.System;

namespace MeuAcervo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<AssemblyMarker>();
        services.AddAutoMapper(config => { }, typeof(AssemblyMarker).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBookSearchOrchestrator, BookSearchOrchestrator>();
        services.AddScoped<IBookCatalogService, BookCatalogService>();
        services.AddScoped<IUserLibraryService, UserLibraryService>();
        services.AddScoped<ISystemInfoService, SystemInfoService>();

        return services;
    }
}
