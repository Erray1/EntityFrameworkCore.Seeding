using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Binding.Context
{
    public class JoinAndMainEntitiesForeignKeyBindContext : MainEntitiesBindContext
    {
        public JoinAndMainEntitiesForeignKeyBindContext(EntityManyToManyRelation relation) : base(relation)
        {
        }
    }
}
