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

    private readonly SeederEntityCreator _entitiesCreator;
    private Dictionary<SeederEntityInfo, IEnumerable<object>> _createdEntities;

    private readonly SeederEntityBinder _entityBinder;
    private readonly SeederEntityAdder<TDbContext> _entityAdder;

    public Seeder(
        IKeyedServiceProvider keyedServiceProvider,
        IServiceProvider serviceProvider,
        SeederEntityCreator entityCreator,
        SeederEntityBinder entityBinder,
        SeederEntityAdder<TDbContext> entityAdder)
    {
        _seederOptions = keyedServiceProvider.GetRequiredKeyedService<SeederOptionsProvider>(typeof(TSeederModel).Name).GetOptions();
        _serviceProvider = serviceProvider;
        _entitiesCreator = entityCreator;
        _entityBinder = entityBinder;
        _entityAdder = entityAdder;
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
            _entityBinder.BindEntities(_createdEntities);
            await _entityAdder.AddEntities(_createdEntities);
        }
    }

}
