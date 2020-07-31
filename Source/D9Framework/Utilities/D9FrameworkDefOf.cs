using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace D9Framework
{
    [DefOf]
    public static class D9FrameworkDefOf
    {
#pragma warning disable CS0649
        public static StatDef HealingRateFactor;
        public static StatDef BleedRateFactor;
#pragma warning restore CS0649

        static D9FrameworkDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(D9FrameworkDefOf));
        }
    }
}
