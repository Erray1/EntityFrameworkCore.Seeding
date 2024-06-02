using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore;
using EntityFrameworkCore.Seeding;
using EntityFrameworkCore.Seeding.Core;
using EntityFrameworkCore.Seeding.Core.Binding;
using EntityFrameworkCore.Seeding.Core.Binding.BindingSteps;
using EntityFrameworkCore.Seeding.Core.Binding.Context;

namespace EntityFrameworkCore.Seeding.Core.Binding.BindingSteps
{
    public class BindMainAndJoinEntitiesStep : ManyToManyBindingStep<MainAndJoinEntitiesBindContext>
    {
        public BindMainAndJoinEntitiesStep(MainAndJoinEntitiesBindContext context)
        {
            _context = context;
        }
        protected override void bindEntities(object leftEntity, object rightEntity, object? joinEntity = null)
        {
            base.bindEntities(leftEntity, rightEntity, joinEntity);
        }
    }
    public class BindMainAndJoinEntitiesStepReversed : BindMainAndJoinEntitiesStep
    {
        public BindMainAndJoinEntitiesStepReversed(MainAndJoinEntitiesBindContext context) : base(context)
        {
        }

        protected override void bindEntities(object leftEntity, object rightEntity, object? joinEntity = null)
        {
            base.bindEntities(rightEntity, leftEntity, joinEntity);
        }
    }
}
