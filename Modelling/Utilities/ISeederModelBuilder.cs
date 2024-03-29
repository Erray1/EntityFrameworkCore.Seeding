using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Modelling.Utilities;

public interface ISeederModelBuilder<TDbContext> where TDbContext : DbContext
{
    public SeederEntityBuilder<TEntity> Entity<TEntity>(Expression<Func<TDbContext, TEntity>> keyExpression);
}