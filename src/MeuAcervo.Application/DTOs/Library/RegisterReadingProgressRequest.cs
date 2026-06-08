namespace MeuAcervo.Application.DTOs.Library;

public sealed record RegisterReadingProgressRequest(
    int? PageNumber,
    decimal? ProgressPercent,
    DateTime? RecordedAtUtc,
    string? Notes);
