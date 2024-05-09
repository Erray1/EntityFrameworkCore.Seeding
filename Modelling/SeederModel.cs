using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.Modelling.Validation;
using Microsoft.EntityFrameworkCore;


namespace EntityFrameworkCore.Seeding.Modelling;

public abstract class SeederModel<TDbContext> where TDbContext : DbContext
{
    public SeederModelInfo? Model;
    public virtual void CreateModel(ISeederModelBuilder builder)
    {
        Model = ((SeederModelBuilder)builder).Build();
    }
}
