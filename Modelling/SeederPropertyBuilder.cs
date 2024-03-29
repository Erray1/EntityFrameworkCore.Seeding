using EFCoreSeeder.Modelling.Utilities;
using EFCoreSeeder.StockData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Modelling;

public class SeederPropertyBuilder<TProperty>
{
    private readonly SeederPropertyInfo _property;
    public SeederPropertyBuilder(SeederPropertyInfo propertyInfo)
    {
        _property = propertyInfo;
    }
    public SeederPropertyBuilder<TProperty> HasValues<Tin>(IEnumerable<Tin> values) where Tin : class
    {
        tryThrowAlreadyConfiguredException();
        _property.PossibleValues = values;
        _property.IsConfigured = true;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasValues<Tin>(string filePath)
    {
        tryThrowAlreadyConfiguredException();
        _property.IsConfigured = true;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasValues<Tin>(SeederStockDataCollection<Tin> values)
    {
        tryThrowAlreadyConfiguredException();
        _property.PossibleValues = values;
        _property.IsConfigured = true;
        return this;
    }
    public SeederPropertyBuilder<TProperty> HasRandomValues()
    {
        tryThrowAlreadyConfiguredException();
        _property.AreValuesRandom = true;
        _property.IsConfigured = true;
        return this;
    }
    private void tryThrowAlreadyConfiguredException()
    {
        if (_property.IsConfigured)
        {
            throw new Exception($"The property {_property.PropertyName} has already been configured");
        }
    }
}