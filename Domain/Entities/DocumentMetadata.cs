using Domain.Constants;
using Domain.Interfaces;

namespace Domain.Entities;

public class DocumentMetadata : IEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string ResponsibleUnit { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Url { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public int EstimatedReadingMinutes { get; set; }

    public ImportanceLevel Importance { get; set; }

    public DocumentCategory Category { get; set; }

    public bool IsActive { get; set; }
}
