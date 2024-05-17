
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
    public SeederEntityCreator(
        IServiceProvider serviceProvider,
        IServiceScopeFactory serviceScopeFactory
        )
    {
        _seederModel = serviceProvider.GetRequiredKeyedService<SeederModelProvider>(typeof(TDbContext).Name).GetModel();
        _scopeFactory = serviceScopeFactory;
    }
    public Dictionary<SeederEntityInfo, List<object>> CreateEntities()
    {
        Dictionary<SeederEntityInfo, List<object>> createdEntities = new();
        using (var scope = _scopeFactory.CreateScope())
        {
            foreach (var entityInfo in _seederModel.Entities)
            {
                var entities = createEmptyEntities(entityInfo);

                if (entityNeedsFilling(entityInfo))
                {
                    var linker = scope.ServiceProvider.GetRequiredService<EntityCreationChainLinker>();
                    var chain = linker.CreateChainFor(entityInfo);
                    chain.FillEntities(entities);
                }

                createdEntities.Add(entityInfo, entities);
            }
            
        }
        return createdEntities;
    }

    private void createEntitiesInternal()
    {
        
        createEntitiesInternal();
    }
    private List<object> createEmptyEntities(SeederEntityInfo entityInfo)
    {
        var entities =  Enumerable.Range(0, entityInfo.TimesCreated)
            .Select(x => Activator.CreateInstance(entityInfo.EntityType)!)
            .ToList();
        return entities;
    }

    private bool entityNeedsFilling(SeederEntityInfo entity)
    {
        return entity.Properties
            .Where(x => x.DataCreationType != SeederDataCreationType.DoNotCreate)
            .Count() != 0;
    }
}

