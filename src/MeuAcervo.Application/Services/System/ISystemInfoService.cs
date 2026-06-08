using MeuAcervo.Application.DTOs.System;

namespace MeuAcervo.Application.Services.System;

public interface ISystemInfoService
{
    ApiInfoResponse GetApiInfo();

    Task<HealthCheckResponse> GetHealthAsync(CancellationToken cancellationToken = default);
}
