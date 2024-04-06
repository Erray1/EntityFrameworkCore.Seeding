using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.History;

public enum HistoryStoreTypes
{
    ToTextFile,
    ToDB,
    NoHistory
}