using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MeuAcervo.API.Common;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, error) = MapException(exception);

        _logger.LogError(
            exception,
            "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object?>.Fail(error, ApiMetadataFactory.Create(context));

        await context.Response.WriteAsJsonAsync(response);
    }

    private static (int StatusCode, ApiError Error) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                new ApiError(
                    "validation_error",
                    "One or more validation errors occurred.",
                    validationException.Errors
                        .GroupBy(error => error.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(error => error.ErrorMessage).Distinct().ToArray()))),

            UnauthorizedException unauthorizedException => (
                StatusCodes.Status401Unauthorized,
                new ApiError(unauthorizedException.ErrorCode, unauthorizedException.Message)),

            ForbiddenException forbiddenException => (
                StatusCodes.Status403Forbidden,
                new ApiError(forbiddenException.ErrorCode, forbiddenException.Message)),

            NotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                new ApiError(notFoundException.ErrorCode, notFoundException.Message)),

            ConflictException conflictException => (
                StatusCodes.Status409Conflict,
                new ApiError(conflictException.ErrorCode, conflictException.Message)),

            BusinessRuleException businessRuleException => (
                StatusCodes.Status422UnprocessableEntity,
                new ApiError(businessRuleException.ErrorCode, businessRuleException.Message)),

            ExternalProviderException externalProviderException => (
                StatusCodes.Status502BadGateway,
                new ApiError(externalProviderException.ErrorCode, externalProviderException.Message)),

            DbUpdateException dbUpdateException when IsUniqueConstraintViolation(dbUpdateException) => (
                StatusCodes.Status409Conflict,
                new ApiError("conflict", "A unique data constraint was violated.")),

            AppException appException => (
                StatusCodes.Status400BadRequest,
                new ApiError(appException.ErrorCode, appException.Message)),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ApiError("internal_server_error", "An unexpected error occurred."))
        };
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        var message = exception.InnerException?.Message ?? exception.Message;

        return message.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
               || message.Contains("unique", StringComparison.OrdinalIgnoreCase)
               || message.Contains("23505", StringComparison.OrdinalIgnoreCase);
    }
}
