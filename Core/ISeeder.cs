using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
public interface ISeeder
{
    public Task ExecuteSeedingAsync(CancellationToken cancellationToken);
}

