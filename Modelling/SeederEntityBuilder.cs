using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.StockData;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Seeding.Modelling;
public sealed class SeederEntityBuilder<TEntity> : ISeederEntityBuilder
    where TEntity : class
{
    private readonly SeederEntityInfo _entity;
    public SeederEntityBuilder(SeederEntityInfo info)
    {
        _entity = info;
    }
    public SeederEntityBuilder<TEntity> HasValues(IEnumerable<TEntity> values, bool mixValues = false,
        params string[] exceptPropertiesNames) 
    {
        var properties = typeof(TEntity).GetProperties();
        var props = properties
            .Where(prop => values
                .Select(x => prop.GetValue(x))
                .Any(x => x is not null) // check
                && !exceptPropertiesNames.Contains(prop.Name))
            .Select(x => new PropertyInfoAndPool
            {
                PropertyInfo = _entity.Properties
                .First(prop => prop.PropertyName == x.Name
                                && prop.PropertyType == x.PropertyType),
                Pool = values.Select(val => x.GetValue(val)).ToList()!
            })
            .ToList();

        foreach (var prop in props) {
            prop.PropertyInfo.IsConfigured = true;
            prop.PropertyInfo.ShuffleValues = mixValues;
            prop.PropertyInfo.DataCreationType = Core.SeederDataCreationType.FromGivenPool;
        }
        return this;
    }

    class PropertyInfoAndPool
    {
        public SeederPropertyInfo PropertyInfo { get; set; }
        public List<object> Pool { get; set; }
    }

    public SeederEntityBuilder<TEntity> HasValues(SeederStockDataCollection values, bool strictPropertyNameMatching = true)
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

    public SeederEntityBuilder<TEntity> HasEntityConnection<TRelatedEntity>(int timesConnected)
    {
        var relatedEntityInfo = _entity.NullableLinkedEntitiesProbabilities.Keys.Single(x => x.EntityType == typeof(TRelatedEntity));
        _entity.NumberOfBoundEntitiesInOneToManyRelationships[relatedEntityInfo] = timesConnected;
        return this;
    }
    public SeederEntityBuilder<TEntity> HasEntityConnection<TRelatedEntity>(int timesConnected, int locality)
    {
        return HasEntityConnection<TRelatedEntity>(timesConnected + new Random(_entity.GetHashCode()).Next(-locality, locality));
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
        _entity.TimesCreated = timesCreated + new Random(_entity.GetHashCode()).Next(-locality, locality);
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
    public SeederEntityBuilder<TEntity> ShuffleValues()
    {
        _entity.ShuffleValues = true;
        return this;
    }
    
    public SeederEntityBuilder<TEntity> DoNotCreate()
    {
        _entity.DoCreate = false;
        return this;
    }

    private List<SeederPropertyInfo> getNotConfiguredProperties()
    {
        return _entity.Properties.Where(x => !x.IsConfigured).ToList();
    }
}

