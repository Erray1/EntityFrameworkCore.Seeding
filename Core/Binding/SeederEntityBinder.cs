using EntityFrameworkCore.Seeding.Core.Binding.BindingSteps;
using EntityFrameworkCore.Seeding.Core.Binding.Context;
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
            }
        }
        foreach (var relation in _model.ManyToManyRelations)
        {
            var (leftSummary, rightSummary) = relation.GetSummary();
            List<object> leftPool = entities[leftSummary.EntityInfo];
            List<object> rightPool = entities[rightSummary.EntityInfo];

            bool joinEntitiesAreCreated = entities.TryGetValue(relation.JoinEntityInfo, out List<object> joinPool);

            //Stack<object> leftPoolStack = new(leftPool);
            //Stack<object> rightPoolStack = new(rightPool);
            //Stack<object>? joinPoolStack = joinEntitiesAreCreated ? new(joinPool) : null;

            //ManyToManyBindingStepsLinker linker = new(relation);
            //ManyToManyBindingStepBase chain = linker.CreateChain();

            MethodInfo leftNavigationCollectionAddMethod = leftSummary.NavigationProperty
                            .PropertyType
                            .GetRuntimeMethods()
                            .Single(x => x.Name == "Add");
            MethodInfo rightNavigationCollectionAddMethod = rightSummary.NavigationProperty
                            .PropertyType
                            .GetRuntimeMethods()
                            .Single(x => x.Name == "Add");

            var random = new Random(leftSummary.EntityInfo.GetHashCode() + rightSummary.EntityInfo.GetHashCode());

            // If relation does not have join entity
            if (!joinEntitiesAreCreated )
            {
                for (int i = 0; i < leftSummary.EntityInfo.TimesCreated; i++)
                {
                    var leftEntityObject = leftPool[i];
                    object leftCollection = leftSummary.NavigationProperty
                        .GetGetMethod()!
                        .Invoke(leftEntityObject, [])!;

                    int numberOfBounds = leftSummary.NumberOfBoundEntities + random.Next(0, leftSummary.BindLocality);

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
                continue;
            }
            // If relation has join entity

            MethodInfo? leftNavigationCollectionToJoinAddMethod = 
                            relation
                            .JoinEntityData
                            .LeftNavigationToJoinProperty?
                            .PropertyType
                            .GetRuntimeMethods()
                            .Single(x => x.Name == "Add");
            MethodInfo? rightNavigationCollectionToJoinAddMethod =
                            relation
                            .JoinEntityData
                            .RightNavigationToJoinProperty?
                            .PropertyType
                            .GetRuntimeMethods()
                            .Single(x => x.Name == "Add");
            MethodInfo leftMainCollectionContainsMethod =
                       relation
                            .LeftNavigationProperty
                            .PropertyType
                            .GetRuntimeMethods()
                            .Where(x => x.Name == "Contains")
                            .First();

            int joinEntityPoolIndex = -1;
            for (int i = 0; i < leftSummary.EntityInfo.TimesCreated; i++)
            {
                var leftEntityObject = leftPool[i];
                object leftMainCollection = leftSummary.NavigationProperty
                    .GetGetMethod()!
                    .Invoke(leftEntityObject, [])!;
                object? leftToJoinCollection = relation.JoinEntityData.LeftNavigationToJoinProperty?
                    .GetGetMethod()?
                    .Invoke(leftEntityObject, [])!;

                int numberOfBounds = leftSummary.NumberOfBoundEntities + random.Next(0, leftSummary.BindLocality);
                for (int j = 0; j < numberOfBounds; j++)
                {
                    // Main
                    var rightEntityObject = rightPool[random.Next(0, rightSummary.EntityInfo.TimesCreated)];

                    // If bound is already exists
                    if ((bool)leftMainCollectionContainsMethod.Invoke(leftMainCollection, [rightEntityObject])) continue;

                    object rightMainCollection = rightSummary.NavigationProperty
                            .GetGetMethod()!
                            .Invoke(rightEntityObject, [])!;

                    object joinEntityObject = joinPool[++joinEntityPoolIndex];

                    // Collection navigation to join
                    object? rightToJoinCollection = relation.JoinEntityData.RightNavigationToJoinProperty?
                    .GetGetMethod()?
                    .Invoke(rightEntityObject, [])!;


                    // Binding main entities
                    leftNavigationCollectionAddMethod.Invoke(leftMainCollection, [rightEntityObject]);
                    rightNavigationCollectionAddMethod.Invoke(rightMainCollection, [leftEntityObject]);

                    // If entities have navigations to join entity
                    if (leftToJoinCollection is not null)
                    {
                        leftNavigationCollectionToJoinAddMethod.Invoke(leftToJoinCollection, [joinEntityObject]);
                    }
                    if (rightToJoinCollection is not null)
                    {
                        rightNavigationCollectionToJoinAddMethod.Invoke(rightToJoinCollection, [joinEntityObject]);
                    }

                    // If join entity have navigation to principal entity
                    if (relation.JoinEntityData.LeftNavigationFromJoinProperty is not null)
                    {
                        relation.JoinEntityData.LeftNavigationFromJoinProperty.SetValue(joinEntityObject, leftEntityObject);
                    }
                    if (relation.JoinEntityData.RightNavigationFromJoinProperty is not null)
                    {
                        relation.JoinEntityData.RightNavigationFromJoinProperty.SetValue(joinEntityObject, rightEntityObject);
                    }

                    // If join entity has foreign keys to principal entity
                    if (relation.JoinEntityData.LeftForeignKeyPropertyOnJoinEntity is not null)
                    {
                        object leftPrimaryKeyValue = relation.JoinEntityData
                            .LeftPrimaryKeyProperty
                            .GetValue(leftEntityObject)!;
                        relation.JoinEntityData.LeftForeignKeyPropertyOnJoinEntity.SetValue(joinEntityObject, leftPrimaryKeyValue);
                    }

                    if (relation.JoinEntityData.RightForeignKeyPropertyOnJoinEntity is not null)
                    {
                        object rightPrimaryKeyValue = relation.JoinEntityData
                            .RightPrimaryKeyProperty
                            .GetValue(rightEntityObject, []);
                        relation.JoinEntityData.RightForeignKeyPropertyOnJoinEntity.SetValue(joinEntityObject, rightPrimaryKeyValue);
                    }
                }
            }

        }
    }
}

