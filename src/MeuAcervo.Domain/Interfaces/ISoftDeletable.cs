namespace MeuAcervo.Domain.Interfaces;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }

    DateTime? DeletedAtUtc { get; set; }
}
