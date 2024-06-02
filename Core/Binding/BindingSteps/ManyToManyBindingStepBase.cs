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
    public abstract class ManyToManyBindingStepBase
    {
        protected ManyToManyBindingStepBase _next;
        public void WithNextStep(ManyToManyBindingStepBase nextStep)
        {
            _next = nextStep;
        }
        public void BindEntities(object leftEntity, object rightEntity, object? joinEntity = null)
        {
            bindEntities(leftEntity, rightEntity, joinEntity);
            if (_next is not null)
            {
                _next.BindEntities(leftEntity, rightEntity, joinEntity);
            }
        }
        protected virtual void bindEntities(object leftEntity, object rightEntity, object? joinEntity = null) { }
    }
}
