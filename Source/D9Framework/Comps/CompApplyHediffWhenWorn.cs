using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace D9Framework.Comps
{
    /// <summary>
    /// Applies a specified hediff at a specified interval when the parent, which must be <c>Apparel</c>, is worn.
    /// </summary>
    /// <remarks>
    /// There unfortunately isn't a method which is run when apparel is equipped, so there will be a delay of max <c>tickInterval</c> before the hediff is applied.
    /// </remarks>
    class CompApplyHediffWhenWorn : ThingComp
    {
        Apparel apparel => base.parent as Apparel;
    }
    class CompProperties_ApplyHediffWhenWorn : CompProperties
    {
        public CompProperties_ApplyHediffWhenWorn()
        {
            base.compClass = typeof(CompApplyHediffWhenWorn);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string str in base.ConfigErrors(parentDef)) yield return str;
            if (!parentDef.thingClass.IsAssignableFrom(typeof(Apparel))) yield return "CompApplyHediffWhenWorn must be on a Thing with thingClass Apparel!";
        }
    }
}
