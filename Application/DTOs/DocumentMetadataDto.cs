using Domain.Constants;

namespace Application.DTOs;

public class DocumentMetadataDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string ResponsibleUnit { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public string Url { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public int EstimatedReadingMinutes { get; set; }

    public ImportanceLevel Importance { get; set; }

    public DocumentCategory Category { get; set; }

    public bool IsActive { get; set; }
}
