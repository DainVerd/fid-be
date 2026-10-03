using Application.Common.Pagination;
using Application.Filters;
using Application.Interfaces.Repositories;
using AwesomeAssertions;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Services;
using Moq;

namespace Infrastructure.Tests.Services;

public class DocumentMetadataServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IDocumentMetadataRepository> _repositoryMock;
    private readonly DocumentMetadataService _service;

    public DocumentMetadataServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IDocumentMetadataRepository>();

        _unitOfWorkMock
            .SetupGet(x => x.MetadaDocuments)
            .Returns(_repositoryMock.Object);

        _service = new DocumentMetadataService(
            _unitOfWorkMock.Object);
    }

    #region tests GetPaginatedAsync
    [Fact]
    public async Task GetPaginatedAsync_WhenDocumentsExist_ReturnsMappedDtos()
    {
        // Arrange
        var filter = new DocumentMetadataFilter();

        var pagination = new PaginationParams
        {
            PageNumber = 1,
            PageSize = 10
        };

        var documents = new List<DocumentMetadata>
        {
            new()
            {
                Id = 1,
                Title = "AML Guidelines",
                Description = "AML document",
                ResponsibleUnit = "Compliance Department",
                CreatedAt = new DateTimeOffset(
                    2026, 10, 1, 10, 0, 0, TimeSpan.Zero),
                Url = "https://example.com/aml.pdf",
                FileType = "PDF",
                EstimatedReadingMinutes = 15,
                Importance = ImportanceLevel.High,
                Category = DocumentCategory.Internal,
                IsActive = true
            }
        };

        var repositoryResult =
            new PaginatedList<DocumentMetadata>(
                documents,
                count: 1,
                pageNumber: 1,
                pageSize: 10);

        _repositoryMock
            .Setup(x => x.GetPaginatedAsync(
                filter,
                pagination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        // Act
        var result = await _service.GetPaginatedAsync(
            filter,
            pagination);

        // Assert
        result.Should().NotBeNull();

        result.TotalCount.Should().Be(1);
        result.PageNumber.Should().Be(1);

        result.Items.Should().ContainSingle();

        var document = result.Items.Single();

        document.Id.Should().Be(1);
        document.Title.Should().Be("AML Guidelines");
        document.ResponsibleUnit.Should()
            .Be("Compliance Department");

        document.Importance.Should()
            .Be(ImportanceLevel.High);

        document.Category.Should()
            .Be(DocumentCategory.Internal);

        document.IsActive.Should().BeTrue();

        _repositoryMock.Verify(
            x => x.GetPaginatedAsync(
                filter,
                pagination,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetPaginatedAsync_WhenRepositoryReturnsMultiplePages_PreservesPaginationMetadata()
    {
        // Arrange
        var filter = new DocumentMetadataFilter();

        var pagination = new PaginationParams
        {
            PageNumber = 2,
            PageSize = 10
        };

        var documents = Enumerable
            .Range(1, 10)
            .Select(id => new DocumentMetadata
            {
                Id = id,
                Title = $"Document {id}",
                ResponsibleUnit = "Compliance Department",
                CreatedAt = DateTimeOffset.UtcNow,
                Url = $"https://example.com/{id}.pdf",
                FileType = "PDF",
                EstimatedReadingMinutes = 10,
                Importance = ImportanceLevel.Medium,
                Category = DocumentCategory.Internal,
                IsActive = true
            })
            .ToList();

        var repositoryResult =
            new PaginatedList<DocumentMetadata>(
                documents,
                count: 25,
                pageNumber: 2,
                pageSize: 10);

        _repositoryMock
            .Setup(x => x.GetPaginatedAsync(
                filter,
                pagination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        // Act
        var result = await _service.GetPaginatedAsync(
            filter,
            pagination);

        // Assert
        result.PageNumber.Should().Be(2);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);

        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();

        result.Items.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetPaginatedAsync_WhenNoDocumentsExist_ReturnsEmptyPage()
    {
        // Arrange
        var filter = new DocumentMetadataFilter();

        var pagination = new PaginationParams
        {
            PageNumber = 1,
            PageSize = 10
        };

        var repositoryResult =
            new PaginatedList<DocumentMetadata>(
                new List<DocumentMetadata>(),
                count: 0,
                pageNumber: 1,
                pageSize: 10);

        _repositoryMock
            .Setup(x => x.GetPaginatedAsync(
                filter,
                pagination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        // Act
        var result = await _service.GetPaginatedAsync(
            filter,
            pagination);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.PageNumber.Should().Be(1);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();

        _repositoryMock.Verify(
            x => x.GetPaginatedAsync(
                filter,
                pagination,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    #endregion
}