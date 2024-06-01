using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.StockData;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.Linq.Expressions;
using System.Text.Json;

namespace EntityFrameworkCore.Seeding.Modelling;
/// <summary>
/// Main class for configuring seeder entity
/// </summary>
/// <typeparam name="TEntity">Type of entity being configured</typeparam>
public sealed class SeederEntityBuilder<TEntity>
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
    /// <summary>
    ///     Sets values of which the data will be generated
    /// </summary>
    /// <remarks>
    ///     Uses only those properties that have a value in each instance
    /// </remarks>
    /// <param name="values"></param>
    /// <param name="exceptPropertiesNames">Properties that will be excluded from pool</param>
    /// <returns>Entity builder for further configuration</returns>
    public SeederEntityBuilder<TEntity> HasValues(IEnumerable<TEntity> values,
        string[]? exceptPropertiesNames = null) 
    {
        if (exceptPropertiesNames is null)
        {
            exceptPropertiesNames = [];
        }
        var properties = typeof(TEntity).GetProperties();
        var props = properties
            .Where(prop => values
                .Select(x => prop.GetValue(x))
                .Any(x => x is not null)
                && !exceptPropertiesNames.Contains(prop.Name))
            .Select(x => new
            {
                PropertyInfo = _entity.Properties
                .First(prop => prop.PropertyName == x.Name
                                && prop.PropertyType == x.PropertyType),
                Pool = values.Select(val => x.GetValue(val)).ToList()!
            })
            .ToList();

        foreach (var prop in props) {
            prop.PropertyInfo.IsConfigured = true;
            prop.PropertyInfo.DataCreationType = Core.SeederDataCreationType.FromGivenPool;
        }
        return this;
    }

    /// <summary>
    ///     Data will be generated from values of chosen collection
    /// </summary>
    /// <param name="values">Stock collection</param>
    /// <returns>Entity builder for further configuration</returns>
    /// <exception cref="NotImplementedException"></exception>
    public SeederStockDataEntityBuilder<TEntity> HasValues(SeederStockDataEntityCollection values)
    {
        throw new NotImplementedException();
        //return new SeederStockDataEntityBuilder<TEntity>(_entity, values);
    }

    /// <summary>
    ///     Data will be generated from json
    /// </summary>
    /// <param name="jsonAbsolutePath"></param>
    /// <param name="options"></param>
    /// <returns>Builder for json configuration</returns>
    /// <exception cref="NotImplementedException"></exception>
    public SeederJsonCreationBuilder<TEntity> HasValues(string jsonAbsolutePath, JsonSerializerOptions? options = null)
    {
        throw new NotImplementedException();
        //if (options is null) options = JsonSerializerOptions.Default;
        //EntityCreatedFromJsonInfo jsonInfo = new(jsonAbsolutePath, options);
        //_entity.JSONInfo.Add(jsonInfo);
        //return new SeederJsonCreationBuilder<TEntity>(_entity, jsonInfo);
    }

    /// <summary>
    ///     Sets probability of nullable relationship. Used in 1:1 and 1:N on dependent side
    /// </summary>
    /// <typeparam name="TRelatedEntity">Type of related entity</typeparam>
    /// <param name="probability"></param>
    /// <returns>Entity builder for further configuration</returns>
    public SeederEntityBuilder<TEntity> HasNotRequiredRelationshipProbability<TRelatedEntity>(double probability)
    {
        var relatedEntityInfo = _model.Entities.Single(x => x.EntityType == typeof(TRelatedEntity));
        var relation = _model.Relations.Single(x => x.DependentEntityInfo.EntityType == relatedEntityInfo.EntityType);
        relation.BindProbability = probability;
        return this;
    }

    /// <summary>
    ///     Sets number of connections in N:N relationships
    /// </summary>
    /// <typeparam name="TRelatedEntity">Type of related entities</typeparam>
    /// <param name="timesConnected">Number of connections</param>
    /// <param name="locality">Offset</param>
    /// <returns>Entity builder for further configuration</returns>
    /// <exception cref="ArgumentException"></exception>
    public SeederEntityBuilder<TEntity> HasNumberOfConnectionsInManyToMany<TRelatedEntity>(int timesConnected, int locality = 0)
    {

        if (timesConnected - locality <= 0)
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

    /// <summary>
    ///     Data of non-configured properties will be generated from random pool
    /// </summary>
    public void HasRandomValues()
    {
        var notConfiguredProperties = getNotConfiguredProperties();

        foreach (var property in notConfiguredProperties)
        {
            property.AreValuesRandom = true;
            property.DataCreationType = Core.SeederDataCreationType.Random;
            property.IsConfigured = true;
        }
        _entity.IsConfigured = true;
    }

    /// <summary>
    ///     Sets number of created instances
    /// </summary>
    /// <param name="timesCreated">Number of created instances</param>
    /// <returns>Entity builder for further configuration</returns>
    public SeederEntityBuilder<TEntity> TimesCreated(int timesCreated)
    {
        _entity.TimesCreated = timesCreated;
        return this;
    }

    /// <summary>
    ///     Sets number of created instances with offset
    /// </summary>
    /// <param name="timesCreated">Number of created instances</param>
    /// <param name="locality">Offset</param>
    /// <returns>Entity builder for further configuration</returns>
    public SeederEntityBuilder<TEntity> TimesCreated(int timesCreated, int locality)
    {
        _entity.TimesCreated = timesCreated + new Random(_entity.GetHashCode()).Next(-locality, locality);
        return this;
    }

    /// <summary>
    ///     Gets builder for configations of entity`s property
    /// </summary>
    /// <remarks>
    ///     Do not configure properties which are used as principal or foreign key or collection of keys in relationships
    ///     These properties are excluded from model for later binding
    /// </remarks>
    /// <typeparam name="TProperty">Type of property being configured</typeparam>
    /// <param name="propertyExpression">Expression of property</param>
    /// <returns>Entity builder for further configuration</returns>
    public SeederPropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
    {
        var property = propertyExpression.GetPropertyAccess();
        var propertyInfo = _entity.Properties
            .Single(x => x.PropertyName == property.Name &&
                    x.PropertyType == property.PropertyType);
        
        return new SeederPropertyBuilder<TProperty>(propertyInfo);
    }
    /// <summary>
    ///     Shuffles values of entities
    /// </summary>
    /// <remarks>
    ///     Used in combination with HasValues(IEnumerable<TEntity> entities) method
    /// </remarks>
    /// <returns>Entity builder for further configuration</returns>
    public SeederEntityBuilder<TEntity> ShuffleValues()
    {
        _entity.ShuffleValues = true;
        foreach (var property in _entity.Properties)
        {
            property.ShuffleValues = true;
        }
        return this;
    }
    
    /// <summary>
    ///     Excludes entity from model
    /// </summary>
    /// <remarks>
    ///     Entity is exluded from creation and from all relationships
    /// </remarks>
    /// <returns></returns>
    public void DoNotCreate()
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
    }

    private List<SeederPropertyInfo> getNotConfiguredProperties()
    {
        return _entity.Properties.Where(x => !x.IsConfigured).ToList();
    }
}

