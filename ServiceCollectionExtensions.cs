using EFCoreSeeder.DI;
using EFCoreSeeder.Modelling;
using EFCoreSeeder.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EFCoreSeeder;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection HasSeeder<TDbContext, TSeeder>(this IServiceCollection services, Action<ISeederOptionsBuilder>? optionsAction = null)
        where TDbContext : DbContext
        where TSeeder : SeederModel<TDbContext>
    {
        services.ConfigureSeederOptions<TDbContext, TSeeder>(optionsAction)
            .ConfigureSeederModel<TDbContext, TSeeder>()
            .AddSeederLoggingServices()
            .AddInitialSeedingServices()
            .AddRefreshingServices();

        return services;
    }
}
