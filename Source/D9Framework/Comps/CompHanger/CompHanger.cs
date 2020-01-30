using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.Sound;
using RimWorld;

namespace D9Framework {
    /// <summary>
    /// Minifies a minifiable object if the thing it's hanging on is missing
    /// </summary>
    public class CompValidator : CompWithCheapHashInterval
    {
        CompProperties_Validator Props => (CompProperties_Validator)base.props;

        public override void CompTick()
        {
            if (Props.ShouldUse && IsCheapIntervalTick(Props.tickInterval))
            {
                foreach(PlaceWorker pw in base.parent.def.PlaceWorkers)
                {
                    if (!pw.AllowsPlacing(base.parent.def, base.parent.Position, base.parent.Rotation, base.parent.Map).Accepted) Minify();
                }
            }
        }
        
        public virtual void Minify()
        {
            Map map = parent.Map;
            MinifiedThing package = MinifyUtility.MakeMinified(parent);
            GenPlace.TryPlaceThing(package, parent.Position, map, ThingPlaceMode.Near);
            SoundDef.Named("ThingUninstalled").PlayOneShot(new TargetInfo(parent.Position, map));
        }
    }
}
