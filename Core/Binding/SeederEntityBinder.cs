using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.Seeding.Core.Binding;
public class SeederEntityBinder
{
    public SeederEntityBinder()
    {
        
    }
    protected IModel _dbModel;
    protected Dictionary<SeederEntityInfo, IEnumerable<object>> _entities;
    public void BindEntities(Dictionary<SeederEntityInfo, IEnumerable<object>> entities, IModel dbModel)
    {
        _entities = entities;
        _dbModel = dbModel;
    }
}

