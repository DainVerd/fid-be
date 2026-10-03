using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    IDocumentMetadataRepository MetadaDocuments { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
