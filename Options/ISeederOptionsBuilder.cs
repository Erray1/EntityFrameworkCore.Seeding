using EFCoreSeeder.History;
using EFCoreSeeder.Logging;
using EFCoreSeeder.Reload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Options;

public interface ISeederOptionsBuilder
{
    public SeederOptionsBuilder HasRefreshingBehaviour(RefreshBehaviours behaviour, Expression<Func<int, int>>? seedingAlterationFunction = null);
    public SeederOptionsBuilder OverrideExistingData(bool flag = true);
    public SeederOptionsBuilder SaveDataAfterFinishing(bool flag = false);
    public SeederOptionsBuilder LogTo<TLogger>(SeederCommands commands = SeederCommands.Seeding);
    public SeederOptionsBuilder LogTo(LoggingTypes loggingType, SeederCommands commands = SeederCommands.Seeding);
    public SeederOptionsBuilder HasHistory(HistoryStoreTypes historyStoreType);
}