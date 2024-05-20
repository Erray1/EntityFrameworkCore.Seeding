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
    
    public PropertyInfo LeftNavigationProperty => Navigation.PropertyInfo!;
    public PropertyInfo RightNavigationProperty => Navigation.Inverse.PropertyInfo!;
    public SeederEntityInfo LeftEntityInfo { get; init; }
    public SeederEntityInfo RightEntityInfo { get; init; }
    public bool IsNumberOfboundEntitiesSpecified => NumberOfBoundEntitiesFromLeft != -1 || NumberOfBoundEntitiesFromRight != -1;
    public int BindLocalityFromLeft { get; set; } = 0;
    public int NumberOfBoundEntitiesFromLeft { get; set; } = -1;
    public int BindLocalityFromRight { get; set; } = 0;
    public int NumberOfBoundEntitiesFromRight { get; set; } = -1;
    public void SetNumberOfBoundEntitiesFor(SeederEntityInfo forEntity, int value, int locality = 0)
    {
        if (value - locality < 0)
        {
            throw new ArgumentException("Locality too large. There is a possibility of negative number of connections");
        }
        if (forEntity.EntityType == LeftEntityInfo.EntityType)
        {
            if (NumberOfBoundEntitiesFromRight != -1)
            {
                throw new Exception("Can set number of connected entities only on one side of N:N relation");
            }
            NumberOfBoundEntitiesFromLeft = value;
            BindLocalityFromLeft = locality;
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
        ISkipNavigation principalNavigation)
    {
        Navigation = principalNavigation;
        LeftEntityInfo = leftEntityInfo;
        RightEntityInfo = rightEntityInfo;
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

