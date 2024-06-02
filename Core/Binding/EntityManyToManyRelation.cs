using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Binding;
public class EntityManyToManyRelation
{
    public ISkipNavigation Navigation;
    public IForeignKey ForeignKey => Navigation.ForeignKey;
    public IEntityType JoinEntityType => Navigation.JoinEntityType;
    public bool HasJoinEntity => JoinEntityType != null;
    
    public PropertyInfo LeftNavigationProperty => Navigation.PropertyInfo!;
    public PropertyInfo RightNavigationProperty => Navigation.Inverse.PropertyInfo!;
    public SeederEntityInfo LeftEntityInfo { get; init; }
    public SeederEntityInfo RightEntityInfo { get; init; }
    public SeederEntityInfo? JoinEntityInfo { get; init; }
    public JoinEntityData? JoinEntityData { get; init; }
    public bool JoinEntityHasPayload => JoinEntityInfo.Properties.Count != 0;
    public bool IsNumberOfboundEntitiesSpecified => NumberOfBoundEntitiesFromLeft != -1 || NumberOfBoundEntitiesFromRight != -1;
    public int BindLocalityFromLeft { get; set; } = 0;
    public int NumberOfBoundEntitiesFromLeft { get; set; } = -1;
    public int BindLocalityFromRight { get; set; } = 0;
    public int NumberOfBoundEntitiesFromRight { get; set; } = -1;
    public void SetNumberOfBoundEntitiesFor(SeederEntityInfo entity, int value, int locality = 0)
    {
        if (value - locality < 0)
        {
            throw new ArgumentException("Locality too large. There is a possibility of negative number of connections");
        }
        if (entity.EntityType == LeftEntityInfo.EntityType)
        {
            if (NumberOfBoundEntitiesFromRight != -1)
            {
                throw new InvalidOperationException("Can set number of connected entities only on one side of N:N relation");
            }
            NumberOfBoundEntitiesFromLeft = value;
            BindLocalityFromLeft = locality;
            if (JoinEntityInfo != null)
            {
                JoinEntityInfo.TimesCreated = (value + Math.Abs(locality)) * LeftEntityInfo.TimesCreated;
            }
            
            
        }
        else
        {
            if (NumberOfBoundEntitiesFromLeft != -1)
            {
                throw new Exception("Can set number of connected entities only on one side of N:N relation");
            }
            NumberOfBoundEntitiesFromRight = value;
            BindLocalityFromRight = locality;
        }
    }
    public (RelationSummaryForBinder left, RelationSummaryForBinder right) GetSummary()
    {
        RelationSummaryForBinder left = new(
            NumberOfBoundEntitiesFromLeft != -1,
            LeftEntityInfo,
            LeftNavigationProperty,
            NumberOfBoundEntitiesFromLeft,
            BindLocalityFromLeft);

        RelationSummaryForBinder right = new(
            NumberOfBoundEntitiesFromRight != -1,
            RightEntityInfo,
            RightNavigationProperty,
            NumberOfBoundEntitiesFromRight,
            BindLocalityFromRight);

        bool onlyOneRequestedBinding = left.RequestedBinding != right.RequestedBinding;
        if (!onlyOneRequestedBinding)
        {
            throw new Exception("Impossible....");
        }
        else
        {
            if (left.RequestedBinding)
            {
                return (left, right);
            }
            return (right, left);
        }
    }
    public bool Compare(SeederEntityInfo first, SeederEntityInfo second)
    {
        return LeftEntityInfo.EntityType == first.EntityType && RightEntityInfo.EntityType == second.EntityType ||
            RightEntityInfo.EntityType == first.EntityType && LeftEntityInfo.EntityType == second.EntityType;
    }
    public EntityManyToManyRelation(
        SeederEntityInfo leftEntityInfo,
        SeederEntityInfo rightEntityInfo,
        SeederEntityInfo? joinEntityInfo,
        ISkipNavigation principalNavigation)
    {
        Navigation = principalNavigation;
        LeftEntityInfo = leftEntityInfo;
        RightEntityInfo = rightEntityInfo;
        JoinEntityInfo = joinEntityInfo;

        JoinEntityData joinData = new();
        joinData.LeftNavigationToJoinProperty = PropertyAccessors.GetNavigationToJoinProperty(joinEntityInfo, leftEntityInfo);
        joinData.RightNavigationToJoinProperty = PropertyAccessors.GetNavigationToJoinProperty(joinEntityInfo, rightEntityInfo);
        joinData.LeftNavigationFromJoinProperty = PropertyAccessors.GetNavigationFromJoinProperty(joinEntityInfo, leftEntityInfo);
        joinData.RightNavigationFromJoinProperty = PropertyAccessors.GetNavigationFromJoinProperty(joinEntityInfo, rightEntityInfo);
        joinData.LeftForeignKeyPropertyOnJoinEntity = PropertyAccessors.GetForeignKeyPropertyOnJoinEntity(JoinEntityType, leftEntityInfo);
        joinData.RightForeignKeyPropertyOnJoinEntity = PropertyAccessors.GetForeignKeyPropertyOnJoinEntity(JoinEntityType, rightEntityInfo);
        joinData.LeftPrimaryKeyProperty = PropertyAccessors.GetPrimaryKeyProperty(JoinEntityType, leftEntityInfo);
        joinData.RightPrimaryKeyProperty = PropertyAccessors.GetPrimaryKeyProperty(JoinEntityType, rightEntityInfo);

        JoinEntityData = joinData;
    }
}

file static class PropertyAccessors
{
    public static PropertyInfo? GetNavigationToJoinProperty(SeederEntityInfo? joinEntity, SeederEntityInfo declaringEntity)
    {
        return joinEntity is not null ?
            declaringEntity
            .EntityType
            .GetProperties()
            .SingleOrDefault(x => {
                var genericParams = x.PropertyType.GetGenericArguments();
                if (genericParams.Count() == 0) return false;
                return genericParams[0] == joinEntity.EntityType;
            })
            : null;
    }

    public static PropertyInfo? GetNavigationFromJoinProperty(SeederEntityInfo joinEntity, SeederEntityInfo targetEntity)
    {
        return joinEntity is not null ?
            joinEntity
            .EntityType
            .GetProperties()
            .SingleOrDefault(x => x.PropertyType == targetEntity.EntityType)
            : null;
    }

    public static PropertyInfo? GetForeignKeyPropertyOnJoinEntity(IEntityType joinEntity, SeederEntityInfo targetEntity)
    {
        return joinEntity?
            .GetForeignKeyProperties()
            .SingleOrDefault(x =>
            {
                var primaryKey = x.FindFirstPrincipal();
                if (primaryKey is null) return false;
                var targetEntityType = primaryKey.DeclaringEntityType.ClrType;
                return targetEntityType == targetEntity.EntityType;
            })
            .PropertyInfo;
    }

    public static PropertyInfo? GetPrimaryKeyProperty(IEntityType joinEntity, SeederEntityInfo targetEntity)
    {
        return joinEntity
            .GetForeignKeyProperties()
            .Select(x => x.FindFirstPrincipal())
            .First(x => x.DeclaringEntityType.ClrType == targetEntity.EntityType)
            .PropertyInfo;
    }
}

public record RelationSummaryForBinder
(
    bool RequestedBinding,
    SeederEntityInfo EntityInfo,
    PropertyInfo NavigationProperty,
    int NumberOfBoundEntities,
    int BindLocality
);

