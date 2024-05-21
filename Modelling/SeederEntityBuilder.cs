using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.StockData;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.Linq.Expressions;
using System.Text.Json;

namespace EntityFrameworkCore.Seeding.Modelling;
public sealed class SeederEntityBuilder<TEntity> : ISeederEntityBuilder
    where TEntity : class
{
    private readonly SeederModelInfo _model;
    private readonly SeederEntityInfo _entity;
    public SeederEntityBuilder(
        SeederEntityInfo entity,
        SeederModelInfo model)
    {
        _entity = entity;
        _model = model;
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
        throw new NotImplementedException();
        _entity.LoadsData = true;
        _entity.LoadedValues = values;
        _entity.StrictMatchingForLoadedData = strictPropertyNameMatching;
        values.MarkEveryPropertyMappedToCollectionAsConfigured(_entity.Properties, strictPropertyNameMatching);

        return this;
    }
    public SeederJsonCreationBuilder<TEntity> HasValues(string jsonAbsolutePath, JsonSerializerOptions? options = null)
    {
        throw new NotImplementedException();
        if (options is null) options = JsonSerializerOptions.Default;
        EntityCreatedFromJsonInfo jsonInfo = new(jsonAbsolutePath, options);
        _entity.JSONInfo.Add(jsonInfo);
        return new SeederJsonCreationBuilder<TEntity>(_entity, jsonInfo);
    }

    public SeederEntityBuilder<TEntity> HasNotRequiredRelationshipProbability<TRelatedEntity>(double probability)
    {
        var relatedEntityInfo = _model.Entities.Single(x => x.EntityType == typeof(TRelatedEntity));
        var relation = _model.Relations.Single(x => x.DependentEntityInfo.EntityType == relatedEntityInfo.EntityType);
        relation.BindProbability = probability;
        return this;
    }

    public SeederEntityBuilder<TEntity> HasNumberOfConnectionsInManyToMany<TRelatedEntity>(int timesConnected, int locality = 0)
    {

        if (timesConnected <= 0)
        {
            throw new ArgumentException("Number of connections cannot be <= 0.\n If you want to avoid connections, exclude entity from model using DoNotCreate() method");
        }
        if (locality < 0)
        {
            throw new ArgumentException("Locality cannot be negative");
        }
        var relatedEntityInfo = _model.Entities.Single(x => x.EntityType == typeof(TRelatedEntity));
        var relation = _model.ManyToManyRelations.Single(x => x.Compare(_entity, relatedEntityInfo));
        relation.SetNumberOfBoundEntitiesFor(_entity, timesConnected);
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
        foreach (var property in _entity.Properties)
        {
            property.ShuffleValues = true;
        }
        return this;
    }
    
    public SeederEntityBuilder<TEntity> DoNotCreate()
    {
        var relationsToRemove = _model.Relations
            .Where(x => x.PrincipalEntityInfo == _entity
            || x.DependentEntityInfo == _entity);
        foreach (var relation  in relationsToRemove)
        {
            _model.Relations.Remove(relation);
        }

        var manyToManyRelationsToRemove = _model.ManyToManyRelations
            .Where(x => x.LeftEntityInfo == _entity
            || x.RightEntityInfo == _entity);
        foreach (var relation in manyToManyRelationsToRemove)
        {
            _model.ManyToManyRelations.Remove(relation);
        }

        _model.Entities.Remove(_entity);
        _entity.DoCreate = false;

        return this;
    }

    private List<SeederPropertyInfo> getNotConfiguredProperties()
    {
        return _entity.Properties.Where(x => !x.IsConfigured).ToList();
    }
}

