using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Options;

public interface ISeederOptionsProvider
{
    /// <summary>
    ///     Gets a copy of configured options
    /// </summary>
    /// <returns>
    ///     A copy of options
    /// </returns>
    public SeederOptions GetOptions();

    /// <summary>
    ///     Reconfigures options for certain seeder
    /// </summary>
    /// <returns>SeederOptionsBuilder for further configuration</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ISeederOptionsBuilder ReconfigureOptions();
}
