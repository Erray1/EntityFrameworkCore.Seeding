using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.Modelling.Validation;
using Microsoft.EntityFrameworkCore;


namespace EntityFrameworkCore.Seeding.Modelling;

public abstract class SeederModel<TDbContext> where TDbContext : DbContext
{
    public SeederModelInfo? Model;
    public virtual void CreateModel(ISeederModelBuilder<TDbContext> builder)
    {
        var model =  (SeederModelInfo)((ISeederBuilder)builder).Build();
        SeederModelValidator.ValidateAndThrowException(model);
        Model = model;
    }
}
