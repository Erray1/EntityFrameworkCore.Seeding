using EntityFrameworkCore.Seeding.Modelling.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Seeding.Modelling;

public sealed class SeederModelBuilder<TDbContext> : ISeederBuilder, ISeederModelBuilder<TDbContext> where TDbContext : DbContext
{
    private readonly SeederModelInfo _model;
    public SeederModelBuilder(SeederModelInfo model)
    {
        _model = model;
    }
    public SeederInfoBase Build()
    {
        return _model;
    }

    public SeederEntityBuilder<TEntity> Entity<TEntity>()
    {
        var entityInfo = getOrCreateEntityInfo<TEntity>();
        return new SeederEntityBuilder<TEntity>(entityInfo);
    }
    public void RandomizeEveryEntity()
    {

    }

    private SeederEntityInfo getOrCreateEntityInfo<TEntity>()
    {
        var trackedEntity = _model.Entities.SingleOrDefault(x => x.EntityType == typeof(TEntity));
        if (trackedEntity is not null) return trackedEntity;
        return createEntityInfo<TEntity>();

    }
    private SeederEntityInfo createEntityInfo<TEntity>()
    {
        var entity = new SeederEntityInfo(typeof(TEntity));
        beginEntityTracking(entity);
        return entity;
    }
    private void beginEntityTracking(SeederEntityInfo entityInfo) 
    { 
        _model.Entities.Add(entityInfo);
    }
}

