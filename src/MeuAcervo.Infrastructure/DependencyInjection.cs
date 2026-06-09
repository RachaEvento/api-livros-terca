using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MeuAcervo.Application.Abstractions.Auth;
using MeuAcervo.Application.Abstractions.Books;
using MeuAcervo.Application.Abstractions.CustomFields;
using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Application.Abstractions.Library;
using MeuAcervo.Application.Abstractions.Loans;
using MeuAcervo.Application.Abstractions.Profiles;
using MeuAcervo.Application.Abstractions.Reviews;
using MeuAcervo.Application.Models.Books;
using MeuAcervo.Infrastructure.Data;
using MeuAcervo.Infrastructure.Providers.OpenLibrary;
using MeuAcervo.Infrastructure.Providers.OpenLibrary.Contracts;
using MeuAcervo.Infrastructure.Repositories;
using MeuAcervo.Infrastructure.Services;
using MeuAcervo.Shared.Configuration;

namespace MeuAcervo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = DatabaseConnectionStringResolver.Resolve(configuration);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });

        var openLibraryOptions = configuration.GetSection(OpenLibraryOptions.SectionName).Get<OpenLibraryOptions>()
                                 ?? throw new InvalidOperationException("Open Library configuration is required.");

        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<IBookCatalogRepository, BookCatalogRepository>();
        services.AddScoped<ICustomFieldRepository, CustomFieldRepository>();
        services.AddScoped<IUserLibraryRepository, UserLibraryRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IAuthTokenService, AuthTokenService>();
        services.AddScoped<IApplicationDbContext>(serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IDatabaseHealthChecker, DatabaseHealthChecker>();

        if (openLibraryOptions.Enabled)
        {
            services.AddScoped<IBookResultNormalizer<OpenLibrarySearchResponse, IReadOnlyCollection<ExternalBookSearchResult>>, OpenLibrarySearchResultNormalizer>();
            services.AddHttpClient<OpenLibraryBookSearchProvider>(client =>
            {
                client.BaseAddress = new Uri(openLibraryOptions.BaseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromSeconds(openLibraryOptions.TimeoutSeconds);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd(openLibraryOptions.UserAgent);
            });
            services.AddScoped<IBookSearchProvider>(serviceProvider => serviceProvider.GetRequiredService<OpenLibraryBookSearchProvider>());
        }

        return services;
    }
}
