using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
public abstract class SeederEntityBinder
{
    protected readonly IModel _dbModel;
    protected readonly Dictionary<SeederEntityInfo, IEnumerable<object>> _entities;
    public abstract void BindEntities();

    public static SeederEntityBinder CreateBinder(IModel dbModel, Dictionary<SeederEntityInfo, IEnumerable<object>> entities)
    {
        throw new NotImplementedException();   
    }
    protected SeederEntityBinder(IModel dbModel, Dictionary<SeederEntityInfo, IEnumerable<object>> entities) {
        _dbModel = dbModel;
        _entities = entities;
    }
}

