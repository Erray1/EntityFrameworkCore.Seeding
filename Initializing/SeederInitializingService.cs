using EFCoreSeeder.DI;
using EFCoreSeeder.Modelling;
using EFCoreSeeder.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Initializing;

public class SeederInitializingService<TDbContext, TSeeder> : IHostedService, ISeederInitializingService
    where TDbContext : DbContext
    where TSeeder : SeederModel<TDbContext>
{
    private readonly SeederOptionsProvider _optionsProvider;
    private readonly SeederModelProvider _modelProvider;
    public SeederInitializingService(IKeyedServiceProvider keyedServiceProvider)
    {
        var seederType = typeof(TSeeder);
        _optionsProvider = keyedServiceProvider.GetRequiredKeyedService<SeederOptionsProvider>(seederType.Name);
        _modelProvider = keyedServiceProvider.GetRequiredKeyedService<SeederModelProvider>(seederType.Name);
    }
    public async Task ExecuteSeeding()
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
