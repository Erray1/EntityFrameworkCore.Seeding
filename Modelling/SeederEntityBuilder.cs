using EntityFrameworkCore.Seeding.StockData;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Seeding.Modelling;
public sealed class SeederEntityBuilder<TEntity>
    where TEntity : class
{
    private readonly SeederEntityInfo _entity;
    public SeederEntityBuilder(SeederEntityInfo info)
    {
        _entity = info;
    }
    public SeederEntityBuilder<TEntity> HasValues(IEnumerable<TEntity> values) 
    {
        _entity.PossibleValues = values.Cast<object>().ToList(); // А нормально ли это???
        foreach (var prop in _entity.Properties) { prop.IsConfigured = true; }
        return this;
    }

    public SeederEntityBuilder<TEntity> HasValues<Tin>(SeederStockDataCollection values, bool strictPropertyNameMatching = true)
    {
        _entity.LoadsData = true;
        _entity.LoadedValues = values;
        _entity.StrictMatchingForLoadedData = strictPropertyNameMatching;
        values.MarkEveryPropertyMappedToCollectionAsConfigured(_entity.Properties, strictPropertyNameMatching);

        return this;
    }

    public SeederEntityBuilder<TEntity> HasNotRequiredRelationshipProbability<TRelatedEntity>(double probability)
    {
        var relatedEntityInfo = _entity.NullableLinkedEntitiesProbabilities.Keys.Single(x => x.EntityType == typeof(TRelatedEntity));
        _entity.NullableLinkedEntitiesProbabilities[relatedEntityInfo] = probability;
        return this;
    }

    public void HasRandomValues()
    {
        var notConfiguredProperties = getNotConfiguredProperties();

        foreach (var property in notConfiguredProperties)
        {
            property.AreValuesRandom = true;
            property.IsConfigured = true;
        }
        _entity.IsConfigured = true;
    }

    public SeederEntityBuilder<TEntity> TimesCreated(int timesCreated)
    {
        _entity.TimesCreated = timesCreated;
        return this;
    }

    public SeederEntityBuilder<TEntity> TimesCreated(int timesCreated, int locality)
    {
        _entity.TimesCreated = timesCreated;
        _entity.Locality = locality;
        return this;
    }

    public SeederPropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
    {
        var property = propertyExpression.GetPropertyAccess();
        var propertyInfo = _entity.Properties
            .Single(x => x.PropertyName == property.Name &&
                    x.PropertyType == property.PropertyType);
        
        return new SeederPropertyBuilder<TProperty>(propertyInfo);
    }
    

    private List<SeederPropertyInfo> getNotConfiguredProperties()
    {
        return _entity.Properties.Where(x => !x.IsConfigured).ToList();
    }
}

