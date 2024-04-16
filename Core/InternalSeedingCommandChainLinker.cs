using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
public class InternalSeedingCommandChainLinker
{
    private ISeedingHandler currentHandler;
    public ISeedingHandler Build() => currentHandler;
    public InternalSeedingCommandChainLinker AddLoggingHandler()
    {
        return this;
    }
    public InternalSeedingCommandChainLinker AddSeedingHandler()
    {
        return this;
    }
    public InternalSeedingCommandChainLinker AddHistoryHandler()
    {
        return this;
    }
    public InternalSeedingCommandChainLinker AddDisposeHandler()
    {
        return this;
    }
}

