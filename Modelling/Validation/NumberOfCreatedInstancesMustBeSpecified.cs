using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling.Validation
{
    public class NumberOfCreatedInstancesMustBeSpecified : SeederModelValidationRule
    {
        public NumberOfCreatedInstancesMustBeSpecified(SeederModelValidationRule? next) : base(next)
        {
        }

        protected override IReadOnlyDictionary<SeederEntityInfo, List<string>> getValidationErrorsSummary(SeederModelInfo modelInfo)
        {
            return modelInfo.Entities
                .Where(x => x.TimesCreated == 0 && x.DoCreate)
                .Select(x => new KeyValuePair<SeederEntityInfo, List<string>>
                (x, new List<string>()
                { $"Number of created instances must be specified. Otherwise, use DoNotCreate() method to exclude entity" }))
                .ToDictionary();
        }
    }
}
