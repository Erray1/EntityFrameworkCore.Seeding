using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Options;

public interface ISeederOptionsProvider
{
    public SeederOptions GetOptions();
    public SeederOptions ReconfigureOptions();
}
