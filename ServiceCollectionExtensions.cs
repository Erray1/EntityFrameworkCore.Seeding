using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EntityFrameworkCore.Seeding;

public static class SeederServiceCollectionExtensions
{
    /// <summary>
    ///     Adds seeding services to service collection
    /// </summary>
    /// <typeparam name="TDbContext">Type of DbContext used for adding data and scaffolding DB model</typeparam>
    /// <typeparam name="TSeeder">Type of SeederModel used for modelling seeded data and relations</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="optionsAction">Additional configuration of seeder</param>
    /// <returns>IServiceCollection</returns>
    /// <exception cref="DbContextDoesNotOverrideOnConfiguringMethod{TDbContext}">Throws exception if DbContext does not override OnConfiguring() method</exception>
    /// <exception cref="InvalidDataException"></exception>
    public static IServiceCollection AddSeeder<TDbContext, TSeederModel>(this IServiceCollection services, Action<ISeederOptionsBuilder>? optionsAction = null)
        where TDbContext : DbContext, new()
        where TSeederModel : SeederModel<TDbContext>
    {
        if (!dbContextOverridesOnConfiguringMethod<TDbContext>())
        {
            throw new DbContextDoesNotOverrideOnConfiguringMethod<TDbContext>();
        }

        if (typeof(TSeederModel).IsAbstract)
        {
            string mainMethodName = typeof(TSeederModel).GetMethods().First().Name;
            throw new InvalidDataException($"For TSeederModel type parameter provide custom type that inherits SeederModel and overrides {mainMethodName}");
        }
        services.ConfigureSeederOptions<TDbContext, TSeederModel>(optionsAction, out SeederOptions options);
        services.ConfigureSeederModel<TDbContext, TSeederModel>(options.ArtificialModelConfiguring);
        if (options.HasInitialBootup)
        {
            services.AddInitialSeedingServices<TDbContext, TSeederModel>();
        }
        services.AddCoreSeederServices<TDbContext, TSeederModel>();
        if (options.CanIncreaseDataVolume)
        {
            services.AddVolumeIncreasingServices(options.VolumeIncreasingFunction);
        }
            
        return services;
    }
    private static bool dbContextOverridesOnConfiguringMethod<TDbContext>()
    {
        return typeof(TDbContext).GetMethod("OnConfiguring")!.DeclaringType == typeof(TDbContext);
    }
}

public class DbContextDoesNotOverrideOnConfiguringMethod<TDbContext> : Exception
    where TDbContext : DbContext
{
    public DbContextDoesNotOverrideOnConfiguringMethod() : base($"{typeof(TDbContext).Name} must override OnConfiguring() method")
    {
        
    }
}
