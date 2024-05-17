using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling.Validation;
public class ProbabilityOfNullableRelationshipsMustBeSpecified : SeederModelValidationRule
{
    public ProbabilityOfNullableRelationshipsMustBeSpecified(SeederModelValidationRule? next) : base(next)
    {
    }

    protected override IReadOnlyDictionary<SeederEntityInfo, List<string>> getValidationErrorsSummary(SeederModelInfo modelInfo)
    {
        return modelInfo.Entities
            .Where(x => x.NullableLinkedEntitiesProbabilities
                .Any(r => r.Value is null))
            .Select(x => new KeyValuePair<SeederEntityInfo, List<string>>(
                x, x.NullableLinkedEntitiesProbabilities
                    .Where(r => r.Value is null)
                    .Select(r => $"Probability of nullable relationship with entity {r.Key.EntityType.Name} is not specified")
                    .ToList()
                ))
            .ToDictionary();
    }
}

