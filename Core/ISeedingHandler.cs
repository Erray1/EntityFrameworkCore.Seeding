using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
public interface ISeedingHandler
{
    public Task ExecuteAsync();
    public Task ExecuteNextStepAsync(ISeedingContext? context);
}

