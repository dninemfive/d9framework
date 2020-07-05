using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;

namespace D9Framework.Misc
{
    class DoubleStuffThing : ThingWithComps
    {
        public ThingDef StuffTwo => stuffTwo;
        private ThingDef stuffTwo;

        public override Color DrawColorTwo
        {
            get
            {
                if (StuffTwo != null) return def.GetColorForStuff(StuffTwo);
                return def.graphicData?.color ?? Color.white;
            }
        }
        public void SetStuffTwoDirect(ThingDef newStuffTwo)
        {
            stuffTwo = newStuffTwo;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref stuffTwo, "stuffTwo");
        }
    }
    public class SecondStuff : DefModExtension
    {
# pragma warning disable CS0649
        public int stuffTwoCost;
        public List<StuffCategoryDef> stuffTwoCategories;
# pragma warning restore CS0649
    }
}
