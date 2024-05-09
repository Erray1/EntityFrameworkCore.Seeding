using EntityFrameworkCore.Seeding.Core.CreationPolicies;
using EntityFrameworkCore.Seeding.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Creation;
public sealed class SeederEntityCreationPolicyOptions
{
    public IEnumerable<SeederPropertyInfo> PropertisCreated {  get; set; }
    public SeederEntityInfo EntityInfo { get; set; }
    public SeederEntityCreationPolicy? Next { get; set; }
}

