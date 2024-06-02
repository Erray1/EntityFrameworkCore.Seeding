using EntityFrameworkCore.Seeding.Core;
using EntityFrameworkCore.Seeding.Core.Binding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Data;
using System.Reflection;

namespace EntityFrameworkCore.Seeding.Modelling.Utilities;

public static class SeederModelScaffolder
{
    public static SeederModelInfo CreateEmptyFromDbContext<TDbcontext>()
        where TDbcontext : DbContext, new()
    {
        var model = new SeederModelInfo();
        addEntitiesAndProperties<TDbcontext>(model);
        return model;
    }
    private static void addEntitiesAndProperties<TDbContext>(SeederModelInfo model)
        where TDbContext : DbContext, new()
    {
        var properties = typeof(TDbContext).GetProperties();
        var dbContextEntities = properties
            .Where(x => x.PropertyType.IsGenericType)
            .Select(x => new
            {
                EntityType = x.PropertyType.GetGenericArguments()[0],
                Properties = getProperties(x.PropertyType.GetGenericArguments()[0])
            })
            .ToList();

        var entities = dbContextEntities
            .Select(x => new SeederEntityInfo(x.EntityType)
            {
                Properties = x.Properties
                .Select(p => new SeederPropertyInfo(p.PropertyType, p.Name))
                .ToList()
            })
            .ToList();

        model.Entities = entities;
        configureEntitiesRelations<TDbContext>(model);
        removeRelationalPropertiesFromEntities(model);
    }

    private static void configureEntitiesRelations<TDbContext>(SeederModelInfo model)
        where TDbContext : DbContext, new()
    {
        var dbModel = new TDbContext().Model;
        var entityTypes = dbModel.GetEntityTypes();

        var allNavigations = entityTypes
            .SelectMany(x => x.GetNavigations())
            .ToList();

        var allManyToManyNavigations = entityTypes
            .SelectMany(x => x.GetSkipNavigations())
            .ToList();

        configureJoinTypes(allManyToManyNavigations, model);

        List<EntityRelation> relations = new();
        List<EntityManyToManyRelation> manyToManyRelations = new();
        model.Relations = relations;
        model.ManyToManyRelations = manyToManyRelations;
        List<string> handledManyToManyNavigationJoinTypeNames = new();
        foreach (var entityInfo in model.Entities)
        {
            var primaryKeys = entityTypes
                .First(e => e.ClrType == entityInfo.EntityType)
                .FindPrimaryKey();

            removePrimaryKeyPropertiesFromEntity(entityInfo, primaryKeys);
            

            var navigations = allNavigations
               .Where(x => x.DeclaringEntityType.Name == entityInfo.EntityType.FullName);
            var manyToManyNavigations = allManyToManyNavigations
                .Where(x => x.DeclaringEntityType.Name == entityInfo.EntityType.FullName);

            foreach (var navigation in navigations)
            {
                if (navigation.IsOnDependent) continue;
                Type dependentEntityType = navigation.TargetEntityType.ClrType;
                SeederEntityInfo dependentEntity = model.Entities.Single(x => x.EntityType == dependentEntityType);
                EntityRelation relation = new(entityInfo, dependentEntity, navigation);

                EntityRelationType relationType = getEntitiesRelationType(relation.PrincipalNavigationProperty, relation.DependentNavigationProperty);
                relation.IsOneToOne = relationType == EntityRelationType.OneToOne;

                relations.Add(relation);
            }
            
            foreach (var navigation in manyToManyNavigations)
            {
                var joinType = navigation.JoinEntityType;
                if (handledManyToManyNavigationJoinTypeNames.Contains(joinType.Name)) continue;
                Type dependentEntityType = navigation.TargetEntityType.ClrType;
                SeederEntityInfo dependentEntity = model.Entities
                    .Single(x => x.EntityType == dependentEntityType);

                SeederEntityInfo? joinEntity = model.Entities
                    .SingleOrDefault(x => x.EntityType == joinType.ClrType);

                EntityManyToManyRelation relation = new(entityInfo, dependentEntity, joinEntity, navigation);
                manyToManyRelations.Add(relation);

                handledManyToManyNavigationJoinTypeNames.Add(joinType.Name);
            }

        }
        Console.WriteLine();
    }
    private static void configureJoinTypes(IEnumerable<ISkipNavigation> skipNavigations, SeederModelInfo model)
    {
        var joinTypes = skipNavigations
            .Select(x => x.JoinEntityType)
            .Distinct()
            .ToList();

        foreach (var joinType in joinTypes)
        {
            SeederEntityInfo joinEntityInfo = model.Entities
                .Single(x => x.EntityType == joinType.ClrType);
            joinEntityInfo.IsJoinEntity = true;
        }
    }
    private static void removePrimaryKeyPropertiesFromEntity(SeederEntityInfo entityInfo, IKey? key)
    {
        PropertyInfo[] keyProperties = key.Properties
            .Select(x => x.PropertyInfo)
            .ToArray();

        if (entityInfo.IsJoinEntity)
        {
            foreach (var property in keyProperties)
            {
                PropertyActions.RemoveProperty(property, entityInfo);
            }
            return;
        }

        foreach (var property in keyProperties)
        {
            var seederPropertyInfo = entityInfo.Properties
                .Single(x => x.PropertyName == property.Name 
                    && x.PropertyType == property.PropertyType);
            seederPropertyInfo.DataCreationType = SeederDataCreationType.CreatedID;
            seederPropertyInfo.IsIdProperty = true;
            seederPropertyInfo.GenerateId = true;
            seederPropertyInfo.IsConfigured = true;
        }
    }
    private static void removeRelationalPropertiesFromEntities(SeederModelInfo model)
    {
        List<EntityRelation> removedRelations = new();
        foreach (var relation in model.Relations)
        {
            // If dependent entity is join entity
            if (model.ManyToManyRelations.Any(x => x.JoinEntityInfo == relation.DependentEntityInfo))
            {
                PropertyActions.RemoveProperty(relation.PrincipalNavigationProperty, relation.PrincipalEntityInfo);
                removedRelations.Add(relation);
                continue;
            }

            PropertyActions.RemoveProperty(relation.PrincipalNavigationProperty, relation.PrincipalEntityInfo);

            if (relation.DependentEntityHasNavigation)
            {
                PropertyActions.RemoveProperty(relation.DependentNavigationProperty, relation.DependentEntityInfo);
            }
            foreach (var foreignKeyProperty in relation.DependentForeignKeyProperties)
            {
                PropertyActions.RemoveProperty(foreignKeyProperty, relation.DependentEntityInfo);
            }
            
        }
        foreach (var relationToRemove in removedRelations) model.Relations.Remove(relationToRemove);


        foreach (var relation in model.ManyToManyRelations)
        {
            // left navigation collection 
            PropertyActions.RemoveProperty(relation.LeftNavigationProperty, relation.LeftEntityInfo);

            // right navigation collection
            PropertyActions.RemoveProperty(relation.RightNavigationProperty, relation.RightEntityInfo);

            // navigation property from join to left
            PropertyActions.RemoveProperty(relation.JoinEntityData.LeftNavigationFromJoinProperty, relation.JoinEntityInfo);

            // navigation property from join to right
            PropertyActions.RemoveProperty(relation.JoinEntityData.RightNavigationFromJoinProperty, relation.JoinEntityInfo);
        }

    }
    private static EntityRelationType getEntitiesRelationType(PropertyInfo principalProperty, PropertyInfo? dependentProperty)
    {
        bool isPrincipalCollection = isCollection(principalProperty.PropertyType);


        if (isPrincipalCollection) // 1 : N
        {
            return EntityRelationType.OneToMany;
        }
        if (!isPrincipalCollection) // 1 : 1
        {

            return EntityRelationType.OneToOne;

        }
        throw new NotSupportedException($"Unknow relation type between {principalProperty.DeclaringType!.Name} and {dependentProperty.DeclaringType!.Name}");
    }
    private static bool isCollection(Type type)
    {
        var interfaces = type.GetInterfaces();

        return interfaces.Any(x => x.Name.Contains("IEnumerable")) && type != typeof(string);
    }

    private static List<PropertyInfo> getProperties(Type entityType)
    {
        var properties = entityType
            .GetRuntimeProperties()
            .ToList();
        return properties;
    }
}

file static class PropertyActions
{
    public static void RemoveProperty(PropertyInfo? property, SeederEntityInfo entity)
    {
        if (property is null) return;
        var propToRemove = entity.Properties
            .SingleOrDefault(x => x.PropertyName == property.Name);
        entity.Properties.Remove(propToRemove);
    }
}
