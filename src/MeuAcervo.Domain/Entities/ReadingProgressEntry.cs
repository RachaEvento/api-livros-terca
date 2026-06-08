using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class ReadingProgressEntry : TenantScopedAuditableEntityBase
{
    public Guid UserLibraryItemId { get; set; }

    public int? PageNumber { get; set; }

    public decimal? ProgressPercent { get; set; }

    public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;

    public string? Notes { get; set; }

    public UserLibraryItem? UserLibraryItem { get; set; }
}
