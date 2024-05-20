using EntityFrameworkCore.Seeding.Core.Creation;
using EntityFrameworkCore.Seeding.Modelling;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EntityFrameworkCore.Seeding.Core.Creation.CreationPolicies;
public abstract class SeederPropertiesCreationPolicy
{
    protected SeederEntityInfo _entityInfo;

    protected List<SeederPropertyInfo> _propertiesFilledWithPolicy;
    protected List<SeederPropertyInfo>.Enumerator _propertiesEnumerator;
    protected IReadOnlyDictionary<SeederPropertyInfo, List<object>> _propertyPool;
    protected SeederPropertiesCreationPolicy? _next;

    public void SetOptions(SeederEntityCreationPolicyOptions options)
    {
        _entityInfo = options.EntityInfo;
        _next = options.Next;
        _propertiesFilledWithPolicy = options.PropertiesCreated.ToList();
    }

    public void FillEntities(List<object> entities)
    {
        createPropertiesPool();
        mapDataFromPoolToEntities(entities);
        if (_next is not null)
        {
            _next.FillEntities(entities);
        }
    }

    protected abstract void createPropertiesPool();
    protected void mapDataFromPoolToEntities(List<object> entities)
    {
        foreach (var (propertyInfo, pool) in _propertyPool)
        {
            PropertyInfo propertyOfEntity = _entityInfo.EntityType
                .GetProperties()
                .Single(x => x.Name == propertyInfo.PropertyName
                            && x.PropertyType == propertyInfo.PropertyType);

            Span<object> poolAsSpan = CollectionsMarshal.AsSpan(pool.ToList());

            if (propertyInfo.ShuffleValues)
            {
                new Random(propertyInfo.GetHashCode()).Shuffle(poolAsSpan);
            }

            int poolLength = poolAsSpan.Length;
            for (int i = 0; i < entities.Count(); i++)
            {
                int valueIndex = i % poolLength;
                object entity = entities.ElementAt(i);
                object propertyValue = poolAsSpan[valueIndex];
                propertyOfEntity.SetValue(entity, propertyValue);
            }
        }
    }
}

