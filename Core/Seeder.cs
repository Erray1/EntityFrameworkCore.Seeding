using EntityFrameworkCore.Seeding.Core.Binding;
using EntityFrameworkCore.Seeding.Core.Creation;
using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Seeding.Core;

public sealed class Seeder<TSeederModel, TDbContext> : ISeeder, IAsyncDisposable
    where TDbContext : DbContext
    where TSeederModel : SeederModel<TDbContext>
{
    private readonly SeederModelInfo _seederModel;
    private readonly SeederOptions _seederOptions;

    private readonly IServiceProvider _serviceProvider;
    private readonly TDbContext _dbContext;

    private readonly SeederEntityCreator _entitiesCreator;
    private Dictionary<SeederEntityInfo, IEnumerable<object>> _createdEntities;

    private readonly SeederEntityBinder _entityBinder;
    private readonly SeederEntityAdder<TDbContext> _entityAdder;

    public Seeder(
        IServiceProvider serviceProvider,
        SeederEntityCreator entityCreator,
        SeederEntityBinder entityBinder,
        SeederEntityAdder<TDbContext> entityAdder,
        TDbContext dbContext)
    {
        _seederOptions = serviceProvider.GetRequiredKeyedService<SeederOptionsProvider>(typeof(TSeederModel).Name).GetOptions();
        _serviceProvider = serviceProvider;
        _entitiesCreator = entityCreator;
        _entityBinder = entityBinder;
        _entityAdder = entityAdder;
        _dbContext = dbContext;
    }

    public async ValueTask DisposeAsync()
    {
        // Delete all seeded data if required
        // Dispose dbFactory
        _createdEntities.Clear();
    }

    public async Task ExecuteSeedingAsync(CancellationToken cancellationToken)
    {
        cancellationToken.Register(async () => await DisposeAsync());
        while (!cancellationToken.IsCancellationRequested)
        {
            _createdEntities = _entitiesCreator.CreateEntities()!;
            _entityBinder.BindEntities(_createdEntities, _dbContext.Model);
            await _entityAdder.AddEntities(_createdEntities);
        }
    }

}
