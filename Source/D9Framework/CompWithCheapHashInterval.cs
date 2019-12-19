using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace D9Framework
{
    abstract class CompWithCheapHashInterval : ThingComp
    {
        private int? hashOffset = null;
        public int TickInterval;
        public bool IsCheapIntervalTick => (int)(Find.TickManager.TicksGame + hashOffset) % TickInterval == 0;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            hashOffset = parent.thingIDNumber.HashOffset();
        }

    }
}
