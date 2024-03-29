using EFCoreSeeder.Reload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Modelling;

public sealed class SeederEntityInfo : SeederInfoBase
{
    public bool IsConfigured;
    public int TimesCreated { get { return TimesCreated + Locality; } set { TimesCreated = value; } }
    public int Locality { get; set; } = 0;
    public RefreshBehaviours? OverrideRefreshBehaviour { get; set; } = null;
    public List<SeederPropertyInfo> Properties { get; set; } = new();
}
