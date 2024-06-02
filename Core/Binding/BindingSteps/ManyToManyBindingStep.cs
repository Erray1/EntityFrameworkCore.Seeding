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
    public class ManyToManyBindingStep<TContext> : ManyToManyBindingStepBase
        where TContext : ManyToManyBindingStepContext
    {
        protected TContext _context;
    }
}
