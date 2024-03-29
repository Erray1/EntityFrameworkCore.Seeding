using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Initializing;

public interface ISeederInitializingService
{
    public Task ExecuteSeeding();
}
