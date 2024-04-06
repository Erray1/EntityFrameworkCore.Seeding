using EntityFrameworkCore.Seeding.History;
using EntityFrameworkCore.Seeding.Logging;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Seeding.Options;

public class SeederOptionsBuilder : ISeederOptionsBuilder
{
    private readonly SeederOptions _options;
    public SeederOptionsBuilder(SeederOptions options)
    {
        _options = options;
    }
    public SeederOptions Build() => _options;
    public SeederOptionsBuilder HasDataVolumeIncreasingServices(Expression<Func<int, int>>? alterationFunction = null)
    {
        _options.CanIncreaseDataVolume = true;
        if (alterationFunction is not null)
        {
            _options.VolumeIncreasingFunction = alterationFunction.Compile();
            _options.HasVolumeIncreadingFunction = true;
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
        
    public SeederOptionsBuilder HasHistory(HistoryStoreTypes historyStoreType)
    {
        _options.HistoryStorageLocation = historyStoreType;
        return this;
    }

    SeederOptionsBuilder ISeederOptionsBuilder.UseLoggerFactory<TLoggerFactory>(SeederCommands commands)
    {
        throw new NotImplementedException();
    }

    SeederOptionsBuilder ISeederOptionsBuilder.UseLogger<TLogger>(SeederCommands commands)
    {
        throw new NotImplementedException();
    }
}