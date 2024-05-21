﻿using EntityFrameworkCore.Seeding.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Creation.CreationPolicies;
public class PropertiesLoadedFromJsonPolicy : SeederPropertiesCreationPolicy
{
    protected override void createPropertiesPool()
    {
        var jsonInfo = _propertiesFilledWithPolicy.First().JsonInfo;
        var jsonString = File.ReadAllText(jsonInfo.JsonAbsolutePath);
    }
}

