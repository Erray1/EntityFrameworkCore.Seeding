using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Seeding;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection HasSeeder<TDbContext, TSeeder>(this IServiceCollection services, Action<ISeederOptionsBuilder>? optionsAction = null)
        where TDbContext : DbContext
        where TSeeder : SeederModel<TDbContext>
    {
        services.ConfigureSeederOptions<TDbContext, TSeeder>(optionsAction, out SeederOptions options)
            .ConfigureSeederModel<TDbContext, TSeeder>(options.ConfigureArtificially);

        if (options.HasLogger)
        {
            services.AddSeederLoggingServices();
        }

        services.AddInitialSeedingServices();
        if (options.CanIncreaseDataVolume)
        {
            services.AddVolumeIncreasingServices(options.VolumeIncreasingFunction);
        }
            
        return services;
    }
}
