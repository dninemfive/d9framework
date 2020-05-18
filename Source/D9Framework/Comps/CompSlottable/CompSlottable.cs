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
    /*
     * Remaining todo:
     *      - Write the ITab. Should have subtabs either to the left of or on top of filters, and make use of IStoreSettingsParent if possible.
     *      - Make the gizmos open the appropriate ITab.
     *      - Allow the each slot to be forbidden, with the appropriate overlay if the whole comp is forbidden.
     *      - Add a boolean setting to have an overlay when the slot is empty, if desired.
     *      - Allow functionality without the ITab, including emptying all slots or each slot individually.
     *      - Write the WorkGiver.
     *      - Possibly a validator for pawns being able to slot things in? Might be best as an extension.
     *      - Clean up and document code better.
     *      - Handle priority for loading slots?
     *          - FloatMenu to load/unload (specific?) slots
     *          - Comp for things which can be slotted in, just to provide a FloatMenu in the other direction?
     */
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
            public ThingFilter fixedThingFilter, thingFilter;
            public bool Full => !Empty && HeldThing.stackCount >= MaxStackCount;
            public bool Empty => HeldThing == null;
            public bool PartiallyFull => !Empty && !Full;
            public int MaxStackCount;
            public string label;
            private Command_Action gizmo = null;

            public Slot(CompSlottable p, int stackCount, string l, ThingFilter fixedFilter = null, ThingFilter defaultFilter = null)
            {
                if (p == null) ULog.Error("Slot constructed with null parent!");
                parent = p;
                MaxStackCount = stackCount;
                label = l;
                fixedThingFilter = fixedFilter;
                thingFilter = defaultFilter;
                contents = new ThingOwner<Thing>(this, true);
            }

            public Gizmo Gizmo
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
                            // open tab for this slot
                        },
                    };
                    return gizmo;
                }
            }

            public bool CanSlot(Thing thing)
            {
                return !Full && fixedThingFilter.Allows(thing) && thingFilter.Allows(thing) && HeldThing.stackCount + thing.stackCount <= MaxStackCount;
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
                foreach (CompProperties_Slottable.SlotSettings s in Props.slots)
                {
                    int stack = s.stackCount ?? Props.slotDefaults.stackCount ?? 1;
                    string label = s.label ?? Props.slotDefaults.label ?? "D9F_CompSlottable_Label".Translate(slots.Count + 1); // default: "Slot {index}"
                    ThingFilter fixedTf = s.fixedThingFilter ?? Props.slotDefaults.fixedThingFilter;
                    ThingFilter defaultTf = s.defaultThingFilter ?? Props.slotDefaults.defaultThingFilter;                    
                    slots.Add(new Slot(this, stack, label, fixedTf, defaultTf));
                }
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
        public SlotSettings slotDefaults;
        public List<SlotSettings> slots;
        public class SlotSettings
        {
            public ThingFilter fixedThingFilter,    // Fixed filter, anything not on this one can never be selected
                           defaultThingFilter;      // Initial filter settings
            public int? stackCount = null;                 // nullable so default has chance of being applied
            public string label = null;
        }
        public bool showFilledBar = false;          // fuel-like percentage filled bar
#pragma warning restore CS0649

        public CompProperties_Slottable()
        {
            base.compClass = typeof(CompSlottable);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (String s in base.ConfigErrors(parentDef)) yield return s;
            if(!slotDefaults.stackCount.HasValue)
            {
                foreach (SlotSettings s in slots) if (!slotDefaults.stackCount.HasValue)
                    {
                        yield return "Stack count for at least one slot is null and the default stack count is null.";
                        break; // only print once per def
                    }
            }
            if(Prefs.DevMode && !parentDef.inspectorTabs.Where(x => x == typeof(ITab_CompSlottable)).Any() && slotDefaults.defaultThingFilter != null)
            {
                foreach (SlotSettings s in slots) if (s.defaultThingFilter != null)
                    {
                        Log.Warning("Potential config error in " + parentDef.defName + ": has default thingFilters but no ITab_CompSlottable. Players will not be able to change filter settings!");
                        break;
                    }
            }
        }
    }
}
