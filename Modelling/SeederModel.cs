using EntityFrameworkCore.Seeding.Modelling.Utilities;
using EntityFrameworkCore.Seeding.Modelling.Validation;
using Microsoft.EntityFrameworkCore;


namespace EntityFrameworkCore.Seeding.Modelling;

/// <summary>
///     Base class for seeder`s model configuration
/// </summary>
/// <typeparam name="TDbContext">Type of DbContext used for this seeder</typeparam>
public abstract class SeederModel<TDbContext> where TDbContext : DbContext
{
    public SeederModelInfo? Model;

    /// <summary>
    ///     Base method for seeder`s model configuration
    /// </summary>
    /// <param name="builder">Model builder interface for model configuring</param>
    public abstract void ConfigureModel(SeederModelBuilder builder);
}
