using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Infrastructure.DataImport;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {

        // add db connection
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")));

        // repositories and uow
        services.AddScoped<
            IDocumentMetadataRepository,
            DocumentMetadataRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // services live here
        services.AddScoped<DatabaseInitializer>();
        services.AddScoped<XmlDocumentMetadataLoader>();
        services.AddScoped<IDocumentMetadataService, DocumentMetadataService>();

        return services;
    }
}
