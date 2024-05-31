using EntityFrameworkCore.Seeding.Core.Creation.CreationPolicies;
using EntityFrameworkCore.Seeding.Modelling;

using System.Collections.Immutable;
using System.Linq;


namespace EntityFrameworkCore.Seeding.Core.Creation;
public class EntityCreationChainLinker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SeederEntityCreationPolicyFactory _policyFactory;
    private SeederPropertiesCreationPolicy _first;
    public EntityCreationChainLinker(
        IServiceProvider serviceProvider,
        SeederEntityCreationPolicyFactory factory)
    {
        _serviceProvider = serviceProvider;
        _policyFactory = factory;
    }

    // Если загружаются только отдельные props, то создать несколько независимых этапов для каждой
    // Если загружается целая сущность, то создать неделимый этап (который надо реализовать быбыб)

    public SeederPropertiesCreationPolicy CreateChainFor(SeederEntityInfo entity)
    {
        var options = new SeederEntityCreationPolicyOptions();
        var propsGroupedByCreationType = groupPropertiesByCreationType(entity);
        if (propsGroupedByCreationType.Count == 0)
        {
            throw new Exception($"Do not need to create props. Use DoNotCreate() method on entity {entity.EntityType.Name}");
        }
        SeederPropertiesCreationPolicy? current = _policyFactory.CreatePolicyFor(propsGroupedByCreationType.ElementAt(0).Key);
        var enumerator = propsGroupedByCreationType.GetEnumerator();
        _first = current;

        if (propsGroupedByCreationType.Count == 1)
        {
            options.PropertiesCreated = propsGroupedByCreationType.First().Value;
            options.EntityInfo = entity;
            _first.SetOptions(options);
        }
        

        for (int i = 1; i < propsGroupedByCreationType.Count; i++)
        {
            var currentProps = propsGroupedByCreationType.ElementAt(i - 1).Value;
            
            var nextCreationType = propsGroupedByCreationType.ElementAt(i).Key;
            var next = _policyFactory.CreatePolicyFor(nextCreationType);

            options.Next = next;
            options.PropertiesCreated = currentProps;
            options.EntityInfo = entity;

            current.SetOptions(options);

            current = next;
            if (i == propsGroupedByCreationType.Count - 1)
            {
                currentProps = propsGroupedByCreationType.ElementAt(i).Value;
                options.Next = null;
                options.PropertiesCreated = currentProps;
                options.EntityInfo = entity;
                current.SetOptions(options);
            }
        }
        enumerator.Reset();

        return _first;
    }
    private ImmutableDictionary<SeederDataCreationType, ImmutableList<SeederPropertyInfo>> groupPropertiesByCreationType(SeederEntityInfo entity)
    {
        var nonLoadedProps = entity.Properties
            .Where(x => x.DataCreationType != SeederDataCreationType.FromJSON && x.DataCreationType != SeederDataCreationType.Loaded)
            .GroupBy(x => x.DataCreationType)
            .Select(x => new KeyValuePair<SeederDataCreationType, ImmutableList<SeederPropertyInfo>>
                (
                x.Key,
                x.ToImmutableList()
             ));

        var jsonProps = entity.Properties
            .Where(x => x.DataCreationType == SeederDataCreationType.FromJSON)
            .GroupBy(x => x.JsonInfo!.JsonAbsolutePath)
            .Select(x => new KeyValuePair<SeederDataCreationType, ImmutableList<SeederPropertyInfo>>
                (
                SeederDataCreationType.FromJSON,
                x.ToImmutableList()
             ));

        var loadedProps = entity.Properties
            .Where(x => x.DataCreationType == SeederDataCreationType.Loaded)
            .GroupBy(x => x.PropertyStockCollection!.ParentEntityCollection)
            .Select(x => new KeyValuePair<SeederDataCreationType, ImmutableList<SeederPropertyInfo>>
                (
                SeederDataCreationType.Loaded,
                x.ToImmutableList()
             ));

        return nonLoadedProps
            .Concat(loadedProps)
            .Concat(jsonProps)
            .Where(x => x.Key != SeederDataCreationType.DoNotCreate)
            .ToImmutableDictionary();
    }
}

