using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace D9Framework
{
    /// <summary>
    /// Adds slots which can accept items. Slots have filters (a standard ThingFilter for now) and are either loaded automatically by pawns using a WorkGiver or a pawn can be ordered to 
    /// load them with a FloatMenu.
    /// </summary>
    class CompSlottable : ThingComp, IThingHolder
    {
        public CompProperties_Slottable Props => (CompProperties_Slottable)base.props;
        public bool Full
        {
            get
            {
                foreach (Slot s in slots) if (s.Empty) return false;
                return true;
            }
        }
        List<Slot> slots;
        public class Slot : IThingHolder
        {
            ThingOwner<Thing> contents;
            public Thing HeldThing => contents.NullOrEmpty() ? contents[0] : null;
            public CompSlottable parent;
            public ThingFilter thingFilter;
            public bool Empty => HeldThing == null;

            public bool CanSlot(Thing thing)
            {
                return Empty && parent.Props.fixedThingFilter.Allows(thing) && thingFilter.Allows(thing);
            }

            public IThingHolder ParentHolder
            {
                get
                {
                    return parent;
                }
            }
            public ThingOwner GetDirectlyHeldThings()
            {
                return contents;
            }
            public void GetChildHolders(List<IThingHolder> outChildren)
            {
                ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach(Slot s in slots)
            {
                
            }
        }

        public bool CanSlotAny(Thing thing)
        {
            foreach (Slot s in slots) if (s.CanSlot(thing)) return true;
            return false;
        }

        
    }
    class CompProperties_Slottable : CompProperties
    {
#pragma warning disable CS0649
        public ThingFilter fixedThingFilter;
        public int slots = 1;
#pragma warning restore CS0649

        public CompProperties_Slottable()
        {
            base.compClass = typeof(CompSlottable);
        }
    }
}
