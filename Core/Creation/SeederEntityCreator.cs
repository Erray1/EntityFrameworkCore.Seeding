
using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Seeding.Core.Creation;
public sealed class SeederEntityCreator<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly SeederModelInfo _seederModel;
    private List<SeederEntityInfo>.Enumerator _entitiesEnumerator;

    private Dictionary<SeederEntityInfo, IEnumerable<object>> _createdEntities = new();
    public SeederEntityCreator(
        IServiceProvider serviceProvider,
        IServiceScopeFactory serviceScopeFactory
        )
    {
        _seederModel = serviceProvider.GetRequiredKeyedService<SeederModelProvider>(typeof(TDbContext).Name).GetModel();
        _entitiesEnumerator = _seederModel.Entities.GetEnumerator();
        _entitiesEnumerator.MoveNext();
        _scopeFactory = serviceScopeFactory;
    }
    public Dictionary<SeederEntityInfo, IEnumerable<object>> CreateEntities()
    {
        createEntitiesInternal();
        return _createdEntities;
    }

    private void createEntitiesInternal()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var linker = scope.ServiceProvider.GetRequiredService<EntityCreationChainLinker>();

            var current = _entitiesEnumerator.Current;
            var chain = linker.CreateChainFor(current);
            var entities = createEmptyEntities(current);
            chain.FillEntities(entities);
            _createdEntities.Add(current, entities);
        }
        
        var isEnd = !_entitiesEnumerator.MoveNext();
        if (isEnd) return;
        createEntitiesInternal();
    }
    private List<object> createEmptyEntities(SeederEntityInfo entityInfo)
    {
        return Enumerable.Range(0, entityInfo.TimesCreated)
            .Select(x => Activator.CreateInstance(entityInfo.EntityType)!)
            .ToList();
    }
}

