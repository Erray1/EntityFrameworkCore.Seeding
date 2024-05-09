using EntityFrameworkCore.Seeding.Modelling;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.CreationPolicies;
public class EntityCreatedFromGivenPoolPolicy : SeederEntityCreationPolicy
{
    protected override void createPropertiesPool()
    {
        _propertyPool = _propertiesFilledWithPolicy
            .Select(x => new KeyValuePair<SeederPropertyInfo, IEnumerable<object>>(x, x.PossibleValuesPool!))
            .ToImmutableDictionary();
    }
}

