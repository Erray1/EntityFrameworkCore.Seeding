using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling.Utilities;

    public sealed record SeederEntityRelationInfo
    (
    PropertyInfo Property,
    EntityRelationType RelationType,
    bool IsNavigationProperty
    );
        
