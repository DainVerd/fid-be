using Application.Common.Pagination;
using Application.DTOs;
using Application.Filters;

namespace Application.Interfaces.Services;

public interface IDocumentMetadataService
{
    Task<PaginatedList<DocumentMetadataDto>> GetPaginatedAsync(
        DocumentMetadataFilter filter,
        PaginationParams pagination,
        CancellationToken cancellationToken = default);
}
