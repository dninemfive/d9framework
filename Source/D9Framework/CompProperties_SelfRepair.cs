using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace D9Framework
{
    class CompProperties_SelfRepair : CompProperties
    {
        public int TicksPerRepair;
        
        public CompProperties_SelfRepair()
        {
            base.compClass = typeof(CompSelfRepair);
        }
    }
}
