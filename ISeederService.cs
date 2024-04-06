using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding;

public interface ISeederService : IDisposable
{
    public Task StartSeedingAsync();
    public void StartSeeding();
}
