using EntityFrameworkCore.Seeding.StockData;

namespace EntityFrameworkCore.Seeding.Modelling;

/// <summary>
/// Main class for configuring seeder property
/// </summary>
/// <typeparam name="TProperty">Type of property being configured</typeparam>
public class SeederPropertyBuilder<TProperty>
{
    private readonly SeederPropertyInfo _property;
    public SeederPropertyBuilder(SeederPropertyInfo propertyInfo)
    {
        _property = propertyInfo;
    }
    /// <summary>
    ///     Sets values of which property will be created
    /// </summary>
    /// <param name="values"></param>
    /// <returns>Property builder for further configuration</returns>
    public SeederPropertyBuilder<TProperty> HasValues(IEnumerable<TProperty> values)
    {
        _property.PossibleValuesPool = values.Cast<object>().ToList();
        _property.IsConfigured = true;
        _property.DataCreationType = Core.SeederDataCreationType.FromGivenPool;
        return this;
    }
    /// <summary>
    ///     Property will be created from json
    /// </summary>
    /// <param name="jsonAbsolutePath"></param>
    /// <returns>Property builder for further configuration</returns>
    public SeederPropertyBuilder<TProperty> HasValues(string jsonAbsolutePath)
    {
        throw new NotImplementedException();
        //_property.IsConfigured = true;
        //_property.DataCreationType = Core.SeederDataCreationType.FromJSON;
        //return this;
    }
    /// <summary>
    ///     Property will be created from random values
    /// </summary>
    /// <returns></returns>
    public SeederPropertyBuilder<TProperty> HasRandomValues()
    {
        _property.AreValuesRandom = true;
        _property.IsConfigured = true;
        _property.DataCreationType = Core.SeederDataCreationType.Random;
        return this;
    }
    /// <summary>
    ///     Value of property is default
    /// </summary>
    /// <returns></returns>
    public SeederPropertyBuilder<TProperty> IsDefault()
    {
        _property.IsConfigured = true;
        object? defaultValue = _property.PropertyType.IsValueType ? Activator.CreateInstance(_property.PropertyType) : null;
        List<object> values = [defaultValue!];
        _property.PossibleValuesPool = values;
        return this;
    }

    /// <summary>
    ///     Pool of property`s values will be shuffled
    /// </summary>
    /// <returns></returns>
    public SeederPropertyBuilder<TProperty> ShuffleValues()
    {
        _property.ShuffleValues = true;
        return this;
    }
    
    /// <summary>
    ///     Excludes property from model
    /// </summary>
    /// <returns></returns>
    public SeederPropertyBuilder<TProperty> DoNotCreate()
    {
        _property.IsConfigured = true;
        _property.DataCreationType = Core.SeederDataCreationType.DoNotCreate;
        return this;
    }
}