using EntityFrameworkCore.Seeding.Modelling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
public abstract class SeederEntityCreator
{
    public static SeederEntityCreator CreateNew(SeederModelInfo seederModel)
    {

    }
    public abstract Dictionary<SeederEntityInfo, IEnumerable<object>> CreateEntities();
}

