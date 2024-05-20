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
        List<EntityRelation> relations = new();
        List<EntityManyToManyRelation> manyToManyRelations = new();
        model.Relations = relations;
        model.ManyToManyRelations = manyToManyRelations;
        List<string> handledManyToManyNavigationJoinTypeNames = new();
        foreach (var entityInfo in model.Entities)
       {
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
                SeederEntityInfo dependentEntity = model.Entities.Single(x => x.EntityType == dependentEntityType);
                EntityManyToManyRelation relation = new(entityInfo, dependentEntity, navigation);
                manyToManyRelations.Add(relation);

                handledManyToManyNavigationJoinTypeNames.Add(joinType.Name);
            }

        }
    }
    private static void removeRelationalPropertiesFromEntities(SeederModelInfo model)
    {
        foreach (var relation in model.Relations)
        {
            var principalNavigationProperty = relation.PrincipalNavigationProperty;
            var principalPropertyToRemove = relation.PrincipalEntityInfo.Properties
                .Single(x => x.PropertyName == principalNavigationProperty.Name
                && x.PropertyType == principalNavigationProperty.PropertyType);
            relation.PrincipalEntityInfo.Properties.Remove(principalPropertyToRemove);

            # warning убирает навигацию, но не внешний ключ
            if (relation.DependentEntityHasNavigation)
            {
                var dependentNavigationProperty = relation.DependentNavigationProperty;
                var dependentNavigationPropertyToRemove = relation.DependentEntityInfo.Properties
                    .Single(x => x.PropertyName == dependentNavigationProperty!.Name &&
                    x.PropertyType == dependentNavigationProperty.PropertyType);
                relation.DependentEntityInfo.Properties.Remove(dependentNavigationPropertyToRemove);
            }
            foreach (var foreignKeyProperty in relation.DependentForeignKeyProperties)
            {
                var propertyInfoToRemove = relation.DependentEntityInfo.Properties
                    .Single(x => x.PropertyName == foreignKeyProperty.Name &&
                        x.PropertyType == foreignKeyProperty.PropertyType);
                relation.DependentEntityInfo.Properties.Remove(propertyInfoToRemove);
            }
            
        }

        foreach (var relation in model.ManyToManyRelations)
        {
            var principalNavigationProperty = relation.LeftNavigationProperty;
            var principalPropertyToRemove = relation.LeftEntityInfo.Properties
                .Single(x => x.PropertyName == principalNavigationProperty.Name
                && x.PropertyType == principalNavigationProperty.PropertyType);
            relation.LeftEntityInfo.Properties.Remove(principalPropertyToRemove);

            var dependentNavigationProperty = relation.RightNavigationProperty;
            var dependentNavigationPropertyToRemove = relation.RightEntityInfo.Properties
                .Single(x => x.PropertyName == dependentNavigationProperty.Name &&
                x.PropertyType == dependentNavigationProperty.PropertyType);
            relation.RightEntityInfo.Properties.Remove(dependentNavigationPropertyToRemove);
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

    class DbContextEntityInfo
    {
        public Type EntityType { get; init; }
        public List<PropertyInfo> Properties { get; init; }
    }
}
