using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSeeder.Modelling;
public class SeederPropertyInfo : SeederInfoBase
{
    public SeederPropertyInfo(string propertyName)
    {
        PropertyName = propertyName;
        IsConfigured = false;
        AreValuesRandom = false;
    }
    public bool IsConfigured;
    public PropertyTypes PropertyType { get; set; }
    public string PropertyName { get; set; }
    public object PossibleValues { get; set; }
    public bool AreValuesRandom { get; set; }
}


