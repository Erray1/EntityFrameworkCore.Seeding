namespace EntityFrameworkCore.Seeding.Modelling;

public sealed class SeederModelInfo : SeederInfoBase
{
    public SeederModelInfo()
    {
        IsConfigured = false;
    }
    public bool IsConfigured;
    public List<SeederEntityInfo> Entities { get; set; }
    
}