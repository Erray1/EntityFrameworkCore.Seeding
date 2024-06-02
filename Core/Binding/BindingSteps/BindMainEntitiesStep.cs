using EntityFrameworkCore;
using EntityFrameworkCore.Seeding;
using EntityFrameworkCore.Seeding.Core;
using EntityFrameworkCore.Seeding.Core.Binding;
using EntityFrameworkCore.Seeding.Core.Binding.BindingSteps;
using EntityFrameworkCore.Seeding.Core.Binding.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Binding.BindingSteps
{
    public class BindMainEntitiesStep : ManyToManyBindingStep<MainEntitiesBindContext>
    {
        public BindMainEntitiesStep(MainEntitiesBindContext context)
        {
            _context = context;
        }
        protected override void bindEntities(object leftEntity, object rightEntity, object? joinEntity = null)
        {

        }
    }

    public class BindMainEntitiesStepReversed : BindMainEntitiesStep
    {
        public BindMainEntitiesStepReversed(MainEntitiesBindContext context) : base(context)
        {
        }

        protected override void bindEntities(object leftEntity, object rightEntity, object? joinEntity = null)
        {
            base.bindEntities(rightEntity, leftEntity, joinEntity);
        }
    }
}
