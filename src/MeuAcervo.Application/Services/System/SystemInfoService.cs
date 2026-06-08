using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Application.DTOs.System;
using MeuAcervo.Shared.Configuration;

namespace MeuAcervo.Application.Services.System;

public sealed class SystemInfoService : ISystemInfoService
{
    private readonly IDatabaseHealthChecker _databaseHealthChecker;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly AppInfoOptions _appInfoOptions;
    private readonly JwtOptions _jwtOptions;

    public SystemInfoService(
        IDatabaseHealthChecker databaseHealthChecker,
        IHostEnvironment hostEnvironment,
        IOptions<AppInfoOptions> appInfoOptions,
        IOptions<JwtOptions> jwtOptions)
    {
        _databaseHealthChecker = databaseHealthChecker;
        _hostEnvironment = hostEnvironment;
        _appInfoOptions = appInfoOptions.Value;
        _jwtOptions = jwtOptions.Value;
    }

    public ApiInfoResponse GetApiInfo()
    {
        return new ApiInfoResponse(
            _appInfoOptions.Name,
            _appInfoOptions.Version,
            _appInfoOptions.Description,
            _hostEnvironment.EnvironmentName,
            "/api/v1",
            !string.IsNullOrWhiteSpace(_jwtOptions.Key),
            "JWT claim tid",
            DateTime.UtcNow);
    }

    public async Task<HealthCheckResponse> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        var databaseAvailable = await _databaseHealthChecker.CanConnectAsync(cancellationToken);

        return new HealthCheckResponse(
            databaseAvailable ? "Healthy" : "Degraded",
            databaseAvailable,
            DateTime.UtcNow);
    }
}
