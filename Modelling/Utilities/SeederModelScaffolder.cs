using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Seeding.Modelling.Utilities;

public static class SeederModelScaffolder
{
    public static SeederModelInfo CreateEmptyFromDbContext<TDbcontext>()
        where TDbcontext : DbContext
    {
        var model = new SeederModelInfo();
        addEntitiesAndProperties<TDbcontext>(model);
        return model;
    }
    private static void addEntitiesAndProperties<TDbContext>(SeederModelInfo model)
        where TDbContext : DbContext
    {
        var dbContextInfo = typeof(TDbContext).GetFields()
            .Where(x => x.IsPublic)
            .Select(x => new DbContextEntityInfo
            {
                EntityType = x.FieldType
            };
    }
    private static void addProperties(SeederEntityInfo entity)
    {

    }
    class DbContextEntityInfo
    {
        public Type EntityType { get; init; }
    }
}
