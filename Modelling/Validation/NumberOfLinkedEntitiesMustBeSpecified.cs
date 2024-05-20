using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling.Validation
{
    public class NumberOfLinkedEntitiesInManyToManyRelationshipsMustBeSpecified : SeederModelValidationRule
    {
        public NumberOfLinkedEntitiesInManyToManyRelationshipsMustBeSpecified(SeederModelValidationRule? next) : base(next)
        {
        }

        protected override IReadOnlyDictionary<SeederEntityInfo, List<string>> getValidationErrorsSummary(SeederModelInfo modelInfo)
        {
            var relationsWithErrors = modelInfo.ManyToManyRelations
                .Where(x => !x.IsNumberOfboundEntitiesSpecified);
            var data = relationsWithErrors
                .Select(x => new KeyValuePair<SeederEntityInfo, List<string>>
                (x.LeftEntityInfo, new List<string>() { $"Number of bound entities with {x.RightEntityInfo.EntityType.Name} is not specified" }))
                .ToDictionary();
            return data;
        }
    }
}
