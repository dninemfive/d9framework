using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace D9Framework {
    public class CompProperties_Validator : CompProperties
    {
        public int tickInterval;

        private bool shouldUse = true;
        public bool ShouldUse => shouldUse;

        public CompProperties_Validator() {
            compClass = typeof(CompValidator);
        }
        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            if(parentDef.placeWorkers == null || parentDef.placeWorkers.Count < 1)
            {
                shouldUse = false;
                yield return "CompValidator used but no PlaceWorkers set";
            }
            if (!parentDef.Minifiable)
            {
                shouldUse = false;
                yield return "CompValidator used but parent def not minifiable";
            }
        }
    }
}
