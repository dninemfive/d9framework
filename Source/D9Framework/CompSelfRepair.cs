using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace D9Framework
{
    public class CompSelfRepair : ThingComp
    {
        CompProperties_SelfRepair Props => (CompProperties_SelfRepair)props;
        public override void CompTick()
        {
            base.CompTick();
            int hp = base.parent.HitPoints;
            if (parent.def.useHitPoints && hp < parent.MaxHitPoints && parent.IsHashIntervalTick(Props.TicksPerRepair)) hp++;
        }
    }
}