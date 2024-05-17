using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling.Validation;
public class NumberOfLinkedEntitiesMustCompute : SeederModelValidationRule
{
    public NumberOfLinkedEntitiesMustCompute(SeederModelValidationRule? next) : base(next)
    {
    }

    protected override IReadOnlyDictionary<SeederEntityInfo, List<string>> getValidationErrorsSummary(SeederModelInfo modelInfo)
    {
        var entitiesWithErrors = modelInfo.Entities
            .Select(x => new
            {
                Entity = x,
                Links = x.NumberOfBoundEntitiesInOneToManyRelationships
                    .Where(r => r.Value != -1 &&
                    !isNumberOfBoundEntitiesPossible(
                        x.TimesCreated,
                        r.Value,
                        r.Key.TimesCreated,
                        x.NullableLinkedEntitiesProbabilities.TryGetValue(r.Key, out double? value) ? value!.Value : 1))
            });

            return entitiesWithErrors.Select(x => new KeyValuePair<SeederEntityInfo, List<string>>
                (x.Entity, x.Links.Select(l =>
                {
                    x.Entity.NullableLinkedEntitiesProbabilities.TryGetValue(l.Key, out double? linkProbability);
                    (int bestNumberOfLinks, int bestNumberOfCreatedDependentEntities) =
                    getAdvice(x.Entity.TimesCreated, l.Value, l.Key.TimesCreated, linkProbability is not null ? linkProbability.Value : 1);

                    return $"Current number of links between {x.Entity.EntityType.Name} and {l.Key.EntityType.Name} is impossible \n " +
                    $"make number of links {bestNumberOfLinks} or set times created of {l.Key.EntityType.Name} as {bestNumberOfCreatedDependentEntities}";
                })
                .ToList()))
            .ToDictionary();
            
            
            
    }
    private bool isNumberOfBoundEntitiesPossible(int timesCreatedPrincipal, int numberOfLinks, int timesCreatedDpendent, double linkProbability = 1)
    {
        throw new NotImplementedException();
    }
    private (int bestNumberOfLinks, int bestNumberOfCreatedDependentEntities) getAdvice(int timesCreatedPrincipal, int numberOfLinks, int timesCreatedDpendent, double linkProbability = 1)
    {
        throw new NotImplementedException();
    }
}

