using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace D9Framework
{
    /// <summary>
    /// A <c>ThingComp</c> which takes optional inputs and processes them into outputs, like a mix of the vanilla fermenting barrels and CompSpawners.
    /// </summary>
    class CompAccumulator : ThingComp
    {
        ThingOwner<Thing> internalContainer; // should probably actually be a CompSlottable, smh
        public CompRefuelable Fuel => base.parent.TryGetComp<CompRefuelable>();
        public CompPowerTrader Power => base.parent.TryGetComp<CompPowerTrader>();
    }
    class CompProperties_Accumulator : CompProperties
    {
        public CompProperties_Accumulator()
        {
            base.compClass = typeof(CompAccumulator);
        }
    }
}
