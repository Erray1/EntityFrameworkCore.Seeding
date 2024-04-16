using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;

namespace EntityFrameworkCore.Seeding.Core;

public class Seeder<TSeederModel, TDbContext> : ISeeder, IAsyncDisposable
    where TDbContext : DbContext
    where TSeederModel : SeederModel<TDbContext>
{
    private readonly SeederModelProvider _modelProvider;
    private readonly SeederOptionsProvider _optionsProvider;
    private readonly DbContextFactory<TDbContext> _dbContextFactory;
    public Seeder(
        IKeyedServiceProvider keyedServiceProvider,
        DbContextFactory<TDbContext> dbContextFactory)
    {
        _modelProvider = keyedServiceProvider.GetRequiredKeyedService<SeederModelProvider>(typeof(TSeederModel).Name);
        _optionsProvider = keyedServiceProvider.GetRequiredKeyedService<SeederOptionsProvider>(typeof(TSeederModel).Name);
        _dbContextFactory = dbContextFactory; // CHECK
    }

    public async ValueTask DisposeAsync()
    {
        // Delete all seeded data if required
        // Dispose dbFactory
    }

    public async Task ExecuteSeedingAsync(CancellationToken cancellationToken)
    {
        var model = _modelProvider.GetModel();
        var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var commandChain = new SeedingCommandChainLinker()
            .Default();

    }
}
