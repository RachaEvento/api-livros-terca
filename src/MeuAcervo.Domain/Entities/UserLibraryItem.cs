using MeuAcervo.Domain.Common;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Domain.Entities;

public sealed class UserLibraryItem : TenantScopedSoftDeletableAuditableEntityBase
{
    public Guid UserId { get; set; }

    public Guid BookEditionId { get; set; }

    public ShelfType ShelfType { get; set; } = ShelfType.Collection;

    public ReadingStatus ReadingStatus { get; set; } = ReadingStatus.NotStarted;

    public AcquisitionFormat? AcquisitionFormat { get; set; }

    public OwnershipType? OwnershipType { get; set; }

    public bool IsFavorite { get; set; }

    public int? CurrentPage { get; set; }

    public decimal? ProgressPercent { get; set; }

    public int ReadCount { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public DateTime? AcquiredAt { get; set; }

    public string? PhysicalLocation { get; set; }

    public string? Condition { get; set; }

    public string? PrivateNotes { get; set; }

    public User? User { get; set; }

    public BookEdition? BookEdition { get; set; }

    public ICollection<ReadingProgressEntry> ReadingProgressEntries { get; set; } = new List<ReadingProgressEntry>();

    public ICollection<UserLibraryItemTag> UserLibraryItemTags { get; set; } = new List<UserLibraryItemTag>();

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public Review? Review { get; set; }
}
