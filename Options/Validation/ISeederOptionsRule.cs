using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Options.Validation;

public interface ISeederOptionsRule
{
    public void ValidateAndThrowException(SeederOptions options);
}
