using EntityFrameworkCore.Seeding.Core.Creation;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Modelling.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.CreationPolicies;
public class EntityCreatedRandomlyPolicy : SeederEntityCreationPolicy
{
    protected override void createPropertiesPool()
    {
        var current = _propertiesEnumerator.Current;
        var currentType = current.PropertyType;

        _propertyPool = _propertiesFilledWithPolicy
            .Select(x => new KeyValuePair<SeederPropertyInfo, IEnumerable<object>>
            (x, RandomValuesGenerator.GetRandomValuesOfType(x.PropertyType, _entityInfo.TimesCreated + _entityInfo.Locality)))
            .ToImmutableDictionary(); 
    }
}

