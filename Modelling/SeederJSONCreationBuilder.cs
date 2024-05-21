using EntityFrameworkCore.Seeding.Modelling.Utilities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling;
public sealed class SeederJsonCreationBuilder<TEntity>
{
    private readonly SeederEntityInfo _entity;
    private readonly EntityCreatedFromJsonInfo _jsonInfo;
    public SeederJsonCreationBuilder(
        SeederEntityInfo entityInfo,
        EntityCreatedFromJsonInfo jsonInfo)
    {
        _entity = entityInfo;
        _jsonInfo = jsonInfo;
    }
    public SeederJsonCreationBuilder<TEntity> WithProperty<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression, string? customPropertyNameInJson = null)
    {
        var property = propertyExpression.GetPropertyAccess();
        var propertyInfo = _entity.Properties
            .Single(x => x.PropertyName == property.Name &&
                    x.PropertyType == property.PropertyType);

        propertyInfo.IsConfigured = true;
        propertyInfo.DataCreationType = Core.SeederDataCreationType.FromJSON;
        propertyInfo.JsonInfo = _jsonInfo;

        string propertyName = customPropertyNameInJson is null ? Char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1)
            : customPropertyNameInJson;
        _jsonInfo.JSONNamesAndProperties.Add(propertyInfo, propertyName);
        return this;
    }
}

