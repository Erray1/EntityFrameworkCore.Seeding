using EntityFrameworkCore.Seeding.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.CreationPolicies;
public class EntityLoadedFromGitHubPolicy : SeederEntityCreationPolicy
{
    public IEnumerable<object> CreateEntities(SeederEntityInfo entity)
    {
        throw new NotImplementedException();
    }

    protected override void createPropertiesPool()
    {
        throw new NotImplementedException();
    }

}

