using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
public static class SeederCoreUtilities
{
    public static (MethodInfo, object) GetMethodAndDbSet(DbContext dbContext, Type entityType, string methodName)
    {

        var dbset = GetDbSet(dbContext, entityType);
        var method = GetDbSetMethod(methodName, dbset);
        return (method, dbset);
    }

    public static object GetDbSet(DbContext dbContext, Type entityType)
    {
        var method = dbContext.GetType()
        .GetMethods()
        .Where(x => x.Name == nameof(dbContext.Set))
        .Single(x => x
            .GetParameters()
            .Count() == 0);
        MethodInfo generic = method.MakeGenericMethod(entityType);
        var dbset = generic.Invoke(dbContext, null)!;
        return dbset;
    }

    public static MethodInfo GetDbSetMethod(string methodName, object dbSet)
    {
        var method = dbSet.GetType()
            .GetMethod(methodName)!;
        return method;
    }
}

