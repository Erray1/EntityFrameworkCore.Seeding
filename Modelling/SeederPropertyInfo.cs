using EntityFrameworkCore.Seeding.Core;
using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.StockData;
using System.ComponentModel;

namespace EntityFrameworkCore.Seeding.Modelling;
public class SeederPropertyInfo
{
    public SeederPropertyInfo(Type propertyType, string propertyName)
    {
        PropertyType = propertyType;
        PropertyName = propertyName;
    }
    public bool IsIdProperty { get; set; } = false;
    public bool? GenerateId {  get; set; } 

    private bool isConfigured = false;
    public bool IsConfigured { get { return isConfigured; } set
        {
            if (isConfigured && value)
            {
                Console.WriteLine($"Trying to configure property {PropertyName} multiple times. Configuration will be overriden");
                //throw new WarningException($"Trying to configure property {PropertyName} multiple times. Configuration will be overriden");
            }
            isConfigured = value;
        }
    }
    public Type PropertyType { get; set; }
    public string PropertyName { get; set; }
    public List<object>? PossibleValuesPool { get; set; }
    public SeederStockDataPropertyCollection? PossibleLoadedValues { get; set; }
    public bool AreValuesRandom { get; set; } = false;
    public bool IsLoadedFromJson { get; set; } = false;
    public string? JsonRelativePath { get; set; }
    public bool ShuffleValues { get; set; } = false;
    public SeederDataCreationType DataCreationType { get; set; }
    public override int GetHashCode()
    {
        return PropertyType.GetHashCode() + PropertyName.GetHashCode();
    }
}


