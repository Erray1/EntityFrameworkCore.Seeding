using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EntityFrameworkCore.Seeding.Core;
public class SeederInitialStarter<TSeederModel, TDbContext> : BackgroundService
    where TDbContext : DbContext
    where TSeederModel : SeederModel<TDbContext>
{
    private readonly IServiceProvider _serviceProvider;
    public SeederInitialStarter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateAsyncScope())
        {
            var seeder = (Seeder<TSeederModel, TDbContext>)scope.ServiceProvider.GetRequiredKeyedService(typeof(Seeder<TSeederModel, TDbContext>), typeof(TDbContext).Name);
            await seeder.ExecuteSeedingAsync(stoppingToken);
            await StopAsync(stoppingToken);
        }
        
    }
}

