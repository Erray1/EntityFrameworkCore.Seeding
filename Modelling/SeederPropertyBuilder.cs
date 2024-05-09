using EntityFrameworkCore.Seeding.StockData;

namespace EntityFrameworkCore.Seeding.Modelling;

public class SeederPropertyBuilder<TProperty>
{
    private readonly SeederPropertyInfo _property;
    public SeederPropertyBuilder(SeederPropertyInfo propertyInfo)
    {
        _property = propertyInfo;
    }
    public SeederPropertyBuilder<TProperty> HasValues<Tin>(IEnumerable<Tin> values) where Tin : class
    {
        _property.PossibleValuesPool = values;
        _property.IsConfigured = true;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasValues<Tin>(string jsonRelativePath)
    {
        _property.IsConfigured = true;
        _property.JsonRelativePath = jsonRelativePath;
        _property.IsLoadedFromJson = true;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasValues(SeederStockDataPropertyCollection values)
    {
        _property.PossibleLoadedValues = values;
        _property.IsConfigured = true;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasRandomValues()
    {
        _property.AreValuesRandom = true;
        _property.IsConfigured = true;
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
    public SeederPropertyBuilder<TProperty> IsIdProperty(bool generateId)
    {
        _property.IsConfigured = true;
        if (generateId)
        {

        }
        
        return this;
    }
}