using EFCoreSeeder.Modelling.Utilities;
using Microsoft.EntityFrameworkCore;


namespace EFCoreSeeder.Modelling;

public abstract class SeederModel<TDbContext> where TDbContext : DbContext
{
    public SeederModelInfo? Model;
    public virtual void CreateModel(ISeederModelBuilder<TDbContext> builder)
    {
        Model = (SeederModelInfo)((ISeederBuilderFinalizer)builder).Finalize();
    }
}
