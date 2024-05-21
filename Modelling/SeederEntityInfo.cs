using EntityFrameworkCore.Seeding.Core.Binding;
using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.StockData;

namespace EntityFrameworkCore.Seeding.Modelling;

public sealed class SeederEntityInfo
{
    public bool DoCreate { get; set; } = true;
    public bool IsConfigured;
    public Type EntityType { get; init; }
    public int TimesCreated { get; set; }
    public bool LoadsData { get; set; } = false;
    public SeederStockDataCollection? LoadedValues { get; set; }
    public bool StrictMatchingForLoadedData { get; set; }
    public bool IsPrincipal {  get; set; }
    public List<object>? PossibleValues { get; set; }
    public List<SeederPropertyInfo> Properties { get; set; } = new();
    public List<EntityCreatedFromJsonInfo> JSONInfo { get; set; } = new();
    public SeederEntityInfo(Type entityType)
    {
        EntityType = entityType;
        IsConfigured = false;
    }
    public bool ShuffleValues { get;set; } = false;
    public override int GetHashCode()
    {
        return EntityType.GetHashCode();
    }
}
