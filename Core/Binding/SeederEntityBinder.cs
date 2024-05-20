using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Modelling.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EntityFrameworkCore.Seeding.Core.Binding;
public class SeederEntityBinder<TDbContext>
{
    public SeederEntityBinder(IServiceProvider serviceProvider)
    {
        _model = serviceProvider.GetRequiredKeyedService<SeederModelProvider>(typeof(TDbContext).Name).GetModel();
    }
    private SeederModelInfo _model;
    private Dictionary<SeederEntityInfo, List<object>> _entities;
    public void BindEntities(Dictionary<SeederEntityInfo, List<object>> entities)
    {
        _entities = entities;

        foreach (var relation in _model.Relations) // 1:1 and 1:N
        {
            List<object> principalPool = entities[relation.PrincipalEntityInfo];
            List<object> dependentPool = entities[relation.DependentEntityInfo];
            var random = new Random(relation.PrincipalEntityInfo.GetHashCode());

            if (relation.IsOneToOne)
            {
                for (int i = 0; i < relation.PrincipalEntityInfo.TimesCreated; i++)
                {
                    if (relation.IsNullable && random.NextDouble() < relation.BindProbability) continue;
                    var principalEntityObject = principalPool[i];
                    var dependentEntityObject = dependentPool[i];

                    if (relation.DependentEntityHasNavigation)
                    {
                        relation.DependentNavigationProperty!.SetValue(dependentEntityObject, principalEntityObject);
                    }
                    var principalPrimaryKey = relation.PrimaryKeyProperty.GetValue(principalEntityObject);

                    relation.PrincipalNavigationProperty.SetValue(principalEntityObject, dependentEntityObject);
                    relation.DependentForeignKeyProperties[0].SetValue(dependentEntityObject, principalPrimaryKey);
                }
            }
            else
            {
                var dependentPoolSpan = CollectionsMarshal.AsSpan(dependentPool);
                random.Shuffle(dependentPoolSpan);

                Stack<object> dependentEntitiesStack = new Stack<object>(dependentPoolSpan.ToArray());
                MethodInfo principalNavigationCollectionAddMethod = relation.PrincipalNavigationProperty
                            .PropertyType
                            .GetRuntimeMethods()
                            .Single(x => x.Name == "Add");

                for (int i = 0; i < relation.DependentEntityInfo.TimesCreated; i++)
                {
                    var principalEntityObject = principalPool[random.Next(0, relation.PrincipalEntityInfo.TimesCreated)];
                    object dependentEntityObject = dependentEntitiesStack.Pop();

                    object principalCollection = relation.PrincipalNavigationProperty
                        .GetGetMethod()!
                        .Invoke(principalEntityObject, [])!;
                    var principalPrimaryKey = relation.PrimaryKeyProperty.GetValue(principalEntityObject);

                    if (relation.IsNullable && random.NextDouble() > relation.BindProbability) continue; // Бывает не повезло не фортануло

                    if (relation.DependentEntityHasNavigation)
                    {
                        relation.DependentNavigationProperty!.SetValue(dependentEntityObject, principalEntityObject);
                    }
                    PropertyInfo dependentForeignKeyProperty = relation.DependentForeignKeyProperties.First();

                    principalNavigationCollectionAddMethod.Invoke(principalCollection, [dependentEntityObject]);

                    dependentForeignKeyProperty.SetValue(dependentEntityObject, principalPrimaryKey);
                }
                //for (int i = 0; i < relation.PrincipalEntityInfo.TimesCreated; i++)
                //{
                //    var principalEntityObject = principalPool[i];
                //    var principalPrimaryKey = relation.PrimaryKeyProperty.GetValue(principalEntityObject);
                //    bool dependendentEntitiesStackHasObject = true;

                //    object principalCollection = relation.PrincipalNavigationProperty
                //        .GetGetMethod()!
                //        .Invoke(principalEntityObject, [])!;

                //    int numberOfBoundaries = relation.DependentEntityInfo.TimesCreated / relation.PrincipalEntityInfo.TimesCreated;

                //    for (int j = 0; j < numberOfBoundaries; j++)
                //    {
                //        dependendentEntitiesStackHasObject = dependentEntitiesStack.TryPeek(out object? dependentEntityObject);
                //        if (!dependendentEntitiesStackHasObject) break;
                //        if (relation.IsNullable && random.NextDouble() > relation.BindProbability) continue; // Бывает не повезло не фортануло

                //        if (relation.DependentEntityHasNavigation)
                //        {
                //            relation.DependentNavigationProperty!.SetValue(dependentEntityObject, principalEntityObject);
                //        }
                //        PropertyInfo dependentForeignKeyProperty = relation.DependentForeignKeyProperties.First();

                //        principalNavigationCollectionAddMethod.Invoke(principalCollection, [dependentEntityObject]);

                //        dependentForeignKeyProperty.SetValue(dependentEntityObject, principalPrimaryKey);
                //    }
                //    if (!dependendentEntitiesStackHasObject) break;
                //}
            }
        }
        foreach (var relation in _model.ManyToManyRelations)
        {
            var (leftSummary, rightSummary) = relation.GetSummary();
            List<object> leftPool = entities[leftSummary.EntityInfo];
            List<object> rightPool = entities[rightSummary.EntityInfo];

            MethodInfo leftNavigationCollectionAddMethod = leftSummary.NavigationProperty
                            .PropertyType
                            .GetRuntimeMethods()
                            .Single(x => x.Name == "Add");
            MethodInfo rightNavigationCollectionAddMethod = rightSummary.NavigationProperty
                            .PropertyType
                            .GetRuntimeMethods()
                            .Single(x => x.Name == "Add");

            var random = new Random(leftSummary.EntityInfo.GetHashCode() + rightSummary.EntityInfo.GetHashCode());

            for (int i = 0; i < leftSummary.EntityInfo.TimesCreated; i++)
            {
                var leftEntityObject = leftPool[i];
                object leftCollection = leftSummary.NavigationProperty
                    .GetGetMethod()!
                    .Invoke(leftEntityObject, [])!;
                int numberOfBounds = leftSummary.NumberOfBoundEntities + leftSummary.BindLocality;

                for (int j = 0; j < numberOfBounds; j++)
                {
                    var rightEntityObject = rightPool[random.Next(0, rightSummary.EntityInfo.TimesCreated)];
                    object rightCollection = rightSummary.NavigationProperty
                        .GetGetMethod()!
                        .Invoke(rightEntityObject, [])!;
                    leftNavigationCollectionAddMethod.Invoke(leftCollection, [rightEntityObject]);
                    rightNavigationCollectionAddMethod.Invoke(rightCollection, [leftEntityObject]);
                }
            }
        }
    }
}

