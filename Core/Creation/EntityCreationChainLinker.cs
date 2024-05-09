using EntityFrameworkCore.Seeding.Core.CreationPolicies;
using EntityFrameworkCore.Seeding.Modelling;

using System.Collections.Immutable;


namespace EntityFrameworkCore.Seeding.Core.Creation;
public class EntityCreationChainLinker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SeederEntityCreationPolicyFactory _policyFactory;
    private SeederEntityCreationPolicy _first;
    public EntityCreationChainLinker(
        IServiceProvider serviceProvider,
        SeederEntityCreationPolicyFactory factory)
    {
        _serviceProvider = serviceProvider;
        _policyFactory = factory;
    }

    // Если загружаются только отдельные props, то создать несколько независимых этапов для каждой
    // Если загружается целая сущность, то создать неделимый этап (который надо реализовать быбыб)

    public SeederEntityCreationPolicy CreateChainFor(SeederEntityInfo entity)
    {
        var options = new SeederEntityCreationPolicyOptions();
        var propsGroupedByCreationType = groupPropertiesByCreationType(entity);
        SeederEntityCreationPolicy? current = _policyFactory.CreatePolicyFor(propsGroupedByCreationType.ElementAt(0).Key);

        for (int i = 1; i < propsGroupedByCreationType.Count; i++)
        {
            var (creationType, props) = propsGroupedByCreationType.ElementAt(i);

            var next = _policyFactory.CreatePolicyFor(creationType);

            options.Next = next;
            options.PropertisCreated = props;
            options.EntityInfo = entity;

            current.SetOptions(options);

            current = next;
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
            .GroupBy (x => x.PossibleLoadedValues)
            .ToImmutableDictionary(g => SeederDataCreationType.Loaded, g => g.ToImmutableList());

        return nonLoadedProps.Concat(loadedProps).ToImmutableDictionary();
    }
}

