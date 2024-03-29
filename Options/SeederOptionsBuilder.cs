using EFCoreSeeder.History;
using EFCoreSeeder.Logging;
using EFCoreSeeder.Modelling;
using EFCoreSeeder.Reload;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace EFCoreSeeder.Options;

public class SeederOptionsBuilder : ISeederOptionsBuilder
{
    private readonly SeederOptions _options;
    public SeederOptionsBuilder(SeederOptions options)
    {
        _options = options;
    }
    public SeederOptions Build() => _options;
    public SeederOptionsBuilder HasRefreshingBehaviour(RefreshBehaviours behaviour, Expression<Func<int, int>>? seedingAlterationFunction = null)
    {
        _options.RefreshBehaviour = behaviour;
        if (behaviour != RefreshBehaviours.NoData) _options.CanRefreshData = true;
        if (seedingAlterationFunction is not null)
        {
            _options.SeedingAlterationFunction = seedingAlterationFunction.Compile();
            _options.HasRefreshFunction = true;
        }

        return this;
    }

    public SeederOptionsBuilder OverrideExistingData(bool flag = true)
    {
        _options.OverrideExistingData = flag;
        return this;
    }
    public SeederOptionsBuilder SaveDataAfterFinishing(bool flag = true)
    {
        _options.SaveDataAfterFinishing = flag;
        return this;
    }

    public SeederOptionsBuilder LogTo<TLogger>(SeederCommands commands = SeederCommands.Seeding)
    {
        _options.CommandsLogged = commands;
        _options.LoggerType = typeof(TLogger);
        return this;
    }
    public SeederOptionsBuilder LogTo(LoggingTypes loggingType, SeederCommands commands = SeederCommands.Seeding)
    {
        _options.CommandsLogged = commands;
        switch (loggingType)
        {
            case LoggingTypes.ToConsole:
                _options.LoggerType = typeof(ConsoleLogger);
                break;
            case LoggingTypes.NoLog:
                _options.LoggerType = null;
                break;
        }
        return this;
        
    }
    public SeederOptionsBuilder HasHistory(HistoryStoreTypes historyStoreType)
    {
        _options.HistoryStorageLocation = historyStoreType;
        return this;
    }
    
}