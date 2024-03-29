using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Reload;

public enum RefreshBehaviours
{
    SeededData,
    InRunTimeCreatedData,
    AllData,
    NoData
}