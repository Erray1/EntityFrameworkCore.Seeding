
using System.Text;

namespace EntityFrameworkCore.Seeding.Modelling.Validation;

public static class SeederModelValidator
{
    public static void ThrowExceptionIfInvalid(SeederModelInfo model)
    {
        bool allEntitiesAreConfigured = model.Entities
            .All(x => x.Properties.All(prop => prop.IsConfigured));

        if (allEntitiesAreConfigured) return;

        var notConfiguredEntities = model.Entities
            .Where(e => !e.IsConfigured)
            .Select(e => new NotConfiguredEntityProps
            {
                Properties = e.Properties,
                EntityType = e.EntityType
            })
            .ToList();

        throw new SeederModelValidationException(getNotConfiguredEntitiesSummary(notConfiguredEntities));
    }

    private static string getNotConfiguredEntitiesSummary(List<NotConfiguredEntityProps> entities)
    {
        var builder = new StringBuilder();
        foreach (var entity in entities)
        {
            builder.AppendFormat("{0}: ", entity.EntityType.Name);
            builder.Append(entity.Properties.Select(x => x.PropertyName).ToArray());
            builder.AppendLine();
        }
        var result = builder.ToString();
        return result;
    }

    class SeederModelValidationException : Exception
    {
        public SeederModelValidationException(string notConfiguredEntitiesSummary)
            : base($"Some entities are not configured:\n{notConfiguredEntitiesSummary}") { }
    }
    class NotConfiguredEntityProps
    {
        public Type EntityType { get; set; }
        public List<SeederPropertyInfo> Properties { get; set; }
    }
}