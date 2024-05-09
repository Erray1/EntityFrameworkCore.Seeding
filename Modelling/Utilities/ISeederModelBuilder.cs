using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Seeding.Modelling.Utilities;

public interface ISeederModelBuilder
{
    public SeederEntityBuilder<TEntity> Entity<TEntity>() where TEntity : class;
}