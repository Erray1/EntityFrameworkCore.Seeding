using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder;

public interface ISeederService : IDisposable
{
    public Task StartSeedingAsync();
    public void StartSeeding();
}
