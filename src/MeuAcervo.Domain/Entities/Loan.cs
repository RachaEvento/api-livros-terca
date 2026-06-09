using MeuAcervo.Domain.Common;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Domain.Entities;

public sealed class Loan : TenantScopedAuditableEntityBase
{
    public Guid UserLibraryItemId { get; set; }

    public string BorrowerName { get; set; } = string.Empty;

    public string? BorrowerContact { get; set; }

    public DateTime LoanedAtUtc { get; set; }

    public DateTime? DueAtUtc { get; set; }

    public DateTime? ReturnedAtUtc { get; set; }

    public LoanStatus Status { get; set; } = LoanStatus.Active;

    public string? Notes { get; set; }

    public UserLibraryItem? UserLibraryItem { get; set; }
}
