using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld; 

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
                foreach (Slot s in slots) if (!s.Full) return false;
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
            public bool Full => !Empty && HeldThing.stackCount >= MaxStackCount;
            public bool Empty => HeldThing == null;
            public int MaxStackCount => parent?.Props?.stackCountPerSlot ?? 0;
            private Command_Action gizmo = null;

            public Slot(CompSlottable p, ThingFilter dtf = null)
            {
                if (p == null) ULog.Error("Slot constructed with null parent!");
                parent = p;
                thingFilter = dtf;
                contents = new ThingOwner<Thing>(this, true);
            }

            public Command_Action Gizmo
            {
                get
                {
                    if (gizmo != null) return gizmo;
                    gizmo = new Command_Action
                    {
                        defaultLabel = Empty ? "D9FEmptySlot".Translate() : (TaggedString)HeldThing.Label, // TODO: partially filled summary?
                        defaultDesc = Empty ? "D9FEmptySlotDesc".Translate() : 
                                              Full ? "D9FFilledSlotDesc".Translate(HeldThing.Label, thingFilter.Summary) : 
                                                     "D9FPartiallyFilledSlotDesc".Translate(HeldThing.Label, thingFilter.Summary, HeldThing.stackCount),
                        activateSound = SoundDef.Named("Click"),
                        icon = Empty ? ContentFinder<Texture2D>.Get("UI/Commands/LaunchReport", true) : ContentFinder<Texture2D>.Get(HeldThing.def.graphicData.texPath), // TODO: custom texture for empty gizmo
                        action = () =>
                        {
                            // create ThingFilter window
                        },
                    };
                    return gizmo;
                }
            }

            public bool CanSlot(Thing thing)
            {
                return !Full && parent.Props.fixedThingFilter.Allows(thing) && thingFilter.Allows(thing) && HeldThing.stackCount + thing.stackCount <= MaxStackCount;
            }
            public bool TrySlotThing(Thing thing)
            {
                if (!CanSlot(thing)) return false;
                // add thing to holder
                // mark gizmo to be remade
                gizmo = null;
                return true;
            }
            public bool TryEmpty()
            {
                if (Empty) return false;
                // remove thing from holder
                // mark gizmo to be remade
                gizmo = null;
                return true;
            }
            // Similarly, when clicking "save" on the ThingFilter popup, set gizmo to null
            #region IThingHolder stuff
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
            #endregion IThingHolder stuff
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                slots = new List<Slot>();
                for (int i = 0; i < Props.slots; i++) slots.Add(new Slot(this, Props.defaultThingFilter));
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach(Slot s in slots)
            {
                yield return s.Gizmo;
            }
        }

        public bool CanSlotAny(Thing thing)
        {
            foreach (Slot s in slots) if (s.CanSlot(thing)) return true;
            return false;
        }

        #region IThingHolder stuff
        public ThingOwner GetDirectlyHeldThings()
        {
            return new ThingOwner<Thing>(this);
        }
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            foreach (Slot s in slots) outChildren.Add(s);
        }
        #endregion IThingHolder stuff
    }
    class CompProperties_Slottable : CompProperties
    {
#pragma warning disable CS0649
        public ThingFilter fixedThingFilter,    // Fixed filter, anything not on this one can never be selected
                           defaultThingFilter;  // Initial filter settings
        public int slots = 1,
                   stackCountPerSlot = 1;
#pragma warning restore CS0649

        public CompProperties_Slottable()
        {
            base.compClass = typeof(CompSlottable);
        }
    }
}
