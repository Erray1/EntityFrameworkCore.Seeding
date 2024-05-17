using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Modelling.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EntityFrameworkCore.Seeding.Core.Binding;
public class SeederEntityBinder
{
    protected IModel _dbModel;
    protected Dictionary<SeederEntityInfo, List<object>> _entities;
    public void BindEntities(Dictionary<SeederEntityInfo, List<object>> entities, IModel dbModel)
    {
        _entities = entities;
        _dbModel = dbModel;

        var entityTypes = dbModel.GetEntityTypes();

        var allNavigations = entityTypes
            .SelectMany(x => x.GetNavigations())
            .ToList();

        var allManyToManyNavigations = entityTypes
            .SelectMany(x => x.GetSkipNavigations())
            .ToList();

        foreach (var (entity, pool) in entities)
        {
            var navigations = allNavigations
                .Where(x => x.DeclaringEntityType.Name == entity.EntityType.FullName);

            var manyToManyNavigations = allManyToManyNavigations
                .Where(x => x.DeclaringEntityType.Name == entity.EntityType.FullName);

            foreach (var navigation in navigations) // 1:1 and 1:N
            {
                if (navigation.IsOnDependent) continue;

                IForeignKey foreignKey = navigation.ForeignKey;
                Type dependentEntityType = navigation.TargetEntityType.ClrType;

                (SeederEntityInfo dependentEntity, List<object> dependentPool)
                    = entities.Single(x => x.Key.EntityType == dependentEntityType);

                PropertyInfo principalNavigationProperty = navigation.PropertyInfo!;

                INavigation? inverseNavigation = navigation.Inverse;

                // Имеется ли навигация или по внешнему ключу
                bool dependentEntityHasNavigation = inverseNavigation != null;
                PropertyInfo dependentNavigationProperty = dependentEntityHasNavigation ?
                    inverseNavigation!.PropertyInfo! :
                    foreignKey.Properties.First().PropertyInfo!;
                

                EntityRelationType relationType = getEntitiesRelationType(principalNavigationProperty, dependentNavigationProperty); ;

                if (relationType == EntityRelationType.OneToOne || relationType == EntityRelationType.OneToOneNullable)
                {
                    for (int i = 0; i < entity.TimesCreated; i++)
                    {
                        var principalEntityObject = pool[i];
                        var dependentEntityObject = dependentPool[i];
                        if (dependentEntityHasNavigation)
                        {
                            dependentNavigationProperty.SetValue(dependentEntityObject, principalEntityObject);
                        }
                        var dependentForeignKeyProperty = foreignKey.Properties.First().PropertyInfo!;
                        var principalPrimaryKey = navigation.ForeignKey.PrincipalEntityType
                            .FindDeclaredPrimaryKey()!
                            .Properties[0]
                            .PropertyInfo!
                            .GetValue(principalEntityObject);

                        principalNavigationProperty.SetValue(principalEntityObject, dependentEntityObject);
                        dependentForeignKeyProperty.SetValue(dependentEntityObject, principalPrimaryKey);
                    }
                }
                else if (relationType == EntityRelationType.OneToMany || relationType == EntityRelationType.OneToManyNullableDependent)
                {
                    var dependentPoolSpan = CollectionsMarshal.AsSpan(dependentPool);
                    var random = new Random(dependentEntityType.GetHashCode());
                    random.Shuffle(dependentPoolSpan);

                    Stack<object> dependentEntitiesStack = new Stack<object>(dependentPoolSpan.ToArray());
                    # warning Доделай блять эту хуйню уже нормально сука
                    int numberOfBoundaries = 3; // entity.NumberOfBoundEntitiesInOneToManyRelationships[dependentEntity];
                    bool nullable = entity.NullableLinkedEntitiesProbabilities.TryGetValue(dependentEntity, out double? boundProbability);

                    MethodInfo principalNavigationCollectionAddMethod = principalNavigationProperty
                                .PropertyType
                                .GetRuntimeMethods()
                                .Single(x => x.Name == "Add");

                    for (int i = 0; i < entity.TimesCreated; i++)
                    {
                        var principalEntityObject = pool[i];
                        var principalPrimaryKey = navigation.ForeignKey.PrincipalEntityType
                                .FindDeclaredPrimaryKey()!
                                .Properties[0]
                                .PropertyInfo!
                                .GetValue(principalEntityObject);
                        object principalCollection = principalNavigationProperty.GetGetMethod()!.Invoke(principalEntityObject, [])!;

                        for (int j = 0; j < numberOfBoundaries; j++)
                        {
                            var dependentEntityObject = dependentEntitiesStack.Pop();
                            if (nullable && random.NextDouble() < boundProbability) continue; // Бывает не повезло не фортануло

                            if (dependentEntityHasNavigation)
                            {
                                dependentNavigationProperty.SetValue(dependentEntityObject, principalEntityObject);
                            }
                            PropertyInfo dependentForeignKeyProperty = foreignKey.Properties.First().PropertyInfo!;
                            
                            principalNavigationCollectionAddMethod.Invoke(principalCollection, [dependentEntityObject]);

                            dependentForeignKeyProperty.SetValue(dependentEntityObject, principalPrimaryKey);
                        }
                    }
                }
            }
            foreach (var navigation in manyToManyNavigations)
            {
                IForeignKey foreignKey = navigation.ForeignKey;
                Type dependentEntityType = navigation.TargetEntityType.ClrType;

                (SeederEntityInfo dependentEntity, List<object> dependentPool)
                    = entities.Single(x => x.Key.EntityType == dependentEntityType);

                PropertyInfo principalNavigationProperty = navigation.PropertyInfo!;
                PropertyInfo dependentNavigationProperty = navigation.Inverse.PropertyInfo!;

                MethodInfo principalNavigationCollectionAddMethod = principalNavigationProperty
                                .PropertyType
                                .GetRuntimeMethods()
                                .Single(x => x.Name == "Add");
                MethodInfo dependentNavigationCollectionAddMethod = dependentNavigationProperty
                                .PropertyType
                                .GetRuntimeMethods()
                                .Single(x => x.Name == "Add");

                var random = new Random(dependentEntityType.GetHashCode());

                for (int i = 0; i < entity.TimesCreated; i++)
                {
                    var principalEntityObject = pool[i];
                    object principalCollection = principalNavigationProperty.GetGetMethod()!.Invoke(principalEntityObject, [])!;
                    int numberOfBoundaries = 100; // entity.NumberOfBoundEntitiesInOneToManyRelationships[dependentEntity];

                    for (int j = 0; j < numberOfBoundaries / 2; j++)
                    {
                        var dependentEntityObject = dependentPool[random.Next(0, numberOfBoundaries)];
                        object dependentCollection = dependentNavigationProperty.GetGetMethod()!.Invoke(dependentEntityObject, [])!;
                        principalNavigationCollectionAddMethod.Invoke(principalCollection, [dependentEntityObject]);
                        dependentNavigationCollectionAddMethod.Invoke (dependentCollection, [principalEntityObject]);
                    }
                }
            }
        }
        return;
    }
    private EntityRelationType getEntitiesRelationType(PropertyInfo principalProperty, PropertyInfo dependentProperty)
    {
        bool isPrincipalNullable = new NullabilityInfoContext().Create(principalProperty).WriteState is NullabilityState.Nullable;
        bool isDependentNullable = new NullabilityInfoContext().Create(dependentProperty).WriteState is NullabilityState.Nullable;
        bool isPrincipalCollection = isCollection(principalProperty.PropertyType);
        bool isDependentCollection = isCollection(dependentProperty.PropertyType);

        if (isPrincipalCollection && isDependentCollection) // N : N
        {
            return EntityRelationType.ManyToMany;
        }
        if (isPrincipalCollection && !isDependentCollection) // 1 : N
        {
            if (isDependentNullable)
            {
                return EntityRelationType.OneToManyNullableDependent;
            }
            return EntityRelationType.OneToMany;
        }
        if (!isPrincipalCollection && !isDependentCollection) // 1 : 1
        {
            if (isPrincipalNullable && isDependentNullable)
            {
                return EntityRelationType.OneToOneNullable;
            }
            else
            {
                return EntityRelationType.OneToOne;
            }
        }
        throw new NotSupportedException($"Unknow relation type between {principalProperty.DeclaringType!.Name} and {dependentProperty.DeclaringType!.Name}");
    }

    private bool isCollection(Type type)
    {
        var interfaces = type.GetInterfaces();

        return interfaces.Any(x => x.Name.Contains("IEnumerable")) && type != typeof(string);
    }
}

