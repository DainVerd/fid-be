using Application.Interfaces.DataImport;
using Application.Interfaces.Repositories;
using Infrastructure.DataImport;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class DatabaseInitializer
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IXmlDocumentMetadataLoader _loader;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        IUnitOfWork unitOfWork,
        IXmlDocumentMetadataLoader loader,
        IHostEnvironment environment,
        ILogger<DatabaseInitializer> logger)
    {
        _unitOfWork = unitOfWork;
        _loader = loader;
        _environment = environment;
        _logger = logger;
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.MetadaDocuments.AnyAsync(
                cancellationToken))
        {
            _logger.LogInformation(
                "Document metadata already exists. Import skipped.");

            return;
        }

        var filePath = Path.Combine(
            _environment.ContentRootPath,
            "Data",
            "documents.xml");

        var documents = await _loader.LoadAsync(
            filePath,
            cancellationToken);

        if (documents.Count == 0)
        {
            _logger.LogWarning(
                "No valid document metadata was found.");
            return;
        }

        await _unitOfWork.MetadaDocuments.AddRangeAsync(
            documents,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Imported {Count} document metadata records.",
            documents.Count);
    }
}