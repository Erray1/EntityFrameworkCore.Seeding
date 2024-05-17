using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using EntityFrameworkCore.Seeding.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;

namespace EntityFrameworkCore.Seeding.Core;
public class SeederEntityAdder<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SeederOptions _options;
    private readonly SeederModelInfo _model;
    public SeederEntityAdder(
        IServiceProvider serviceProvider
        )
    {
        _serviceProvider = serviceProvider;
        _options = serviceProvider.GetKeyedService<SeederOptionsProvider>(typeof(TDbContext).Name)!.GetOptions();
    }
    public async Task AddEntities(Dictionary<SeederEntityInfo, List<object>> entities)
    {
        
    }
}

