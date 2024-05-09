using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Seeding;

public static class SeederServiceCollectionExtensions
{
    public static IServiceCollection AddSeeder<TDbContext, TSeeder>(this IServiceCollection services, Action<ISeederOptionsBuilder>? optionsAction = null)
        where TDbContext : DbContext
        where TSeeder : SeederModel<TDbContext>
    {
        services.ConfigureSeederOptions<TDbContext, TSeeder>(optionsAction, out SeederOptions options);
        services.ConfigureSeederModel<TDbContext, TSeeder>(options.ArtificialModelConfiguring);

        if (options.HasLogger)
        {
            services.AddSeederLoggingServices<TDbContext, TSeeder>();
        }

        services.AddInitialSeedingServices<TDbContext, TSeeder>();
        services.AddCoreSeederServices<TDbContext, TSeeder>();
        if (options.CanIncreaseDataVolume)
        {
            services.AddVolumeIncreasingServices(options.VolumeIncreasingFunction);
        }
            
        return services;
    }
}
