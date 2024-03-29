﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCoreSeeder.Options;

namespace EFCoreSeeder.DI;

public class SeederOptionsProvider : ISeederOptionsProvider
{
    private readonly SeederOptions _options;
    public SeederOptionsProvider(SeederOptions options)
    {
        _options = options;
    }
    public SeederOptions GetOptions()
    {
        return _options;
    }

    public SeederOptions ReconfigureOptions()
    {
        throw new NotImplementedException();
    }
}