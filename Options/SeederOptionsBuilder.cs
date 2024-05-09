using EntityFrameworkCore.Seeding.History;
using EntityFrameworkCore.Seeding.Logging;
using Microsoft.Extensions.Logging;
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

    public ISeederOptionsBuilder AllowChatGPTUsage(string gptToken)
    {
        throw new NotImplementedException();
    }
    public ISeederOptionsBuilder HasDataVolumeIncreasingServices(Expression<Func<int, int>>? alterationFunction = null)
    {
        _options.CanIncreaseDataVolume = true;
        if (alterationFunction is not null)
        {
            _options.VolumeIncreasingFunction = alterationFunction.Compile();
            _options.HasVolumeIncreasingFunction = true;
        }
        return this;
    }

    public ISeederOptionsBuilder InitialBootup(bool flag = true)
    {
        _options.HasInitialBootup = flag;
        return this;
    }

    public ISeederOptionsBuilder OverrideExistingData(bool flag = true)
    {
        _options.OverrideExistingData = flag;
        return this;
    }

    public ISeederOptionsBuilder SaveDataAfterFinishing(bool flag = true)
    {
        _options.SaveDataAfterFinishing = flag;
        return this;
    }

    public ISeederOptionsBuilder UseLogger<TLogger>(SeederCommands commands)
        where TLogger : class, ILogger
    {
        throw new NotImplementedException();
    }
}