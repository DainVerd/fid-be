using Application.Interfaces.Repositories;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(
        AppDbContext context,
        IDocumentMetadataRepository IDocumentMetadataRepository)
    {
        _context = context;
        MetadaDocuments = IDocumentMetadataRepository;
    }

    public IDocumentMetadataRepository MetadaDocuments { get; }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}