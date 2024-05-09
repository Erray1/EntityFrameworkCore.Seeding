using EntityFrameworkCore.Seeding.History;
using EntityFrameworkCore.Seeding.Logging;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Seeding.Options;

public interface ISeederOptionsBuilder
{
    public ISeederOptionsBuilder InitialBootup(bool flag = true);
    public ISeederOptionsBuilder HasDataVolumeIncreasingServices(Expression<Func<int, int>>? alterationFunction = null);
    public ISeederOptionsBuilder OverrideExistingData(bool flag = true);
    public ISeederOptionsBuilder SaveDataAfterFinishing(bool flag = true);
    public ISeederOptionsBuilder UseLogger<TLogger>(SeederCommands commands = SeederCommands.Seeding)
        where TLogger : class, ILogger;
    public ISeederOptionsBuilder AllowChatGPTUsage(string gptToken);
}