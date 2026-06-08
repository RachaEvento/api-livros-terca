using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MeuAcervo.API.Auth;
using MeuAcervo.API.Common;
using MeuAcervo.Application.Abstractions.CurrentUser;
using MeuAcervo.Shared.Auth;
using MeuAcervo.Shared.Configuration;
using MeuAcervo.Shared.Results;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MeuAcervo.API.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiFoundation(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHttpContextAccessor()
            .AddScoped<ICurrentUserContext, CurrentUserContext>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddOptions<AppInfoOptions>()
            .Bind(configuration.GetSection(AppInfoOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<OpenLibraryOptions>()
            .Bind(configuration.GetSection(OpenLibraryOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var details = context.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors
                            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "The input is invalid." : error.ErrorMessage)
                            .ToArray());

                var response = ApiResponse<object?>.Fail(
                    new ApiError("validation_error", "One or more validation errors occurred.", details),
                    ApiMetadataFactory.Create(context.HttpContext));

                return new BadRequestObjectResult(response);
            };
        });

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                                 ?? throw new InvalidOperationException("Jwt configuration is required.");

                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    NameClaimType = JwtClaimNames.Username,
                    RoleClaimType = JwtClaimNames.Roles,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization(options =>
        {
            foreach (var policy in AuthorizationPolicies.PermissionMap)
            {
                options.AddPolicy(policy.Key, builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(new PermissionRequirement(policy.Value));
                });
            }
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Meu Acervo API",
                Version = "v1",
                Description = "Fundação da API REST multi-tenant do sistema Meu Acervo."
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Bearer JWT. Exemplo: Bearer {token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            options.AddSecurityDefinition("Bearer", securityScheme);
            options.DocumentFilter<AuthorizeDocumentFilter>();

            options.SupportNonNullableReferenceTypes();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        });

        return services;
    }
}

internal sealed class AuthorizeDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var apiDescription in context.ApiDescriptions)
        {
            if (apiDescription.ActionDescriptor is not Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor actionDescriptor)
            {
                continue;
            }

            var actionAttributes = actionDescriptor.MethodInfo.GetCustomAttributes(inherit: true);
            var controllerAttributes = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(inherit: true);

            var allowsAnonymous = actionAttributes.OfType<AllowAnonymousAttribute>().Any()
                || controllerAttributes.OfType<AllowAnonymousAttribute>().Any();

            if (allowsAnonymous)
            {
                continue;
            }

            var requiresAuthorization = actionAttributes.OfType<AuthorizeAttribute>().Any()
                || controllerAttributes.OfType<AuthorizeAttribute>().Any();

            if (!requiresAuthorization)
            {
                continue;
            }

            if (!TryGetOperation(swaggerDoc, apiDescription, out var operation))
            {
                continue;
            }

            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", swaggerDoc, null)] = []
            });
        }
    }

    private static bool TryGetOperation(
        OpenApiDocument swaggerDoc,
        Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription apiDescription,
        out OpenApiOperation operation)
    {
        operation = null!;

        var relativePath = apiDescription.RelativePath;
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return false;
        }

        var normalizedPath = "/" + relativePath.TrimStart('/').Split('?')[0];
        if (!swaggerDoc.Paths.TryGetValue(normalizedPath, out var pathItem) || pathItem?.Operations is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(apiDescription.HttpMethod))
        {
            return false;
        }

        var operationType = new HttpMethod(apiDescription.HttpMethod);
        if (!pathItem.Operations.TryGetValue(operationType, out var resolvedOperation) || resolvedOperation is null)
        {
            return false;
        }

        operation = resolvedOperation;
        return true;
    }
}
