using EntityFrameworkCore.Seeding.Modelling.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Seeding.Modelling;

public sealed class SeederModelBuilder : ISeederBuilder, ISeederModelBuilder
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

    }
}

