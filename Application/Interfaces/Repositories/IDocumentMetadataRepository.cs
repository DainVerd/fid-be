using Application.Common.Pagination;
using Application.Filters;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IDocumentMetadataRepository
{
    Task<bool> AnyAsync(
        CancellationToken cancellationToken = default);

    Task<PaginatedList<DocumentMetadata>> GetPaginatedAsync(
    DocumentMetadataFilter filter,
    PaginationParams pagination,
    CancellationToken cancellationToken = default);

    Task<DocumentMetadata?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<DocumentMetadata> documents,
        CancellationToken cancellationToken = default);
}
