using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Modelling.Utilities;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Binding;
public sealed class EntityRelation
{
    public bool IsPrincipal => !PrincipalNavigation.IsOnDependent;
    public INavigation PrincipalNavigation;
    public INavigation? InverseNavigation => PrincipalNavigation.Inverse;
    public IForeignKey ForeignKey => PrincipalNavigation.ForeignKey;
    public PropertyInfo PrimaryKeyProperty => ForeignKey.PrincipalEntityType.FindDeclaredPrimaryKey()!.Properties[0].PropertyInfo!;
    public bool DependentEntityHasNavigation => InverseNavigation != null;
    public PropertyInfo PrincipalNavigationProperty => PrincipalNavigation.PropertyInfo!;
    public PropertyInfo? DependentNavigationProperty => DependentEntityHasNavigation ?
    InverseNavigation!.PropertyInfo! : null;
    public PropertyInfo[] DependentForeignKeyProperties => ForeignKey.Properties.Select(x => x.PropertyInfo!).ToArray();

    public bool IsOneToOne;
    public bool IsNullable => !ForeignKey.IsRequiredDependent;
    public double BindProbability { get; set; } = 1;

    public SeederEntityInfo PrincipalEntityInfo { get; init; }
    public SeederEntityInfo DependentEntityInfo { get; init; }
    public EntityRelation(
        SeederEntityInfo principalEntityInfo,
        SeederEntityInfo dependentEntityInfo,
        INavigation principalNavigation)
    {
        PrincipalNavigation = principalNavigation;
        DependentEntityInfo = dependentEntityInfo;
        PrincipalEntityInfo = principalEntityInfo;
    }
}

