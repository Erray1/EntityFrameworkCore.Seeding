using EFCoreSeeder.Modelling.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFCoreSeeder.Modelling;

public sealed class SeederModelBuilder<TDbContext> : ISeederBuilderFinalizer, ISeederModelBuilder<TDbContext>, IDisposable where TDbContext : DbContext
{
    private readonly SeederModelInfo _model;
    public SeederModelBuilder(SeederModelInfo model)
    {
        _model = model;
    }
    public SeederInfoBase Finalize()
    {
        // Set dispose flag
        return _model;
    }

    public SeederEntityBuilder<TEntity> Entity<TEntity>(Expression<Func<TDbContext, TEntity>> keyExpression)
    {
        var entityInfo = getOrCreateEntityInfo(keyExpression);
        return new SeederEntityBuilder<TEntity>(entityInfo);
    }

    private SeederEntityInfo getOrCreateEntityInfo<TEntity>(Expression<Func<TDbContext, TEntity>> keyExpression)
    {
        throw new NotImplementedException();
    }
    private void beginEntityTracking(SeederEntityInfo entityInfo) 
    { 
        
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}