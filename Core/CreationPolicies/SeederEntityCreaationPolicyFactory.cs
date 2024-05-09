using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace EntityFrameworkCore.Seeding.Core.CreationPolicies;
public class SeederEntityCreationPolicyFactory
{
    private static ReadOnlyDictionary<SeederDataCreationType, Type> _policies = new(new Dictionary<SeederDataCreationType, Type>()
    {
        {SeederDataCreationType.FromJSON, typeof(EntityLoadedFromJsonPolicy)},
        {SeederDataCreationType.FromGivenPool, typeof(EntityCreatedFromGivenPoolPolicy)},
        {SeederDataCreationType.Random, typeof(EntityCreatedRandomlyPolicy)},
        {SeederDataCreationType.Loaded, typeof(EntityLoadedFromGitHubPolicy)}
    });
    private readonly IServiceProvider _serviceProvider;
    public SeederEntityCreationPolicyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public SeederEntityCreationPolicy CreatePolicyFor(SeederDataCreationType dataCreationType)
    {
        var policyType = _policies[dataCreationType];
        return (SeederEntityCreationPolicy)_serviceProvider.GetRequiredService(policyType);
    }
}

