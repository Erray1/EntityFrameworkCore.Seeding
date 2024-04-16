using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
public class SeedingLoggingHandler : ISeedingHandler
{
    public Task ExecuteAsync()
    {
        throw new NotImplementedException();
    }

    public Task ExecuteNextStepAsync(ISeedingContext? context)
    {
        throw new NotImplementedException();
    }
}

