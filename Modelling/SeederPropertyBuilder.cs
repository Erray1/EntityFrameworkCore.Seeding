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
        _property.PossibleValues = values;
        _property.IsConfigured = true;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasValues<Tin>(string jsonRelativePath)
    {
        _property.IsConfigured = true;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasValues(SeederStockDataCollection values)
    {
        _property.PossibleValues = values;
        _property.IsConfigured = true;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasRandomValues()
    {
        _property.AreValuesRandom = true;
        _property.IsConfigured = true;
        return this;
    }
}