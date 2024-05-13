using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace EntityFrameworkCore.Seeding.Core.Creation.CreationPolicies;
public class SeederEntityCreationPolicyFactory
{
    private static ReadOnlyDictionary<SeederDataCreationType, Type> _policies = new(new Dictionary<SeederDataCreationType, Type>()
    {
        {SeederDataCreationType.FromJSON, typeof(PropertiesLoadedFromJsonPolicy)},
        {SeederDataCreationType.FromGivenPool, typeof(PropertiesCreatedFromGivenPoolPolicy)},
        {SeederDataCreationType.Random, typeof(PropertiesCreatedRandomlyPolicy)},
        {SeederDataCreationType.Loaded, typeof(PropertiesLoadedFromGitHubPolicy)},
        {SeederDataCreationType.CreatedID, typeof(EntityHasGeneratedIDPolicy)}
    });
    private readonly IServiceProvider _serviceProvider;
    public SeederEntityCreationPolicyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public SeederPropertiesCreationPolicy CreatePolicyFor(SeederDataCreationType dataCreationType)
    {
        var policyType = _policies[dataCreationType];
        return (SeederPropertiesCreationPolicy)_serviceProvider.GetRequiredService(policyType);
    }
}

