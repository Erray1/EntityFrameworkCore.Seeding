
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.Seeding.Modelling.Validation;

public static class SeederModelValidator
{
    public static void ThrowExceptionIfInvalid(SeederModelInfo model)
    {
        var context = new Dictionary<SeederEntityInfo, List<string>>();
        var chain = SeederValidationRuleChainLinker.CreateChain();
        chain.ThrowExceptionIfInvalid(model, context);
    }
}