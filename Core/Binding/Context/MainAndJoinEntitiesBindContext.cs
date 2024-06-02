using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Binding.Context
{
    public class MainAndJoinEntitiesBindContext : ManyToManyBindingStepContext
    {
        public MainAndJoinEntitiesBindContext(EntityManyToManyRelation relation) : base(relation)
        {
        }
    }
}
