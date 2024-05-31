using EntityFrameworkCore.Seeding.Modelling.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Seeding.Modelling;

/// <summary>
/// Main class for building seeder model
/// </summary>
public class SeederModelBuilder 
{ 
    private readonly SeederModelInfo _model;
    public SeederModelBuilder(SeederModelInfo model)
    {
        _model = model;
    }

    /// <summary>
    ///     Method for entity configuring
    /// </summary>
    /// <typeparam name="TEntity">Entity being configured</typeparam>
    /// <returns>Builder for entity configuration</returns>
    public SeederEntityBuilder<TEntity> Entity<TEntity>()
        where TEntity : class
    {
        var entityInfo = _model.Entities.Single(x =>  x.EntityType == typeof(TEntity));
        return new SeederEntityBuilder<TEntity>(entityInfo, _model);
    }

    /// <summary>
    /// Randomizes configuration
    /// </summary>
    public void RandomizeConfiguration()
    {
        foreach (var entity in _model.Entities)
        {
            entity.TimesCreated = Random.Shared.Next(0, 30);
            //foreach (var entityLink in entity.NullableLinkedEntitiesProbabilities)
            //{
            //    entity.NullableLinkedEntitiesProbabilities[entityLink.Key] = Random.Shared.NextDouble();
            //}
            foreach (var property in entity.Properties)
            {
                property.AreValuesRandom = true;
                property.IsConfigured = true;
            }
        }
    }
}

