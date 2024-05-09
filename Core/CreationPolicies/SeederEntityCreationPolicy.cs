using EntityFrameworkCore.Seeding.Core.Creation;
using EntityFrameworkCore.Seeding.Modelling;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace EntityFrameworkCore.Seeding.Core.CreationPolicies;
public abstract class SeederEntityCreationPolicy
{
    protected SeederEntityInfo _entityInfo;
    
    protected List<SeederPropertyInfo> _propertiesFilledWithPolicy;
    protected List<SeederPropertyInfo>.Enumerator _propertiesEnumerator;
    protected IReadOnlyDictionary<SeederPropertyInfo, IEnumerable<object>> _propertyPool;
    protected SeederEntityCreationPolicy? _next;
    
    public void SetOptions(SeederEntityCreationPolicyOptions options)
    {
        _entityInfo = options.EntityInfo;
        _next = options.Next;
        _propertiesFilledWithPolicy = options.PropertisCreated.ToList();
    }

    public void FillEntities(IEnumerable<object> entities)
    {
        createPropertiesPool();
        mapDataFromPoolToEntities(entities);
    }

    protected abstract void createPropertiesPool();
    protected void mapDataFromPoolToEntities(IEnumerable<object> entities)
    {
        var entityPropertyInfo = _entityInfo.EntityType.GetProperties();
        foreach (var entitiy in entities)
        {
            var propertiesValues = _propertyPool
                .Select(x => 
                    new KeyValuePair<SeederPropertyInfo, object>
                    (x.Key, 
                        x.Value
                        .ElementAt(Random.Shared.Next
                            (0, x.Value.Count()))))
                .ToImmutableDictionary();
            // Later
            foreach (var propertyValue in propertiesValues)
            {
                var property = entityPropertyInfo.Single(x => x.Name == propertyValue.Key.PropertyName);
                property.SetValue(entitiy, propertyValue.Value, null);
            }

        }
    }

}

