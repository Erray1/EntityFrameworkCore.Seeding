using EntityFrameworkCore.Seeding.StockData;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Seeding.Modelling;
public class SeederEntityBuilder<TEntity>
    where TEntity : class
{
    public SeederEntityInfo Entity { get; set; }
    public SeederEntityBuilder(SeederEntityInfo info)
    {
        Entity = info;
    }
    public SeederEntityBuilder<TEntity> HasValues(List<TEntity> values) 
    {
        Entity.PossibleValues = values.Cast<object>().ToList(); // А нормально ли это???
        return this;
    }

    public SeederEntityBuilder<TEntity> HasValues<Tin>(SeederStockDataCollection values, bool strictPropertyMatching = true)
    {
        Entity.LoadsData = true;
        Entity.LoadedValues = values;
        Entity.StrictMatchingForLoadedData = strictPropertyMatching;
        return this;
    }

    public SeederEntityBuilder<TEntity> HasNotRequiredRelationshipProbability<TRelatedEntity>(int probability)
    {
        var relatedEntityInfo = Entity.NullableLinkedEntitiesProbabilities.Keys.Single(x => x.EntityType == typeof(TRelatedEntity));
        Entity.NullableLinkedEntitiesProbabilities[relatedEntityInfo] = probability;
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
        Entity.IsConfigured = true;
    }

    public SeederEntityBuilder<TEntity> TimesCreated(int timesCreated)
    {
        Entity.TimesCreated = timesCreated;
        return this;
    }

    public SeederEntityBuilder<TEntity> TimesCreated(int timesCreated, int locality)
    {
        Entity.TimesCreated = timesCreated;
        Entity.Locality = locality;
        return this;
    }

    public SeederPropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> keyExpression)
    {
        var propertyInfo = getPropertyInfo(keyExpression);
        
        return new SeederPropertyBuilder<TProperty>(propertyInfo);
    }
    
    private SeederPropertyInfo getPropertyInfo<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
    {
        throw new NotImplementedException();

    }

    private List<SeederPropertyInfo> getNotConfiguredProperties()
    {
        return Entity.Properties.Where(x => !x.IsConfigured).ToList();
    }
}

