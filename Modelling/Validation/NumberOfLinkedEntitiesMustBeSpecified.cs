using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling.Validation
{
    public class NumberOfLinkedEntitiesInOneToManyRelationshipsMustBeSpecified : SeederModelValidationRule
    {
        public NumberOfLinkedEntitiesInOneToManyRelationshipsMustBeSpecified(SeederModelValidationRule? next) : base(next)
        {
        }

        protected override IReadOnlyDictionary<SeederEntityInfo, List<string>> getValidationErrorsSummary(SeederModelInfo modelInfo)
        {
            return modelInfo.Entities
                .Where(x => x.NumberOfBoundEntitiesInOneToManyRelationships.Any(r => r.Value == 0))
                .Select(x => new KeyValuePair<SeederEntityInfo, List<string>>
                (x, x.NumberOfBoundEntitiesInOneToManyRelationships
                    .Where(r => r.Value == 0)
                    .Select(r => $"Number of bound instances of {r.Key.EntityType.Name} must be specified")
                    .ToList()))
                .ToDictionary();
        }
    }
}
