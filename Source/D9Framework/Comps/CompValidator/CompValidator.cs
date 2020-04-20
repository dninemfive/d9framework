using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.Sound;
using RimWorld;

namespace D9Framework {
    /// <summary>
    /// Minifies or destroys the parent Thing if it wouldn't be allowed to be placed at its current location.
    /// </summary>
    public class CompValidator : CompWithCheapHashInterval
    {
        CompProperties_Validator Props => (CompProperties_Validator)base.props;

        public override void CompTick()
        {
            base.CompTick();   
            if (Props.ShouldUse && IsCheapIntervalTick(Props.tickInterval))
            {
                foreach(PlaceWorker pw in base.parent.def.PlaceWorkers)
                {
                    if (!pw.AllowsPlacing(base.parent.def, base.parent.Position, base.parent.Rotation, base.parent.Map).Accepted)
                    {
                        MinifyOrDestroy();
                        break;
                    }
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            string ret = base.CompInspectStringExtra();
            if (Prefs.DevMode)
            {
                ret += "PlaceWorkers: (count = " + base.parent.def.PlaceWorkers.Count + "):";
                for (int i = 0; i < Math.Min(3, base.parent.def.PlaceWorkers.Count); i++) ret += "\n\t" + base.parent.def.PlaceWorkers.ElementAt(i).ToString();
            }
            return ret;
        }

        public virtual void MinifyOrDestroy()
        {
            if (base.parent.def.Minifiable)
            {
                Map map = parent.Map;
                MinifiedThing package = MinifyUtility.MakeMinified(parent);
                GenPlace.TryPlaceThing(package, parent.Position, map, ThingPlaceMode.Near);
                SoundDef.Named("ThingUninstalled").PlayOneShot(new TargetInfo(parent.Position, map));
            }
            else
            {
                base.parent.Destroy(DestroyMode.KillFinalize);
            }
        }
    }
}
