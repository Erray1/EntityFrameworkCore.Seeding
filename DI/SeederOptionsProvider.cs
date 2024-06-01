using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Seeding.Options;

namespace EntityFrameworkCore.Seeding.DI;

public class SeederOptionsProvider : ISeederOptionsProvider
{
    private SeederOptions _options;
    public SeederOptionsProvider(SeederOptions options)
    {
        _options = options;
    }

    /// <summary>
    ///     Gets a copy of configured options
    /// </summary>
    /// <returns>
    ///     A copy of options
    /// </returns>
    public SeederOptions GetOptions()
    {
        SeederOptions optionsCopy = new SeederOptions();
        var properties = typeof(SeederOptions).GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(_options);
            property.SetValue(optionsCopy, value, null);
        }
        return optionsCopy;
    }

    /// <summary>
    ///     Reconfigures options for certain seeder
    /// </summary>
    /// <returns>SeederOptionsBuilder for further configuration</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ISeederOptionsBuilder ReconfigureOptions()
    {
        if (!_options.AllowOptionsChanging)
        {
            throw new InvalidOperationException("You cannot change options in runtime." +
                " If you want to do this, call AllowOptionsReconfiguringInRuntime() on ISeederOptionsBuilder in bootup");
        }

        var newOptions = new SeederOptions();
        _options = newOptions;
        return new SeederOptionsBuilder(newOptions);
    }
}
