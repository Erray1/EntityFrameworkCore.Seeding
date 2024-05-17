using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling.Validation
{
    public class SeederModelValidationException : Exception
    {
        public SeederModelValidationException(string message) : base(message) { }
        
    }
}
