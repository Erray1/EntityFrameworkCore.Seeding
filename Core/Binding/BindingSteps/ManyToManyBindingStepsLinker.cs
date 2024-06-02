using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore;
using EntityFrameworkCore.Seeding;
using EntityFrameworkCore.Seeding.Core;
using EntityFrameworkCore.Seeding.Core.Binding;
using EntityFrameworkCore.Seeding.Core.Binding.BindingSteps;
using EntityFrameworkCore.Seeding.Core.Binding.Context;

namespace EntityFrameworkCore.Seeding.Core.Binding.BindingSteps
{
    public class ManyToManyBindingStepsLinker
    {
        // Типы шагов, подходящие при определённых условиях
        private static IReadOnlyDictionary<Func<EntityManyToManyRelation, bool>, Type> _bindingStepTypes;
        static ManyToManyBindingStepsLinker()
        {
            #warning заполнить эту хуету
            // Common step operates with left side
            // Reversed step operates with right side
            _bindingStepTypes = new Dictionary<Func<EntityManyToManyRelation, bool>, Type>()
            {
                {x => x.LeftNavigationProperty is not null, typeof(BindMainEntitiesStep) },
                {x => x.RightNavigationProperty is not null, typeof(BindMainEntitiesStepReversed) },
                {x => x.JoinEntityData.LeftNavigationToJoinProperty is not null , typeof(BindMainAndJoinEntitiesStep) },
                {x => x.JoinEntityData.RightNavigationToJoinProperty is not null , typeof(BindMainAndJoinEntitiesStepReversed) },
                {x => x.JoinEntityData.LeftNavigationFromJoinProperty is not null, typeof(BindJoinAndMainEntitiesNavigationStep) },
                {x => x.JoinEntityData.LeftNavigationFromJoinProperty is not null, typeof(BindJoinAndMainEntitiesNavigationStepReversed) },
            };
        }

        private readonly EntityManyToManyRelation _relation;
        public ManyToManyBindingStepsLinker(EntityManyToManyRelation relation)
        {
            _relation = relation;
        }
        public ManyToManyBindingStepBase CreateChain()
        {
            ManyToManyBindingStepBase? first = null;
            ManyToManyBindingStepBase? current = null;
            foreach (var (predicate, stepType) in _bindingStepTypes)
            {
                if (!predicate.Invoke(_relation)) continue;

                var stepCtor = stepType.GetConstructors().Single();
                var contextType = stepCtor.GetParameters().Single().ParameterType;
                var contextCtor = contextType.GetConstructors().Single();

                var context = contextCtor.Invoke([_relation]);
                var step = (ManyToManyBindingStepBase)stepCtor.Invoke([context]);

                if (current is null)
                {
                    first = step;
                    current = step;
                    continue;
                }

                current.WithNextStep(step);
                current = step;
            }
            return first;
        }
    }
}
