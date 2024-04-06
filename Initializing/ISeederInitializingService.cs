using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Initializing;

public interface ISeederInitializingService
{
    public Task ExecuteSeeding();
}
