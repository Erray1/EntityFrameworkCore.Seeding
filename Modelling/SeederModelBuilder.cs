using EntityFrameworkCore.Seeding.Modelling.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Seeding.Modelling;

public class SeederModelBuilder : ISeederBuilder, ISeederModelBuilder
{
    private readonly SeederModelInfo _model;
    public SeederModelBuilder(SeederModelInfo model)
    {
        _model = model;
    }
    public SeederModelInfo Build()
    {
        return _model;
    }

    public SeederEntityBuilder<TEntity> Entity<TEntity>()
        where TEntity : class
    {
        var entityInfo = _model.Entities.Single(x =>  x.EntityType == typeof(TEntity));
        return new SeederEntityBuilder<TEntity>(entityInfo);
    }
    public void RandomizeEveryEntity()
    {
        foreach (var entity in _model.Entities)
        {
            entity.TimesCreated = Random.Shared.Next(0, 30);
            foreach (var entityLink in entity.NullableLinkedEntitiesProbabilities)
            {
                entity.NullableLinkedEntitiesProbabilities[entityLink.Key] = Random.Shared.NextDouble();
            }
            foreach (var property in entity.Properties)
            {
                property.AreValuesRandom = true;
                property.IsConfigured = true;
            }
        }
    }
}

