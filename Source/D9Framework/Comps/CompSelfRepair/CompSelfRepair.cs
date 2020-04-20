using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace D9Framework
{
    /// <summary>
    /// Automatically repairs a specified (non-pawn) item. Example implementation of CompWithCheapHashInterval and designed for use with CompFromStuff.
    /// </summary>
    public class CompSelfRepair : CompWithCheapHashInterval
    {        
        CompProperties_SelfRepair Props => (CompProperties_SelfRepair)props;
        public override void CompTick()
        {
            base.CompTick();
            int hp = base.parent.HitPoints;
            if (IsCheapIntervalTick(Props.tickInterval) && parent.def.useHitPoints && hp < parent.MaxHitPoints) hp++;
        }
        public override string CompInspectStringExtra()
        {
            string ret = base.CompInspectStringExtra();
            if(Prefs.DevMode) ret += "CompSelfRepair with TicksPerRepair " + Props.tickInterval;
            return ret;
        }
    }
}