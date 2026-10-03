using Application.Interfaces.DataImport;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.Tests.Persistence;

public class DatabaseInitializerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IDocumentMetadataRepository> _repositoryMock;
    private readonly Mock<IXmlDocumentMetadataLoader> _loaderMock;
    private readonly Mock<IHostEnvironment> _environmentMock;
    private readonly Mock<ILogger<DatabaseInitializer>> _loggerMock;

    private readonly DatabaseInitializer _initializer;

    public DatabaseInitializerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IDocumentMetadataRepository>();
        _loaderMock = new Mock<IXmlDocumentMetadataLoader>();

        _environmentMock = new Mock<IHostEnvironment>();
        _loggerMock = new Mock<ILogger<DatabaseInitializer>>();

        _unitOfWorkMock
            .SetupGet(x => x.MetadaDocuments)
            .Returns(_repositoryMock.Object);

        _initializer = new DatabaseInitializer(
            _unitOfWorkMock.Object,
            _loaderMock.Object,
            _environmentMock.Object,
            _loggerMock.Object);
    }

    #region InitializeAsync tests
    [Fact]
    public async Task InitializeAsync_WhenDocumentsAlreadyExist_SkipsImport()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.AnyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _initializer.InitializeAsync();

        // Assert
        _loaderMock.Verify(
            x => x.LoadAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _repositoryMock.Verify(
            x => x.AddRangeAsync(
                It.IsAny<IEnumerable<Domain.Entities.DocumentMetadata>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task InitializeAsync_WhenDatabaseIsEmpty_ImportsDocumentsAndSavesChanges()
    {
        // Arrange
        var documents = new List<DocumentMetadata>
    {
        new()
        {
            Id = 1,
            Title = "AML Guidelines",
            Description = "AML document",
            ResponsibleUnit = "Compliance Department",
            CreatedAt = DateTimeOffset.UtcNow,
            Url = "https://example.com/aml.pdf",
            FileType = "PDF",
            EstimatedReadingMinutes = 15,
            Importance = ImportanceLevel.High,
            Category = DocumentCategory.Internal,
            IsActive = true
        }
    };

        _repositoryMock
            .Setup(x => x.AnyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _environmentMock
            .SetupGet(x => x.ContentRootPath)
            .Returns("C:\\app");

        _loaderMock
            .Setup(x => x.LoadAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(documents);

        // Act
        await _initializer.InitializeAsync();

        // Assert
        _loaderMock.Verify(
            x => x.LoadAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _repositoryMock.Verify(
            x => x.AddRangeAsync(
                documents,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_WhenLoaderReturnsNoValidDocuments_DoesNotSaveChanges()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.AnyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _environmentMock
            .SetupGet(x => x.ContentRootPath)
            .Returns("C:\\app");

        _loaderMock
            .Setup(x => x.LoadAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<DocumentMetadata>());

        // Act
        await _initializer.InitializeAsync();

        // Assert
        _repositoryMock.Verify(
            x => x.AddRangeAsync(
                It.IsAny<IEnumerable<DocumentMetadata>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    #endregion
}
