using EntityFrameworkCore.Seeding.Core;
using EntityFrameworkCore.Seeding.StockData;
using System.ComponentModel;

namespace EntityFrameworkCore.Seeding.Modelling;
public class SeederPropertyInfo : SeederInfoBase
{
    public SeederPropertyInfo(Type propertyType)
    {
        PropertyType = propertyType;
    }
    private bool isConfigured = false;
    public bool IsConfigured { get { return isConfigured; } set
        {
            if (isConfigured && value)
            {
                throw new WarningException($"Trying to configure property {PropertyName} multiple times. Configuration will be overriden");
            }
            isConfigured = value;
        }
    }
    public Type PropertyType { get; set; }
    public string PropertyName { get; set; }
    public IEnumerable<object>? PossibleValuesPool { get; set; }
    public SeederStockDataPropertyCollection? PossibleLoadedValues { get; set; }
    public bool AreValuesRandom { get; set; } = false;
    public bool IsLoadedFromJson { get; set; } = false;
    public string? JsonRelativePath { get; set; }
    public SeederDataCreationType DataCreationType { get; set; }
    public override int GetHashCode()
    {
        return PropertyType.GetHashCode() + PropertyName.GetHashCode();
    }
}


