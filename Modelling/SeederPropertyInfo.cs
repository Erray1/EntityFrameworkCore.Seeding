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
    public bool AreValuesRandom { get; set; } = false;
    public EntityCreatedFromJsonInfo? JsonInfo { get; set; }
    public bool ShuffleValues { get; set; } = false;
    public bool IsLoaded { get; set; } = false;
    public SeederStockDataPropertyCollection? PropertyStockCollection { get; set; }

    private SeederDataCreationType dataCreationType;
    public SeederDataCreationType DataCreationType { 
        get
        {
            return dataCreationType;
        }
        set
        {
            if (value == SeederDataCreationType.CreatedID)
            {
                ShuffleValues = false;
            }
            dataCreationType = value;
        }
    }
    public override int GetHashCode()
    {
        return PropertyType.GetHashCode() + PropertyName.GetHashCode();
    }
}


