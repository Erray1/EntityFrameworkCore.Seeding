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
    public class BindJoinAndMainEntitiesNavigationStep : ManyToManyBindingStep<JoinAndMainEntitiesNavigationBindContext>
    {
        public BindJoinAndMainEntitiesNavigationStep(JoinAndMainEntitiesNavigationBindContext context)
        {
            _context = context;
        }
        protected override void bindEntities(object leftEntity, object rightEntity, object? joinEntity = null)
        {

        }
    }

    public class BindJoinAndMainEntitiesNavigationStepReversed : BindJoinAndMainEntitiesNavigationStep
    {
        public BindJoinAndMainEntitiesNavigationStepReversed(JoinAndMainEntitiesNavigationBindContext context) : base(context)
        {
        }

        protected override void bindEntities(object leftEntity, object rightEntity, object? joinEntity = null)
        {
            base.bindEntities(rightEntity, leftEntity, joinEntity);
        }
    }
}
