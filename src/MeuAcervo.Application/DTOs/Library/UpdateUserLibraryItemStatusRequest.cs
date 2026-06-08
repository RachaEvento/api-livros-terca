using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Library;

public sealed record UpdateUserLibraryItemStatusRequest(
    ReadingStatus ReadingStatus,
    DateTime? StartedAt,
    DateTime? FinishedAt);
