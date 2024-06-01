using EntityFrameworkCore.Seeding.Core;
using EntityFrameworkCore.Seeding.Core.Binding;
using EntityFrameworkCore.Seeding.Core.Creation;
using EntityFrameworkCore.Seeding.Core.Creation.CreationPolicies;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.Modelling.Validation;
using EntityFrameworkCore.Seeding.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Seeding.DI;

public static partial class InternalSeederDIExtensions
{
    public static IServiceCollection ConfigureSeederOptions<TDbContext, TSeeder>(
        this IServiceCollection services,
        Action<ISeederOptionsBuilder>? optionsAction,
        out SeederOptions options)
        where TSeeder : SeederModel<TDbContext>
        where TDbContext : DbContext
    {
        var builder = new SeederOptionsBuilder(new SeederOptions());

        if (optionsAction is not null)
        {
            optionsAction.Invoke(builder);
        }

        options = builder.Build();

        services.AddKeyedSingleton(typeof(TDbContext).Name, new SeederOptionsProvider(options));

        return services;
    }

    public static IServiceCollection ConfigureSeederModel<TDbContext, TSeeder>(
        this IServiceCollection services)
        where TSeeder : SeederModel<TDbContext>
        where TDbContext : DbContext, new()
    {
        var seederType = typeof(TSeeder);

        var seederModel = (TSeeder)Activator.CreateInstance(seederType)!;
        var seederModelInfo = SeederModelScaffolder.CreateEmptyFromDbContext<TDbContext>();
        var builder = new SeederModelBuilder(seederModelInfo);
        seederModel.ConfigureModel(builder);

        SeederModelValidator.ThrowExceptionIfInvalid(seederModelInfo);

        services.AddKeyedSingleton(typeof(TDbContext).Name, new SeederModelProvider(seederModelInfo));

        return services;
    }
    public static IServiceCollection AddSeederLoggingServices<TDbContext, TSeeder>(this IServiceCollection services)
        where TSeeder : SeederModel<TDbContext>
        where TDbContext : DbContext
    {
        return services;
    }

    public static IServiceCollection AddInitialSeedingServices<TDbContext, TSeeder>(this IServiceCollection services)
        where TSeeder : SeederModel<TDbContext>
        where TDbContext : DbContext
    {
        services.AddHostedService<SeederInitialStarter<TSeeder, TDbContext>>();
        return services;
    }

    public static IServiceCollection AddVolumeIncreasingServices(this IServiceCollection services, Func<int, int>? volumeIncreasingFunction)
    {
        // Add history
        return services;
    }
    public static IServiceCollection AddCoreSeederServices<TDbContext, TSeeder>(this IServiceCollection services)
        where TSeeder : SeederModel<TDbContext>
        where TDbContext : DbContext

    {
        services.AddKeyedScoped<Seeder<TSeeder, TDbContext>>(typeof(TDbContext).Name);
        
        services.AddScoped<SeederEntityBinder<TDbContext>>();
        services.AddScoped<SeederEntityAdder<TDbContext>>();

        // Creation

        services.AddScoped<SeederEntityCreator<TDbContext>>();
        services.AddScoped<EntityCreationChainLinker>();
        services.AddScoped<SeederEntityCreationPolicyFactory>();

        var policies = typeof(SeederPropertiesCreationPolicy).Assembly.GetTypes()
            .Where(t => !t.IsAbstract
            && !t.IsInterface
            && t.IsSubclassOf(typeof(SeederPropertiesCreationPolicy)));
        //foreach (var policy in policies) services.AddScoped(policy);
        foreach (var policy in policies) services.AddTransient(policy);

        return services;
    }
}
