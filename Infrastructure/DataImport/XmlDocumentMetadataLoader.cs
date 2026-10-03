using Domain.Constants;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace Infrastructure.DataImport;

public class XmlDocumentMetadataLoader
{
    private readonly ILogger<XmlDocumentMetadataLoader> _logger;

    public XmlDocumentMetadataLoader(
        ILogger<XmlDocumentMetadataLoader> logger)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<DocumentMetadata>> LoadAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(filePath);

        var xml = await XDocument.LoadAsync(
            stream,
            LoadOptions.None,
            cancellationToken);

        var result = new List<DocumentMetadata>();

        foreach (var element in xml.Root?.Elements("document")
                     ?? Enumerable.Empty<XElement>())
        {
            // Assumed what some records could be invalid
            var document = TryMapDocument(element);

            if (document is null)
                continue;

            result.Add(document);
        }

        return result;
    }

    private DocumentMetadata? TryMapDocument(XElement element)
    {
        try
        {
            var title = element.Element("title")?.Value;
            var responsibleUnit =
                element.Element("responsibleUnit")?.Value;
            var url = element.Element("url")?.Value;
            var fileType = element.Element("fileType")?.Value;

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(responsibleUnit) ||
                string.IsNullOrWhiteSpace(url) ||
                string.IsNullOrWhiteSpace(fileType))
            {
                LogInvalid(element, "Required field is missing.");
                return null;
            }

            if (!DateTimeOffset.TryParse(
                    element.Element("createdAt")?.Value,
                    out var createdAt))
            {
                LogInvalid(element, "Invalid createdAt.");
                return null;
            }

            if (!int.TryParse(
                    element.Element("estimatedReadingMinutes")?.Value,
                    out var readingMinutes) ||
                readingMinutes <= 0)
            {
                LogInvalid(
                    element,
                    "Invalid estimatedReadingMinutes.");

                return null;
            }

            if (!Enum.TryParse<ImportanceLevel>(
                    element.Element("importance")?.Value,
                    true,
                    out var importance))
            {
                LogInvalid(element, "Invalid importance.");
                return null;
            }

            if (!Enum.TryParse<DocumentCategory>(
                    element.Element("category")?.Value,
                    true,
                    out var category))
            {
                LogInvalid(element, "Invalid category.");
                return null;
            }

            if (!bool.TryParse(
                    element.Element("isActive")?.Value,
                    out var isActive))
            {
                LogInvalid(element, "Invalid isActive.");
                return null;
            }

            // in real project should use Automapper for mapping instead
            // of manual mapping
            return new DocumentMetadata
            {
                Title = title,
                Description = element.Element("description")?.Value,
                ResponsibleUnit = responsibleUnit,
                CreatedAt = createdAt,
                Url = url,
                FileType = fileType,
                EstimatedReadingMinutes = readingMinutes,
                Importance = importance,
                Category = category,
                IsActive = isActive
            };
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to parse XML document record.");

            return null;
        }
    }

    private void LogInvalid(
        XElement element,
        string reason)
    {
        _logger.LogWarning(
            "Skipping invalid document '{Title}'. Reason: {Reason}",
            element.Element("title")?.Value ?? "Unknown",
            reason);
    }
}
