using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace D9Framework
{
    public abstract class CompWithCheapHashInterval : ThingComp
    {
        /// <summary>
        /// A comp implementing a cheaper HashInterval.
        /// <para>In the base game, <c>IsHashIntervalTick</c> is meant to allow modders to distribute ticks so there aren't lag spikes around TickRare and similar methods.</para>
        /// <para>However, that implementation recomputes the offset each time it's called, reducing the performance gains from distributing ticks.</para>
        /// <para>This method calculates it once and saves it, meaning you can use it whenever. It's small enough that it honestly doesn't need an abstract class,
        /// but I like promoting good design.</para>
        /// </summary>
        private int hashOffset = 0;
        public bool IsCheapIntervalTick(int interval) => (int)(Find.TickManager.TicksGame + hashOffset) % interval == 0;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            hashOffset = parent.thingIDNumber.HashOffset();
        }

    }
}
