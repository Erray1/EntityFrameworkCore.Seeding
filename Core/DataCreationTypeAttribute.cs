﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
    public class DataCreationTypeAttribute : Attribute
    {
    public DataCreationType CreationType { get; init; }
    }

