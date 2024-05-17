using System.Text;

namespace EntityFrameworkCore.Seeding.Modelling.Validation
{
    public abstract class SeederModelValidationRule
    {
        protected SeederModelValidationRule? _next;
        public SeederModelValidationRule(SeederModelValidationRule? next)
        {
            _next = next;
        }
        public void ThrowExceptionIfInvalid(SeederModelInfo modelInfo, in Dictionary<SeederEntityInfo, List<string>> context)
        {
            var summary = getValidationErrorsSummary(modelInfo);
            foreach (var (entity, newErrors) in summary)
            {
                bool hasEntity = context.TryGetValue(entity, out var errorsOfEntity);
                if (hasEntity)
                {
                    errorsOfEntity!.AddRange(newErrors);
                }
                context.Add(entity, newErrors!);
            }
            if (_next is not null)
            {
                _next.ThrowExceptionIfInvalid(modelInfo, context);
                return;
            }
            if (context.Count != 0)
            {
                var message = getValidationSummaryAsString(context);
                throw new SeederModelValidationException(message);
            }

        }
        protected abstract IReadOnlyDictionary<SeederEntityInfo, List<string>> getValidationErrorsSummary(SeederModelInfo modelInfo);
        protected string getValidationSummaryAsString(Dictionary<SeederEntityInfo, List<string>> summary)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var (entity, errors) in summary)
            {
                builder.AppendLine(entity.EntityType.Name);
                foreach (var error in errors)
                {
                    builder.AppendLine(error);
                }
            }
            return builder.ToString();
        }

    }
}
