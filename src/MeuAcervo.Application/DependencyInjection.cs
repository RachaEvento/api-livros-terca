using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MeuAcervo.Application.Abstractions.Books;
using MeuAcervo.Application.Services.Auth;
using MeuAcervo.Application.Services.Books;
using MeuAcervo.Application.Services.CustomFields;
using MeuAcervo.Application.Services.Library;
using MeuAcervo.Application.Services.Loans;
using MeuAcervo.Application.Services.Reviews;
using MeuAcervo.Application.Services.System;
using MeuAcervo.Application.Services.Tags;

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
        services.AddScoped<ICustomFieldService, CustomFieldService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IUserLibraryService, UserLibraryService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<ISystemInfoService, SystemInfoService>();

        return services;
    }
}
