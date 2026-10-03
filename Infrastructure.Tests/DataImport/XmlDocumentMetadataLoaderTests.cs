using AwesomeAssertions;
using Domain.Constants;
using Infrastructure.DataImport;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.Tests.DataImport;

public class XmlDocumentMetadataLoaderTests
{
    private readonly Mock<ILogger<XmlDocumentMetadataLoader>> _loggerMock;
    private readonly XmlDocumentMetadataLoader _loader;

    public XmlDocumentMetadataLoaderTests()
    {
        _loggerMock = new Mock<ILogger<XmlDocumentMetadataLoader>>();
        _loader = new XmlDocumentMetadataLoader(_loggerMock.Object);
    }

    #region LoadAsync tests
    [Fact]
    public async Task LoadAsync_WhenXmlIsValid_ReturnsDocuments()
    {
        // Arrange
        var xml = """
            <documents>
              <document>
                <title>AML Compliance Guidelines</title>
                <description>Internal AML guidance.</description>
                <responsibleUnit>Compliance Department</responsibleUnit>
                <createdAt>2026-10-01T10:30:00+03:00</createdAt>
                <url>https://example.com/aml.pdf</url>
                <fileType>PDF</fileType>
                <estimatedReadingMinutes>15</estimatedReadingMinutes>
                <importance>High</importance>
                <category>Internal</category>
                <isActive>true</isActive>
              </document>
            </documents>
            """;

        var filePath = await CreateTempXmlFileAsync(xml);

        try
        {
            // Act
            var result = await _loader.LoadAsync(filePath);

            // Assert
            result.Should().ContainSingle();

            var document = result.Single();

            document.Title.Should().Be("AML Compliance Guidelines");
            document.Description.Should().Be("Internal AML guidance.");
            document.ResponsibleUnit.Should().Be("Compliance Department");
            document.FileType.Should().Be("PDF");
            document.EstimatedReadingMinutes.Should().Be(15);
            document.IsActive.Should().BeTrue();
            document.Importance.Should().Be(ImportanceLevel.High);
            document.Category.Should().Be(DocumentCategory.Internal);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task LoadAsync_WhenOneDocumentIsInvalid_SkipsInvalidDocument()
    {
        // Arrange
        // one of the xml documents contain invalid data for import
        var xml = """
        <documents>
          <document>
            <title>Valid AML Document</title>
            <description>Valid document.</description>
            <responsibleUnit>Compliance Department</responsibleUnit>
            <createdAt>2026-10-01T10:30:00+03:00</createdAt>
            <url>https://example.com/valid.pdf</url>
            <fileType>PDF</fileType>
            <estimatedReadingMinutes>15</estimatedReadingMinutes>
            <importance>High</importance>
            <category>Internal</category>
            <isActive>true</isActive>
          </document>

          <document>
            <title>Broken Document</title>
            <description>Invalid document.</description>
            <responsibleUnit>Risk Management</responsibleUnit>
            <createdAt>not-a-date</createdAt>
            <url>https://example.com/broken.pdf</url>
            <fileType>PDF</fileType>
            <estimatedReadingMinutes>20</estimatedReadingMinutes>
            <importance>Medium</importance>
            <category>Internal</category>
            <isActive>true</isActive>
          </document>
        </documents>
        """;

        var filePath = await CreateTempXmlFileAsync(xml);

        try
        {
            // Act
            var result = await _loader.LoadAsync(filePath);

            // Assert
            result.Should().ContainSingle();

            var document = result.Single();

            document.Title.Should().Be("Valid AML Document");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task LoadAsync_WhenImportanceIsInvalid_SkipsDocument()
    {
        // Arrange
        var xml = """
        <documents>
          <document>
            <title>Broken Importance Document</title>
            <description>Invalid importance value.</description>
            <responsibleUnit>Compliance Department</responsibleUnit>
            <createdAt>2026-10-01T10:30:00+03:00</createdAt>
            <url>https://example.com/broken.pdf</url>
            <fileType>PDF</fileType>
            <estimatedReadingMinutes>15</estimatedReadingMinutes>
            <importance>SuperCritical</importance>
            <category>Internal</category>
            <isActive>true</isActive>
          </document>
        </documents>
        """;

        var filePath = await CreateTempXmlFileAsync(xml);

        try
        {
            // Act
            var result = await _loader.LoadAsync(filePath);

            // Assert
            result.Should().BeEmpty();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task LoadAsync_WhenEstimatedReadingMinutesIsInvalid_SkipsDocument()
    {
        // Arrange
        var xml = """
        <documents>
          <document>
            <title>Broken Reading Time Document</title>
            <description>Invalid reading time value.</description>
            <responsibleUnit>Compliance Department</responsibleUnit>
            <createdAt>2026-10-01T10:30:00+03:00</createdAt>
            <url>https://example.com/broken.pdf</url>
            <fileType>PDF</fileType>
            <estimatedReadingMinutes>abc</estimatedReadingMinutes>
            <importance>High</importance>
            <category>Internal</category>
            <isActive>true</isActive>
          </document>
        </documents>
        """;

        var filePath = await CreateTempXmlFileAsync(xml);

        try
        {
            // Act
            var result = await _loader.LoadAsync(filePath);

            // Assert
            result.Should().BeEmpty();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task LoadAsync_WhenEstimatedReadingMinutesIsZero_SkipsDocument()
    {
        // Arrange
        var xml = """
        <documents>
          <document>
            <title>Zero Reading Time Document</title>
            <description>Invalid reading time value.</description>
            <responsibleUnit>Compliance Department</responsibleUnit>
            <createdAt>2026-10-01T10:30:00+03:00</createdAt>
            <url>https://example.com/broken.pdf</url>
            <fileType>PDF</fileType>
            <estimatedReadingMinutes>0</estimatedReadingMinutes>
            <importance>High</importance>
            <category>Internal</category>
            <isActive>true</isActive>
          </document>
        </documents>
        """;

        var filePath = await CreateTempXmlFileAsync(xml);

        try
        {
            // Act
            var result = await _loader.LoadAsync(filePath);

            // Assert
            result.Should().BeEmpty();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    #endregion

    private static async Task<string> CreateTempXmlFileAsync(string content)
    {
        var filePath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid()}.xml");

        await File.WriteAllTextAsync(filePath, content);

        return filePath;
    }
}
