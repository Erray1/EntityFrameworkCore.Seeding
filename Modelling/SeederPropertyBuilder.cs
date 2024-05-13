using EntityFrameworkCore.Seeding.StockData;

namespace EntityFrameworkCore.Seeding.Modelling;

public class SeederPropertyBuilder<TProperty>
{
    private readonly SeederPropertyInfo _property;
    public SeederPropertyBuilder(SeederPropertyInfo propertyInfo)
    {
        _property = propertyInfo;
    }
    public SeederPropertyBuilder<TProperty> HasValues(IEnumerable<TProperty> values)
    {
        _property.PossibleValuesPool = values.Cast<object>().ToList();
        _property.IsConfigured = true;
        _property.DataCreationType = Core.SeederDataCreationType.FromGivenPool;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasValues(string jsonRelativePath)
    {
        _property.IsConfigured = true;
        _property.JsonRelativePath = jsonRelativePath;
        _property.IsLoadedFromJson = true;
        _property.DataCreationType = Core.SeederDataCreationType.FromJSON;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasValues(SeederStockDataPropertyCollection values)
    {
        _property.PossibleLoadedValues = values;
        _property.IsConfigured = true;
        _property.DataCreationType = Core.SeederDataCreationType.Loaded;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasRandomValues()
    {
        _property.AreValuesRandom = true;
        _property.IsConfigured = true;
        _property.DataCreationType = Core.SeederDataCreationType.Random;
        return this;
    }
    public SeederPropertyBuilder<TProperty> IsDefault()
    {
        _property.IsConfigured = true;
        object? defaultValue = _property.PropertyType.IsValueType ? Activator.CreateInstance(_property.PropertyType) : null;
        List<object> values = [defaultValue!];
        _property.PossibleValuesPool = values;
        return this;
    }
    public SeederPropertyBuilder<TProperty> IsIdProperty(bool generateId = false)
    {
        
        _property.IsIdProperty = true;
        _property.IsConfigured = true;
        if (!generateId)
        {
            _property.DataCreationType = Core.SeederDataCreationType.DoNotCreate;
            _property.GenerateId = true;
            return this;
        }
        _property.DataCreationType = Core.SeederDataCreationType.CreatedID;
        return this;
    }
    public SeederPropertyBuilder<TProperty> DoNotCreate()
    {
        _property.IsConfigured = true;
        _property.DataCreationType = Core.SeederDataCreationType.DoNotCreate;
        return this;
    }
}