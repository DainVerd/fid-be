using Domain.Entities;

namespace Application.Interfaces.DataImport;

public interface IXmlDocumentMetadataLoader
{
    Task<IReadOnlyCollection<DocumentMetadata>> LoadAsync(
       string filePath,
       CancellationToken cancellationToken = default);
}
