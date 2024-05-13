using EntityFrameworkCore.Seeding.Core.Creation;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Modelling.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Creation.CreationPolicies;
public class PropertiesCreatedRandomlyPolicy : SeederPropertiesCreationPolicy
{
    protected override void createPropertiesPool()
    {
        _propertyPool = _propertiesFilledWithPolicy
            .Select(x => new KeyValuePair<SeederPropertyInfo, List<object>>
            (x, RandomValuesGenerator.GetRandomValuesOfType(x.PropertyType, _entityInfo.TimesCreated)))
            .ToImmutableDictionary();
    }
}

