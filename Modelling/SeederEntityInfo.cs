using EntityFrameworkCore.Seeding.StockData;

namespace EntityFrameworkCore.Seeding.Modelling;

public sealed class SeederEntityInfo
{
    public bool IsConfigured;
    public Type EntityType { get; init; }
    public int TimesCreated { get { return TimesCreated + Locality; } set { TimesCreated = value; } }
    public int Locality { get; set; } = 0;
    public bool LoadsData { get; set; } = false;
    public SeederStockDataCollection? LoadedValues { get; set; }
    public List<SeederEntityInfo> LinkedEntities { get; init; } = new();
    public Dictionary<SeederEntityInfo, int> NullableLinkedEntitiesProbabilities { get; init; } = new();
    public List<object>? PossibleValues { get; set; }
    public List<SeederPropertyInfo> Properties { get; set; } = new();
    public SeederEntityInfo(Type entityType)
    {
        EntityType = entityType;
        IsConfigured = false;
    }
    public override int GetHashCode()
    {
        return EntityType.GetHashCode();
    }
}
