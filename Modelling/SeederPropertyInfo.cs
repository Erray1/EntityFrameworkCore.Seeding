using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling;
public class SeederPropertyInfo : SeederInfoBase
{
    public SeederPropertyInfo(Type propertyType)
    {
        PropertyType = propertyType;
        IsConfigured = false;
        AreValuesRandom = false;
    }
    public bool IsConfigured;
    public Type PropertyType { get; set; }
    public object PossibleValues { get; set; }
    public bool AreValuesRandom { get; set; }
}


