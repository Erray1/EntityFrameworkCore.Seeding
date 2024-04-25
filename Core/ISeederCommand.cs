using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
internal interface ISeederCommand
{
    public Task Execute(ISeedingContext context);
    protected Task MoveNext(ISeedingContext context);
}

