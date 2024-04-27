using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;

namespace EntityFrameworkCore.Seeding.Core;
public class SeederEntityAdder<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TDbContext _dbContext;
    private readonly SeederOptions _options;
    private readonly SeederModelInfo _model;
    public SeederEntityAdder(
        IServiceProvider serviceProvider,
        IKeyedServiceProvider keyedServiceProvider)
    {
        _serviceProvider = serviceProvider;
        _dbContext = serviceProvider.GetRequiredService<TDbContext>()!;
        _options = keyedServiceProvider.GetKeyedService<SeederOptionsProvider>(typeof(TDbContext).Name)!.GetOptions();
    }
    public async Task AddEntities(Dictionary<SeederEntityInfo, IEnumerable<object>> entities)
    {
        entities = entities.OrderBy(x => !x.Key.IsPrincipal).ToDictionary();
        foreach (var entity in entities)
        {
            var (addMethod, dbSet) = SeederCoreUtilities.GetMethodAndDbSet(_dbContext, entity.Key.EntityType, "AddRange");
            if (_options.OverrideExistingData)
            {
                var clearMethod = SeederCoreUtilities.GetDbSetMethod("Clear", dbSet);
                clearMethod.Invoke(dbSet, []);
            }
            addMethod.Invoke(dbSet, [entity.Value]);
        }
    }
}

