using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
public enum DataCreationType
{
    FromJSON,
    FromGivenPool,
    Random,
    Loaded
}

