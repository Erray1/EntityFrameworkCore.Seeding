using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Options.Validation;
internal class RefreshBehaviourMustHaveHistoryStorage : ISeederOptionsRule
{
    public void ValidateAndThrowException(SeederOptions options)
    {
        if (options.HasRefreshFunction && options.HistoryStorageLocation == History.HistoryStoreTypes.NoHistory)
        {
            throw new Exception("Seeder with refresh behaviour must have storage location configurated in its options");
        }
    }
}
