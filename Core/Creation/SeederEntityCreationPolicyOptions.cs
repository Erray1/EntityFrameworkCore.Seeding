using EntityFrameworkCore.Seeding.Core.Creation.CreationPolicies;
using EntityFrameworkCore.Seeding.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Creation;
public sealed class SeederEntityCreationPolicyOptions
{
    public IEnumerable<SeederPropertyInfo> PropertiesCreated {  get; set; }
    public SeederEntityInfo EntityInfo { get; set; }
    public SeederPropertiesCreationPolicy? Next { get; set; }
}

