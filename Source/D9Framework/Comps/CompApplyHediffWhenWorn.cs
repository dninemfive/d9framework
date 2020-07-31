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
    /// 
    /// For performance reasons, the interval isn't used when the apparel's <c>tickerType</c> is Rare.
    /// 
    /// If you want custom behavior with severity, e.g. increasing severity when worn, use a HediffComp for that; applied hediffs will be of severity <c>initialSeverity</c> to start.
    /// </remarks>
    class CompApplyHediffWhenWorn : CompWithCheapHashInterval
    {
        Apparel Apparel => base.parent as Apparel;
        CompProperties_ApplyHediffWhenWorn Props => (CompProperties_ApplyHediffWhenWorn)base.props;

        public void ApplyHediff()
        {
            Apparel.Wearer.health.AddHediff(Props.hediffToApply, null, null, null);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (IsCheapIntervalTick(Props.tickInterval)) ApplyHediff();
        }
        public override void CompTickRare()
        {
            base.CompTickRare();
            ApplyHediff();
        }
    }
    class CompProperties_ApplyHediffWhenWorn : CompProperties
    {
#pragma warning disable CS0649
        public int tickInterval = 250;
        public HediffDef hediffToApply;
#pragma warning restore CS0649

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
