using EntityFrameworkCore.Seeding.Modelling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Creation.CreationPolicies
{
    public class EntityHasGeneratedIDPolicy : SeederPropertiesCreationPolicy
    {
        protected override void createPropertiesPool()
        {
            var propertyInfo = _propertiesFilledWithPolicy.First();
            var propertyType = propertyInfo.PropertyType;
            if (propertyType == typeof(int))
            {
                var values = Enumerable
                    .Range(0, _entityInfo.TimesCreated)
                    .Cast<object>()
                    .ToList();

                _propertyPool = new Dictionary<SeederPropertyInfo, List<object>>()
                {{ propertyInfo,  values} };
            }
            if (propertyType == typeof(Guid) || propertyType == typeof(string) )
            {
                var values = RandomValuesGenerator.GetRandomValuesOfType(typeof(Guid), _entityInfo.TimesCreated);
                if (propertyType == typeof(string))
                {
                    values = values
                        .Select(x => x.ToString())
                        .Cast<object>()
                        .ToList();
                }
                _propertyPool = new Dictionary<SeederPropertyInfo, List<object>>()
                {{ propertyInfo,  values} };
            }

        }
    }
}
