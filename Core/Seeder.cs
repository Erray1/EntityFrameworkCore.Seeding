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
    private readonly SeederModelProvider _modelProvider;
    private readonly SeederOptionsProvider _optionsProvider;
    private readonly DbContextFactory<TDbContext> _dbContextFactory;
    private TDbContext _dbContext;
    private IModel _dbModel => _dbContext.Model;
    private readonly SeederModelInfo _seederModel;
    private readonly SeederOptions _seederOptions;

    private readonly IServiceProvider _serviceProvider;
    private Dictionary<SeederEntityInfo, IEnumerable<object>> _createdEntities;


    public Seeder(
        IKeyedServiceProvider keyedServiceProvider,
        DbContextFactory<TDbContext> dbContextFactory,
        IServiceProvider serviceProvider,
        TDbContext dbContext)
    {
        _modelProvider = keyedServiceProvider.GetRequiredKeyedService<SeederModelProvider>(typeof(TSeederModel).Name);
        _seederModel = _modelProvider.GetModel();
        _dbContextFactory = dbContextFactory; // CHECK
        _seederOptions = keyedServiceProvider.GetRequiredKeyedService<SeederOptionsProvider>(typeof(TSeederModel).Name).GetOptions();
        _serviceProvider = serviceProvider;
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
            _createdEntities = SeederEntityCreator.CreateNew(_seederModel).CreateEntities()!;
            SeederEntityBinder.CreateBinder(_dbModel, _createdEntities).BindEntities();
            await AddEntititesToDatabase();
        }
    }

    private async Task AddEntititesToDatabase()
    {
        await _dbContext.SaveChangesAsync();
    }
}
