using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.Seeding.Core;
public class SeederEntityBinder
{
    protected IModel _dbModel;
    protected Dictionary<SeederEntityInfo, IEnumerable<object>> _entities;
    public void BindEntities(Dictionary<SeederEntityInfo, IEnumerable<object>> entities)
    {
        _entities = entities;
    }
}

