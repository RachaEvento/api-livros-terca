namespace MeuAcervo.Application.DTOs.Library;

public sealed record ReadingProgressEntryResponse(
    Guid Id,
    int? PageNumber,
    decimal? ProgressPercent,
    DateTime RecordedAtUtc,
    string? Notes);
