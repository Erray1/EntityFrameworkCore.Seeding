using EntityFrameworkCore.Seeding.Core.Binding;

namespace EntityFrameworkCore.Seeding.Modelling;

public sealed class SeederModelInfo : SeederInfoBase
{
    public SeederModelInfo()
    {
        IsConfigured = false;
    }
    public bool IsConfigured { get; set; }
    public List<SeederEntityInfo> Entities { get; set; }
    public List<EntityRelation> Relations { get; set; }
    public List<EntityManyToManyRelation> ManyToManyRelations { get; set;}
    
}