using EFCoreSeeder.Modelling.Utilities;
using EFCoreSeeder.Reload;
using EFCoreSeeder.StockData;
using System.Linq.Expressions;

namespace EFCoreSeeder.Modelling;
public class SeederEntityBuilder<TEntity>
{
    public SeederEntityInfo Entity { get; set; }
    public SeederEntityBuilder(SeederEntityInfo info)
    {
        Entity = info;
    }
    public SeederEntityBuilder<TEntity> HasValues<Tin>(IEnumerable<Tin> values) where Tin : class
    {
        return this;
    }
    public SeederEntityBuilder<TEntity> HasValues<Tin>(SeederStockDataCollection<Tin> values)
    {
        return this;
    }
    public SeederEntityBuilder<TEntity> HasNotRequiredRelationshipProbability<TRelatedEntity>(Expression<Func<TEntity, TRelatedEntity>> keyExpression)
    {
        return this;
    }
    public void HasRandomValues()
    {
        var allProperties = typeof(TEntity)
            .GetProperties()
            .Select(prop => prop.Name)
            .ToList();

        var notConfiguredProperties = allProperties
            .Where(x => Entity.Properties
                .Select(prop => prop.PropertyName)
                .Contains(x))
            .Select(x => createPropertyInfo(x))
            .ToList();

        foreach (var property in notConfiguredProperties)
        {
            property.AreValuesRandom = true;
            property.IsConfigured = true;
        }
        Entity.IsConfigured = true;
            
    }
    public SeederEntityBuilder<TEntity> HasRefreshBehaviour(RefreshBehaviours behaviour)
    {
        Entity.OverrideRefreshBehaviour = behaviour;
        return this;
    }
    public SeederEntityBuilder<TEntity> TimesCreated(int timesCreated)

    {
        Entity.TimesCreated = timesCreated;
        return this;
    }
    public SeederEntityBuilder<TEntity> TimesCreated(int timesCreated, int locality)
    {
        Entity.TimesCreated += timesCreated;
        Entity.Locality = locality;
        return this;
    }
    public SeederPropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> keyExpression)
    {
        var propertyInfo = getOrCreatePropertyInfo(keyExpression);

        beginPropertyTracking(propertyInfo);
        
        return new SeederPropertyBuilder<TProperty>(propertyInfo);
    }
    public void FillOtherPropertiesWithRandomValues()
    {
        var allProperties = typeof(TEntity)
            .GetProperties()
            .Select(prop => prop.Name)
            .ToList();

        var notConfiguredProperties = allProperties
            .Where(x => Entity.Properties
                .Select(prop => prop.PropertyName)
                .Contains(x))
            .Select(x => createPropertyInfo(x))
            .ToList();

        foreach (var property in notConfiguredProperties)
        {
            property.AreValuesRandom = true;
            property.IsConfigured = true;
        }
        Entity.IsConfigured = true;
    }
    private SeederPropertyInfo getOrCreatePropertyInfo<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
    {
        var propertyName = nameof(TProperty); // ЗАМЕНИТЬ
        var trackedProperty = Entity.Properties.SingleOrDefault(p => p.PropertyName == propertyName);
        if (trackedProperty is not null) return trackedProperty;
        return new(nameof(TProperty));
    }
    private SeederPropertyInfo createPropertyInfo(string propertyName)
    {
        var property = new SeederPropertyInfo(propertyName);

        beginPropertyTracking(property);
        return property;    
    }
    private void beginPropertyTracking(SeederPropertyInfo propertyInfo)
    {
        Entity.Properties.Add(propertyInfo);
    }

    public object Build()
    {
        throw new NotImplementedException();
    }
}

