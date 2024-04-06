using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Seeding.Modelling.Utilities;

public interface ISeederModelBuilder<TDbContext> where TDbContext : DbContext
{
    public SeederEntityBuilder<TEntity> Entity<TEntity>();
}