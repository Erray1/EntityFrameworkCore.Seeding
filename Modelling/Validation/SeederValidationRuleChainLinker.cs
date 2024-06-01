using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling.Validation;
public static class SeederValidationRuleChainLinker
{
    public static SeederModelValidationRule CreateChain()
    {
        var rules = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(x => x.GetTypes())
        .Where(x => typeof(SeederModelValidationRule).IsAssignableFrom(x) && !x.IsAbstract)
        .ToList();

        SeederModelValidationRule? prev = null;

        for (int i = rules.Count - 1; i >= 0; i--)
        {
            Type ruletype = rules[i];
            ConstructorInfo ctor = ruletype.GetConstructor([typeof(SeederModelValidationRule)])!;
            var rule = (SeederModelValidationRule)ctor.Invoke([prev]);
            prev = rule;
        }
        return prev!;
    }
}

