using EntityFrameworkCore.Seeding.Core.CreationPolicies;
using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;

namespace EntityFrameworkCore.Seeding.Core.Creation;
public sealed class SeederEntityCreator
{
    private readonly SeederModelInfo _seederModel;
    private List<SeederEntityInfo>.Enumerator _entitiesEnumerator;

    private readonly EntityCreationChainLinker _linker;

    private Dictionary<SeederEntityInfo, IEnumerable<object>> _createdEntities = new();
    public SeederEntityCreator(
        IServiceProvider serviceProvider,
        EntityCreationChainLinker linker
        )
    {
        _seederModel = seederModelProvider.GetModel();
        _entitiesEnumerator = _seederModel.Entities.GetEnumerator();
        _linker = linker;

    }
    public Dictionary<SeederEntityInfo, IEnumerable<object>> CreateEntities()
    {
        createEntitiesInternal();
        return _createdEntities;
    }

    private void createEntitiesInternal()
    {
        var current = _entitiesEnumerator.Current;
        var chain = _linker.CreateChainFor(current);
        var entities = createEmptyEntities(current);
        chain.FillEntities(entities);

        _createdEntities.Add(current, entities);

        var isEnd = !_entitiesEnumerator.MoveNext();
        if (isEnd) return;
        createEntitiesInternal();
    }
    private IEnumerable<object> createEmptyEntities(SeederEntityInfo entityInfo)
    {
        return Enumerable.Range(0, entityInfo.TimesCreated + entityInfo.Locality)
            .Select(x => Activator.CreateInstance(entityInfo.EntityType)!);

    }
}

