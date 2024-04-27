using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace EntityFrameworkCore.Seeding.Core.CreationPolicies;
public class SeederEntityCreaationPolicyFactory
{
    private static ReadOnlyDictionary<DataCreationType, Type> _policies = new(new Dictionary<DataCreationType, Type>()
    {
        {DataCreationType.FromJSON, typeof(object)},
        {DataCreationType.FromGivenPool, typeof(object)},
        {DataCreationType.Random, typeof(object)},
        {DataCreationType.Loaded, typeof(object)},
    });
    private readonly IServiceProvider _serviceProvider;
    public SeederEntityCreaationPolicyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISeederEntityCreationPolicy CreatePolicyFor(SeederEntityInfo entity)
    {
        var creationType = getCreationTypeFor(entity);
        var policyType = _policies[creationType];
        return (ISeederEntityCreationPolicy)_serviceProvider.GetRequiredService(policyType);
    }

    private DataCreationType getCreationTypeFor(SeederEntityInfo entity)
    {
        throw new NotImplementedException();
    }
}

