using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Options.Validation;

public class SeederOptionsValidator
{
    private readonly List<ISeederOptionsRule> Rules;
    public SeederOptionsValidator()
    {
        var ruleType = typeof(ISeederOptionsRule);
        Rules = ruleType.Assembly.ExportedTypes
            .Where(t => ruleType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => Activator.CreateInstance(t))
            .Cast<ISeederOptionsRule>()
            .ToList();
    }

    public void Validate(SeederOptions options)
    {
        foreach (var rule in Rules) { rule.ValidateAndThrowException(options); }
    }
}
