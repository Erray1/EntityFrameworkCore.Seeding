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

/// <summary>
///     Main class used for seeding data.
///     Can be injected into other services.
/// </summary>
/// <typeparam name="TSeederModel">Type of used SeederModel</typeparam>
/// <typeparam name="TDbContext">Type of used DbContext</typeparam>
public sealed class Seeder<TSeederModel, TDbContext> : ISeeder //, IAsyncDisposable
    where TDbContext : DbContext
    where TSeederModel : SeederModel<TDbContext>
{

    private readonly SeederEntityCreator<TDbContext> _entitiesCreator;
    private Dictionary<SeederEntityInfo, List<object>> _createdEntities;

    private readonly SeederEntityBinder<TDbContext> _entityBinder;
    private readonly SeederEntityAdder<TDbContext> _entityAdder;

    public Seeder(
        IServiceProvider serviceProvider,
        SeederEntityBinder<TDbContext> entityBinder,
        SeederEntityAdder<TDbContext> entityAdder)
    {
        _entitiesCreator = serviceProvider.GetRequiredService<SeederEntityCreator<TDbContext>>();
        _entityBinder = entityBinder;
        _entityAdder = entityAdder;
    }
    /// <summary>
    ///     Starts the seeding process
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ExecuteSeedingAsync(CancellationToken cancellationToken)
    {
        // cancellationToken.Register(async () => await DisposeAsync());
        while (!cancellationToken.IsCancellationRequested)
        {
            _createdEntities = _entitiesCreator.CreateEntities()!;
            _entityBinder.BindEntities(_createdEntities);
            await _entityAdder.AddEntities(_createdEntities);
        }
    }

}
