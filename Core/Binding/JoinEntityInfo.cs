using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Binding;
public class JoinEntityData
{
    public PropertyInfo? LeftNavigationToJoinProperty;
    public PropertyInfo? RightNavigationToJoinProperty;
    public PropertyInfo? LeftNavigationFromJoinProperty;
    public PropertyInfo? RightNavigationFromJoinProperty;
    public PropertyInfo? LeftForeignKeyPropertyOnJoinEntity;
    public PropertyInfo? RightForeignKeyPropertyOnJoinEntity;
    public PropertyInfo? LeftPrimaryKeyProperty;
    public PropertyInfo? RightPrimaryKeyProperty;
}

