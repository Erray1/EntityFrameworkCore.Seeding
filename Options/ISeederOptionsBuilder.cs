using EntityFrameworkCore.Seeding.History;
using EntityFrameworkCore.Seeding.Logging;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Seeding.Options;

public interface ISeederOptionsBuilder
{
    public SeederOptionsBuilder HasDataVolumeIncreasingServices(Expression<Func<int, int>>? alterationFunction = null);
    public SeederOptionsBuilder OverrideExistingData(bool flag = true);
    public SeederOptionsBuilder SaveDataAfterFinishing(bool flag = true);
    public SeederOptionsBuilder UseLoggerFactory<TLoggerFactory>(SeederCommands commands = SeederCommands.Seeding)
        where TLoggerFactory : class, ILoggerFactory;
    public SeederOptionsBuilder UseLogger<TLogger>(SeederCommands commands = SeederCommands.Seeding)
        where TLogger : class, ILogger;
}