using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using System.Reflection;

namespace EntityFrameworkCore.Seeding.Core;
public class SeederEntityAdder<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SeederOptions _options;
    private readonly SeederModelInfo _model;
    private readonly TDbContext _dbContext;
    public SeederEntityAdder(
        IServiceProvider serviceProvider,
        TDbContext dbContext
        )
    {
        _serviceProvider = serviceProvider;
        _options = serviceProvider.GetKeyedService<SeederOptionsProvider>(typeof(TDbContext).Name)!.GetOptions();
        _dbContext = dbContext;
    }
    public async Task AddEntities(Dictionary<SeederEntityInfo, List<object>> entities)
    {
        var dbSets = typeof(TDbContext)
            .GetProperties()
            .Where(x => x.PropertyType.IsGenericType)
            .Select(x => new DbSetInfo
            {
                DbSet = x.GetGetMethod()!.Invoke(_dbContext, [])!,
                DbSetProperty = x,
                AddMethod = x.PropertyType.GetMethod("Add")!
            })
            .ToList();
        var nonJoinEntities = entities
            .Where(x => !x.Key.IsJoinEntity)
            .ToList();

        foreach (var (entity, pool) in nonJoinEntities)
        {
            var dbSet = dbSets.Single(x => x.DbSetProperty.PropertyType.GetGenericArguments()[0] == entity.EntityType);
            if (_options.OverrideExistingData)
            {
                clearDbSet(dbSet);
            }
            foreach (var entityObject in pool)
            {
                dbSet.AddMethod.Invoke(dbSet.DbSet, [entityObject]);
            }
        }
        await _dbContext.SaveChangesAsync();
    }
    private void clearDbSet(DbSetInfo dbSet)
    {
        MethodInfo removeRangeMethod = dbSet.DbSetProperty.PropertyType
            .GetMethods()
            .Where(x => x.Name == "RemoveRange")
            .ElementAt(1);
        removeRangeMethod.Invoke(dbSet.DbSet, [dbSet.DbSet]);
    }
    class DbSetInfo
    {
        public object DbSet { get; set; }
        public PropertyInfo DbSetProperty { get; set; }
        public MethodInfo AddMethod { get; set; }
    }
}

