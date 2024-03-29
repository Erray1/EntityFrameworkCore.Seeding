using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Modelling;

public sealed class SeederModelInfo : SeederInfoBase
{
    public SeederModelInfo()
    {
        IsConfigured = false;
        Entities = new List<SeederEntityInfo>();
    }
    public bool IsConfigured;
    public IEnumerable<SeederEntityInfo> Entities { get; set; }
    
}