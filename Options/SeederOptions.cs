using EFCoreSeeder.History;
using EFCoreSeeder.Logging;
using EFCoreSeeder.Modelling;
using EFCoreSeeder.Reload;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Options;

public class SeederOptions
{
    public bool OverrideExistingData { get; set; } = true;
    public bool SaveDataAfterFinishing { get; set; } = true;
    public bool CanRefreshData { get; set; } = false;
    public RefreshBehaviours? RefreshBehaviour { get; set; }
    public bool HasRefreshFunction { get; set; } = false;
    public Func<int, int>? SeedingAlterationFunction { get; set; }
    public HistoryStoreTypes HistoryStorageLocation { get; set; } = HistoryStoreTypes.NoHistory;
    public SeederCommands CommandsLogged { get; set; } = SeederCommands.Seeding;
    public Type? LoggerType { get; set; }

}
