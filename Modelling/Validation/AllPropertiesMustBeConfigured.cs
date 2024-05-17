using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling.Validation;
public class AllPropertiesMustBeConfigured : SeederModelValidationRule
{
    public AllPropertiesMustBeConfigured(SeederModelValidationRule? next) : base(next) { }

    protected override IReadOnlyDictionary<SeederEntityInfo, List<string>> getValidationErrorsSummary(SeederModelInfo modelInfo)
    {
        var nonConfiguredProperties = modelInfo.Entities
            .Where(x => x.Properties.Any(p => !p.IsConfigured))
            .Select(x => new KeyValuePair<SeederEntityInfo, IEnumerable<SeederPropertyInfo>> 
            (x, x.Properties
                .Where(p => !p.IsConfigured)));

        return nonConfiguredProperties
            .Select(x => new KeyValuePair<SeederEntityInfo, List<string>>(
                x.Key, getNonConfiguredPropertiesErrorMessage(x.Value)))
            .ToDictionary();
            
    }
    private List<string> getNonConfiguredPropertiesErrorMessage(IEnumerable<SeederPropertyInfo> properties)
    {
        return properties
            .Select(x => $"Property {x.PropertyName} is not configured")
            .ToList();
    }
}

