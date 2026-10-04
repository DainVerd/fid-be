using Application.Common.Pagination;
using Application.Filters;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DocumentMetadataRepository : IDocumentMetadataRepository
{
    private readonly AppDbContext _context;

    public DocumentMetadataRepository(AppDbContext context)
    {
        _context = context;
    }
    public Task<bool> AnyAsync(
           CancellationToken cancellationToken = default)
    {
        return _context.MetadaDocuments.AnyAsync(cancellationToken);
    }

    public Task<DocumentMetadata?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.MetadaDocuments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<PaginatedList<DocumentMetadata>> GetPaginatedAsync(
        DocumentMetadataFilter filter,
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var query = _context.MetadaDocuments
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(x =>
                x.Title.Contains(filter.Search) ||
                (x.Description != null &&
                 x.Description.Contains(filter.Search)));

        if (!string.IsNullOrWhiteSpace(filter.ResponsibleUnit))
            query = query.Where(x =>
                x.ResponsibleUnit == filter.ResponsibleUnit);

        if (!string.IsNullOrWhiteSpace(filter.FileType))
            query = query.Where(x =>
                x.FileType == filter.FileType);

        if (filter.Importance.HasValue)
            query = query.Where(x =>
                x.Importance == filter.Importance.Value);

        if (filter.Category.HasValue)
            query = query.Where(x =>
                x.Category == filter.Category.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(x =>
                x.IsActive == filter.IsActive.Value);

        query = pagination.SortBy?.ToLowerInvariant() switch
        {
            "title" => pagination.IsDescending
                ? query.OrderByDescending(x => x.Title)
                : query.OrderBy(x => x.Title),

            "createdat" => pagination.IsDescending
                ? query.OrderByDescending(x => x.CreatedAt)
                : query.OrderBy(x => x.CreatedAt),

            "estimatedreadingminutes" => pagination.IsDescending
                ? query.OrderByDescending(x => x.EstimatedReadingMinutes)
                : query.OrderBy(x => x.EstimatedReadingMinutes),

            "importance" => pagination.IsDescending
                ? query.OrderByDescending(x => x.Importance)
                : query.OrderBy(x => x.Importance),

            "responsibleunit" => pagination.IsDescending
               ? query.OrderByDescending(x => x.ResponsibleUnit)
               : query.OrderBy(x => x.ResponsibleUnit),

            "filetype" => pagination.IsDescending
               ? query.OrderByDescending(x => x.FileType)
               : query.OrderBy(x => x.FileType),

            _ => query.OrderBy(x => x.Id)
        };

        return await PaginatedList<DocumentMetadata>.CreateAsync(
            query,
            pagination.PageNumber,
            pagination.PageSize,
            cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<DocumentMetadata> documents,
        CancellationToken cancellationToken = default)
    {
        await _context.MetadaDocuments.AddRangeAsync(
            documents,
            cancellationToken);
    }
}
