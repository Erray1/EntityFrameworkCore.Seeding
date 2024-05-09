using EntityFrameworkCore.Seeding.Core.CreationPolicies;
using EntityFrameworkCore.Seeding.Modelling;

namespace EntityFrameworkCore.Seeding.Core.Creation;
public class SeederEntityCreationPolicyBuilder // singleton of scope
{
    private SeederEntityCreationPolicy? _currentPolicy;
    private readonly SeederEntityCreationPolicyFactory _policyFactory;
    private SeederEntityCreationPolicyOptions _currentOptions = new();

    public SeederEntityCreationPolicyBuilder(
        SeederEntityCreationPolicyFactory policyFactory
        )
    {
        _policyFactory = policyFactory;
    }
    public SeederEntityCreationPolicyBuilder OfEntity(SeederEntityInfo entity)
    {
        _currentOptions.EntityInfo = entity;
        return this;
    }

    public SeederEntityCreationPolicyBuilder WithNext(SeederEntityCreationPolicy next)
    {
        _currentOptions.Next = next;
        return this;
    }
    public SeederEntityCreationPolicyBuilder WithProperties(IEnumerable<SeederPropertyInfo> props, SeederDataCreationType dataCreationType)
    {
        _currentPolicy = _policyFactory.CreatePolicyFor(dataCreationType);
        _currentOptions.PropertisCreated = props;
        return this;
    }
    public SeederEntityCreationPolicy Build() {
        _currentPolicy!.SetOptions( _currentOptions );
        var policy = _currentPolicy;
        _currentPolicy = null;
        return policy;
    }
}

