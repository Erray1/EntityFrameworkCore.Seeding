using EntityFrameworkCore.Seeding.Core.Creation.CreationPolicies;
using EntityFrameworkCore.Seeding.Modelling;

using System.Collections.Immutable;


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
        SeederPropertiesCreationPolicy? current = _policyFactory.CreatePolicyFor(propsGroupedByCreationType.ElementAt(0).Key);

        var enumerator = propsGroupedByCreationType.GetEnumerator();

        _first = current;

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

        return _first;
    }
    private ImmutableDictionary<SeederDataCreationType, ImmutableList<SeederPropertyInfo>> groupPropertiesByCreationType(SeederEntityInfo entity)
    {
        var nonLoadedProps = entity.Properties
            .Where(x => x.PossibleLoadedValues is null)
            .GroupBy(x => x.DataCreationType)
            .ToImmutableDictionary(g => g.Key, g => g.ToImmutableList());

        var loadedProps = entity.Properties
            .Where(x => x.PossibleLoadedValues is not null)
            .GroupBy(x => x.PossibleLoadedValues)
            .ToImmutableDictionary(g => SeederDataCreationType.Loaded, g => g.ToImmutableList());

        return nonLoadedProps
            .Concat(loadedProps)
            .Where(x => x.Key != SeederDataCreationType.DoNotCreate)
            .ToImmutableDictionary();
    }
}

