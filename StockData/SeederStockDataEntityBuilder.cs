using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.StockData;
public class SeederStockDataEntityBuilder<TEntity>
{
    private readonly SeederEntityInfo _entity;
    private readonly SeederStockDataEntityCollection _data;
    public SeederStockDataEntityBuilder(
        SeederEntityInfo entity,
        SeederStockDataEntityCollection data)
    {
        _entity = entity;
        _data = data;
    }
    private SeederStockDataPropertyCollection[] getAllPropertiesInCollection()
    {
        var properties = _data.GetType()
            .GetProperties()
            .Where(x => x.PropertyType.IsAssignableTo(typeof(SeederStockDataPropertyCollection)))
            .Select(x => x.GetValue(_data))
            .Cast<SeederStockDataPropertyCollection>()
            .ToArray();
        return properties;
    }
    public SeederStockDataEntityBuilder<TEntity> MapAllProperties(bool strictPropertyMatching = true)
    {
        var properties = getAllPropertiesInCollection();
        foreach (var propertyCollection in properties)
        {
            var bestMatchingPropertyInfo = _entity.Properties
                .Where(x => x.PropertyType == propertyCollection.PropertyType)
                .FirstOrDefault(x => strictPropertyMatching ? propertyCollection.PropertyName == x.PropertyName
                                    : propertyCollection.PropertyName.Contains(x.PropertyName));
            if (bestMatchingPropertyInfo is null)
            {
                if (!strictPropertyMatching) continue;
                throw new InvalidOperationException($"Cannot find property with name {propertyCollection.PropertyName} in entity {_entity.EntityType.Name}");
            }

            bestMatchingPropertyInfo.IsLoaded = true;
            bestMatchingPropertyInfo.DataCreationType = Core.SeederDataCreationType.Loaded;
            bestMatchingPropertyInfo.IsConfigured = true;
            bestMatchingPropertyInfo.PropertyStockCollection = propertyCollection;
        }
        return this;
    }
    public SeederStockDataEntityBuilder<TEntity> MapProperty<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression, SeederStockDataPropertyCollection propertyCollection)
    {
        var propertyInfo = propertyExpression.GetPropertyAccess();
        SeederPropertyInfo property = _entity.Properties.Single(
            x => x.PropertyName == propertyInfo.Name &&
            x.PropertyType == propertyInfo.PropertyType
            );

        property.IsLoaded = true;
        property.DataCreationType = Core.SeederDataCreationType.Loaded;
        property.IsConfigured = true;
        property.PropertyStockCollection = propertyCollection;

        return this;
    }
}

