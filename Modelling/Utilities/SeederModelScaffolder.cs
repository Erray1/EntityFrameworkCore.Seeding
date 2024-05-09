using EntityFrameworkCore.Seeding.Core;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;

namespace EntityFrameworkCore.Seeding.Modelling.Utilities;

public static class SeederModelScaffolder
{
    public static SeederModelInfo CreateEmptyFromDbContext<TDbcontext>()
        where TDbcontext : DbContext
    {
        var model = new SeederModelInfo();
        addEntitiesAndProperties<TDbcontext>(model);
        return model;
    }
    private static void addEntitiesAndProperties<TDbContext>(SeederModelInfo model)
        where TDbContext : DbContext
    {
        var properties = typeof(TDbContext).GetProperties();
        var dbContextEntities = properties
            .Where(x => x.PropertyType.IsGenericType)
            .Select(x => new DbContextEntityInfo
            {
                EntityType = x.PropertyType.GetGenericArguments()[0],
                Properties = getProperties(x.PropertyType.GetGenericArguments()[0])
            })
            .ToList();

        var entities = dbContextEntities
            .Select(x => new SeederEntityInfo(x.EntityType)
            {
                Properties = x.Properties
                .Select(p => new SeederPropertyInfo(p.PropertyType) { PropertyName = p.Name})
                .ToList(),
            })
            .ToList();

        configureEntities<TDbContext>(entities);
        model.Entities = entities;
    }

    private static void configureEntities<TDbContext>(List<SeederEntityInfo> entities)
        where TDbContext : DbContext
    {
        var allEntityTypes = entities.Select(x => x.EntityType);
        var linkedEntitiesAndRelationTypes = new Dictionary<SeederEntityInfo, EntityRelationType>();
        foreach (var entity in entities)
        {
            var linkedEntities_ = entity.EntityType.GetProperties()
                .Where(x => allEntityTypes.Contains(x.PropertyType) ||
                x.PropertyType.IsGenericType && allEntityTypes.Contains(x.PropertyType.GetGenericArguments()[0]));

            var linkedEntities = linkedEntities_
                .Select(x => entities.Single(e => e.EntityType == x.PropertyType ||
                x.PropertyType.IsGenericType && e.EntityType == x.PropertyType.GetGenericArguments()[0]))
                .ToList();

            linkedEntities.ForEach(x => linkedEntitiesAndRelationTypes.Add(x, getEntityRelation(entity, x)));
            entity.LinkedEntities = new(linkedEntitiesAndRelationTypes);
            linkedEntitiesAndRelationTypes.Clear();
        }
    }

    private static EntityRelationType getEntityRelation(SeederEntityInfo fromEntity, SeederEntityInfo toEntity)
    {
        Type toEntityType = toEntity.EntityType;
        var linkProperty = fromEntity.EntityType.GetProperties()
            .Single(x => x.PropertyType == toEntityType ||
            x.PropertyType.IsGenericType && x.PropertyType.GetGenericArguments()[0] == toEntityType);
        if (linkProperty.PropertyType.IsGenericType)
        {
            return EntityRelationType.OneToMany;
        }
        var isPropertyNullable = new NullabilityInfoContext().Create(linkProperty).WriteState is NullabilityState.Nullable;
        if (isPropertyNullable)
        {
            fromEntity.NullableLinkedEntitiesProbabilities.Add(toEntity, 1);
            return EntityRelationType.OneToOneNullable;
        }
        return EntityRelationType.OneToOne;
    }

    private static List<PropertyInfo> getProperties(Type entityType)
    {
        var properties = entityType
            .GetRuntimeProperties()
            .ToList();
        return properties;
    }

    class DbContextEntityInfo
    {
        public Type EntityType { get; init; }
        public List<PropertyInfo> Properties { get; init; }
    }
}
