using EntityFrameworkCore.Seeding.History;
using EntityFrameworkCore.Seeding.Logging;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Seeding.Options;

/// <summary>
/// Interface containing methods for seeder configuration
/// </summary>
public interface ISeederOptionsBuilder
{
    /// <summary>
    ///     Determines if IHostedService will be added to service collection for seeder service activation without a manual call.
    /// </summary>
    /// <param name="flag"></param>
    /// <returns>Options builder used for further configuration</returns>
    public ISeederOptionsBuilder InitialBootup(bool flag = true);
    /// <summary>
    ///     Determines if after each call on seeder service, volume of created data will increase or decrease
    /// </summary>
    /// <param name="alterationFunction">
    ///     The function that affects the change in the amount of data created
    /// </param>
    /// <returns>Options builder used for further configuration</returns>
    public ISeederOptionsBuilder HasDataVolumeIncreasingServices(Func<int, int>? alterationFunction = null);
    /// <summary>
    ///     Determines if all tables will be cleared before creating new data
    /// </summary>
    /// <param name="flag"></param>
    /// <returns>Options builder used for further configuration</returns>
    public ISeederOptionsBuilder OverrideExistingData(bool flag = true);
    /// <summary>
    ///     Determines if data will be saved after application shutdown
    /// </summary>
    /// <remarks>
    ///     In works
    /// </remarks>
    /// <param name="flag"></param>
    /// <returns>Options builder used for further configuration</returns>
    public ISeederOptionsBuilder SaveDataAfterFinishing(bool flag = true);
    /// <summary>
    ///     Adds the ability to use ChatGPT for data generation
    /// </summary>
    /// <remarks>In work. Do not use</remarks>
    /// <param name="gptToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ISeederOptionsBuilder AllowChatGPTUsage(string gptToken);
    /// <summary>
    /// Determines if options can be reconfigured in runtime
    /// </summary>
    /// <param name="flag"></param>
    /// <returns>Options builder used for further configuration</returns>
    public ISeederOptionsBuilder AllowOptionsReconfiguringInRuntime(bool flag = true);
}