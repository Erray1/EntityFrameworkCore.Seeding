using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling.Utilities;

public enum EntityRelationType
{
    OneToOne,
    OneToOneNullable,
    OneToMany,
    ManyToOne,
    ManyToMany,

}
