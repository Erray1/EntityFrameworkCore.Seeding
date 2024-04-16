using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.StockData;

namespace EntityFrameworkCore.Seeding.Modelling;

public sealed class SeederEntityInfo
{
    public bool IsConfigured;
    public Type EntityType { get; init; }
    public int TimesCreated { get; set; }
    public int Locality { get; set; } = 0;
    public bool LoadsData { get; set; } = false;
    public SeederStockDataCollection? LoadedValues { get; set; }
    public bool StrictMatchingForLoadedData { get; set; }
    public Dictionary<SeederEntityInfo, EntityRelationType> LinkedEntities { get; set; }
    public Dictionary<SeederEntityInfo, int> NullableLinkedEntitiesProbabilities { get; set; }
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
