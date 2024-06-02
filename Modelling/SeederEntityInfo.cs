using EntityFrameworkCore.Seeding.Core.Binding;
using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.StockData;
using System.Reflection;

namespace EntityFrameworkCore.Seeding.Modelling;

public sealed class SeederEntityInfo
{
    public bool IsJoinEntity { get; set; }
    public bool DoCreate { get; set; } = true;
    public bool IsConfigured => Properties.All(x => x.IsConfigured);
    public Type EntityType { get; init; }
    public int TimesCreated { get; set; }
    public List<SeederPropertyInfo> Properties { get; set; } = new();
    public List<EntityCreatedFromJsonInfo> JSONInfo { get; set; } = new();
    public SeederEntityInfo(Type entityType)
    {
        EntityType = entityType;
    }
    public bool ShuffleValues { get;set; } = false;
    public override int GetHashCode()
    {
        return EntityType.GetHashCode();
    }
}
