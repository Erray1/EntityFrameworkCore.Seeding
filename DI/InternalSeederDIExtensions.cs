using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Seeding.DI;

public static partial class InternalSeederDIExtensions
{
    public static IServiceCollection ConfigureSeederOptions<TDbContext, TSeeder>(this IServiceCollection services, Action<ISeederOptionsBuilder>? optionsAction, out SeederOptions options)
        where TSeeder : SeederModel<TDbContext>
        where TDbContext : DbContext
    {
        var seederType = typeof(TSeeder);
        var builder = new SeederOptionsBuilder(new SeederOptions());

        if (optionsAction is not null)
        {
            optionsAction.Invoke(builder);
        }

        options = builder.Build();

        services.AddKeyedSingleton(new SeederOptionsProvider(options), seederType.Name);

        return services;
    }

    public static IServiceCollection ConfigureSeederModel<TDbContext, TSeeder>(this IServiceCollection services, bool isConfigurerArtificial)
        where TSeeder : SeederModel<TDbContext>
        where TDbContext : DbContext
    {
        var seederType = typeof(TSeeder);


        var seederModel = (TSeeder)Activator.CreateInstance(seederType)!;
        var builder = new SeederModelBuilder<TDbContext>(SeederModelScaffolder.CreateEmptyFromDbContext<TDbContext>());
        seederModel.CreateModel(builder);

        services.AddKeyedSingleton(new SeederModelProvider((SeederModelInfo)builder.Build()), seederType.Name);

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

    public static IServiceCollection AddVolumeIncreasingServices(this IServiceCollection services, Func<int, int>? volumeIncreasingFunction)
    {
        // Add history
        return services;
    }
}
