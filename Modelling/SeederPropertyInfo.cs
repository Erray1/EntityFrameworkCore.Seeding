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
    public string Name { get; set; }
    public object PossibleValues { get; set; }
    public bool AreValuesRandom { get; set; }
}


