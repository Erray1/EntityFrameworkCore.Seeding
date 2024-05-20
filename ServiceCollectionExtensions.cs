using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EntityFrameworkCore.Seeding;

public static class SeederServiceCollectionExtensions
{
    public static IServiceCollection AddSeeder<TDbContext, TSeeder>(this IServiceCollection services, Action<ISeederOptionsBuilder>? optionsAction = null)
        where TDbContext : DbContext, new()
        where TSeeder : SeederModel<TDbContext>
    {
        if (!dbContextImplementsOnConfiguringMethod<TDbContext>())
        {
            throw new Exception($"{typeof(TDbContext).Name} must override OnConfiguring() method for seeder to work");
        }
        services.ConfigureSeederOptions<TDbContext, TSeeder>(optionsAction, out SeederOptions options);
        services.ConfigureSeederModel<TDbContext, TSeeder>(options.ArtificialModelConfiguring);

        services.AddInitialSeedingServices<TDbContext, TSeeder>();
        services.AddCoreSeederServices<TDbContext, TSeeder>();
        if (options.CanIncreaseDataVolume)
        {
            services.AddVolumeIncreasingServices(options.VolumeIncreasingFunction);
        }
            
        return services;
    }
    private static bool dbContextImplementsOnConfiguringMethod<TDbContext>()
    {
    #warning ДОПИСАТЬ
        return true;
        //var type = typeof(TDbContext);
        //type.GetMethod()
    }
}
