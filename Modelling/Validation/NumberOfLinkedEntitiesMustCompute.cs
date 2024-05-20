using EntityFrameworkCore.Seeding.Core.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
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
        var wrongOneToOneRelations = modelInfo.Relations
            .Where(x => x.IsOneToOne)
            .Where(x => !isRelationPossibleOneToOne(x))
            .Select(x => new KeyValuePair<SeederEntityInfo, string>
            (x.PrincipalEntityInfo, $"Relation with {x.DependentEntityInfo.EntityType.Name} is impossible with current configuration of number of created entities and bind probability"));

        var wrongOneToManyRelations = modelInfo.Relations
            .Where(x => !x.IsOneToOne)
            .Where(x => !isRelationPossibleOneToMany(x))
            .Select(x => new KeyValuePair<SeederEntityInfo, string>
            (x.PrincipalEntityInfo, $"Relation with {x.DependentEntityInfo.EntityType.Name} is impossible with current configuration of number of created entities and bind probability"));

        var wrongManyToManyRelations = modelInfo.ManyToManyRelations
            .Where(x => !isRelationPossibleManyToMany(x))
            .Select(x => new KeyValuePair<SeederEntityInfo, string>
            (x.LeftEntityInfo, $"Relation with {x.RightEntityInfo.EntityType.Name} is impossible with current configuration of number of created entities and bind probability"));

        var errors = wrongOneToOneRelations
            .Concat(wrongOneToManyRelations)
            .Concat(wrongManyToManyRelations);
        var result = new Dictionary<SeederEntityInfo, List<string>>();

        foreach (var error in errors)
        {
            if (result.ContainsKey(error.Key))
            {
                result[error.Key].Add(error.Value);
            }
            else
            {
                result[error.Key] = new List<string>() { error.Value };
            }
        }
        return result;
    }
    private bool isRelationPossibleOneToOne(EntityRelation relation)
    {
        return relation.DependentEntityInfo.TimesCreated * relation.BindProbability <= relation.PrincipalEntityInfo.TimesCreated;
    }
    private bool isRelationPossibleOneToMany(EntityRelation relation)
    {
        return relation.DependentEntityInfo.TimesCreated * relation.BindProbability >= relation.PrincipalEntityInfo.TimesCreated;
    }
    private bool isRelationPossibleManyToMany(EntityManyToManyRelation relation)
    {
    #warning Не доделано
        return true;
    }
}

