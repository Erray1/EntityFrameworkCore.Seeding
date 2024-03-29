using EFCoreSeeder.Modelling;
using EFCoreSeeder.Options;
using EFCoreSeeder.Refreshing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.DI;

public static partial class InternalSeederDIExtensions
{
    public static IServiceCollection ConfigureSeederOptions<TDbContext, TSeeder>(this IServiceCollection services, Action<ISeederOptionsBuilder>? optionsAction)
        where TSeeder : SeederModel<TDbContext>
        where TDbContext : DbContext
    {
        var seederType = typeof(TSeeder);
        var builder = new SeederOptionsBuilder(new SeederOptions());

        if (optionsAction is not null)
        {
            optionsAction.Invoke(builder);
        }

        var options = builder.Build();

        services.AddKeyedSingleton(new SeederOptionsProvider(options), seederType.Name);

        return services;
    }

    public static IServiceCollection ConfigureSeederModel<TDbContext, TSeeder>(this IServiceCollection services)
        where TSeeder : SeederModel<TDbContext>
        where TDbContext : DbContext
    {
        var seederType = typeof(TSeeder);
        // REMAKE


        var seederModel = ((SeederModel<TDbContext>)(Activator.CreateInstance(seederType)!));
        var builder = new SeederModelBuilder<TDbContext>(new SeederModelInfo());
        seederModel.CreateModel(builder);

        services.AddKeyedSingleton(new SeederModelProvider((SeederModelInfo)builder.Finalize()), seederType.Name);

        return services;
    }
    public static IServiceCollection AddSeederLoggingServices(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddInitialSeedingServices(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddRefreshingServices(this IServiceCollection services)
    {
        services.AddScoped<ISeederRefreshingService, SeederRefreshingService>();
        // Add history
        return services;
    }
}
