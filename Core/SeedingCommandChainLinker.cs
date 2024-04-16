using EntityFrameworkCore.Seeding.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
public class SeedingCommandChainLinker
{
    private InternalSeedingCommandChainLinker internalLinker;

    public ISeedingHandler Default()
    {
        return internalLinker.AddSeedingHandler()
            .AddHistoryHandler()
            .AddLoggingHandler()
            .Build();
    }
    public ISeedingHandler FromOptions(SeederOptions options)
    {
        throw new NotImplementedException();
    }
}

