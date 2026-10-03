using Domain.Constants;

namespace Application.Filters;

public class DocumentMetadataFilter
{
    public string? Search { get; set; }

    public string? ResponsibleUnit { get; set; }

    public string? FileType { get; set; }

    public ImportanceLevel? Importance { get; set; }

    public DocumentCategory? Category { get; set; }

    public bool? IsActive { get; set; }
}
