using Application.Common.Pagination;
using Application.DTOs;
using Application.Filters;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Infrastructure.Services;

public class DocumentMetadataService : IDocumentMetadataService
{
    private readonly IUnitOfWork _unitOfWork;

    public DocumentMetadataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedList<DocumentMetadataDto>> GetPaginatedAsync(
        DocumentMetadataFilter filter,
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.MetadaDocuments.GetPaginatedAsync(
            filter,
            pagination,
            cancellationToken);

        var items = result.Items
            .Select(x => new DocumentMetadataDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                ResponsibleUnit = x.ResponsibleUnit,
                CreatedAt = x.CreatedAt,
                Url = x.Url,
                FileType = x.FileType,
                EstimatedReadingMinutes = x.EstimatedReadingMinutes,
                Importance = x.Importance,
                Category = x.Category,
                IsActive = x.IsActive
            })
            .ToList();

        return new PaginatedList<DocumentMetadataDto>(
            items,
            result.TotalCount,
            result.PageNumber,
            pagination.PageSize);
    }
}
