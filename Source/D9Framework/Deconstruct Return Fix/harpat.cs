using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace Deconstruct_Return_Fix
{
    [StaticConstructorOnStartup]
    static class DeconstructReturnFix
    {
        static DeconstructReturnFix()
        {
            var harmony = HarmonyInstance.Create("com.dninemfive.deconstructreturnfix");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            /*
            var harmony = HarmonyInstance.Create("com.dninemfive.advancedshields");
            var original = typeof(WorkGiver_HunterHunt).GetMethod("HasShieldAndRangedWeapon");
            var prefix = typeof(Hunterfix).GetMethod("HunterPrefix");
            harmony.Patch(original, new HarmonyMethod(prefix), null);
            original = typeof(Alert_ShieldUserHasRangedWeapon).GetMethod("GetReport");
            prefix = typeof(GenericFix).GetMethod("GenericPrefix");
            harmony.Patch(original, new HarmonyMethod(prefix), null);
            */
            Log.Message("Deconstruct Return Fix harmony patch successfully loaded");
        }

        [HarmonyPatch(typeof(GenLeaving), "DoLeavingsFor", new Type[] { typeof(Thing), typeof(Map), typeof(DestroyMode), typeof(CellRect), typeof(Predicate<IntVec3>)})]
        class CalcFix
        {
            [HarmonyPrefix]
            public static bool DoLeavingsForPrefix(Thing diedThing, Map map, DestroyMode mode, CellRect leavingsRect, Predicate<IntVec3> nearPlaceValidator = null)
            {
                //code copied and modified directly from DoLeavingsFor
                if (mode == DestroyMode.Deconstruct)// || mode == DestroyMode.Refund)
                {
                    ThingOwner<Thing> thingOwner = new ThingOwner<Thing>();
                    if (GenLeaving.CanBuildingLeaveResources(diedThing, mode))
                    {                        
                        Frame frame = diedThing as Frame;
                        if (frame != null)
                        {
                            for (int num = frame.resourceContainer.Count - 1; num >= 0; num--)
                            {                                
                                int num2 = GBRLC(diedThing, mode)(frame.resourceContainer[num].stackCount);
                                if (num2 > 0)
                                {
                                    frame.resourceContainer.TryTransferToContainer(frame.resourceContainer[num], thingOwner, num2, true);
                                }
                            }
                            frame.resourceContainer.ClearAndDestroyContents(mode);
                        }
                        else
                        {
                            List<ThingDefCountClass> list = diedThing.CostListAdjusted();
                            for (int l = 0; l < list.Count; l++)
                            {                                
                                ThingDefCountClass thingDefCountClass = list[l];
                                int num3 = GBRLC(diedThing, mode)(thingDefCountClass.count);
                                if (num3 > 0)
                                {
                                    Thing thing2 = ThingMaker.MakeThing(thingDefCountClass.thingDef, null);
                                    thing2.stackCount = num3;
                                    thingOwner.TryAdd(thing2, true);
                                }
                            }//end for
                        }//end if..else
                    }//end if
                    List<IntVec3> list2 = leavingsRect.Cells.InRandomOrder(null).ToList();
                    int num5 = 0;
                    while (true)
                    {
                        if (thingOwner.Count > 0)
                        {
                            ThingOwner<Thing> thingOwner2 = thingOwner;
                            Thing thing3 = thingOwner[0];
                            IntVec3 dropLoc = list2[num5];
                            ThingPlaceMode mode2 = ThingPlaceMode.Near;
                            Thing thing4 = default(Thing);
                            if (thingOwner2.TryDrop(thing3, dropLoc, map, mode2, out thing4, null, nearPlaceValidator))
                            {
                                num5++;
                                if (num5 >= list2.Count)
                                {
                                    num5 = 0;
                                }
                                continue;
                            }
                            break;
                        }
                        return false;
                    }
                    Log.Warning("Deconstruct Return Fix: Failed to place all leavings for destroyed thing " + diedThing + " at " + leavingsRect.CenterCell, false);
                    return false;
                }//end if
                return true;
            }//end DoLeavingsForPrefix
            public static Func<int, int> GBRLC(Thing t, DestroyMode d) //GetBuildingResourcesLeaveCalculator very slightly modified.
            {
                if (!GenLeaving.CanBuildingLeaveResources(t, d))
                {
                    return (int count) => 0;
                }
                switch (d)
                {
                    case DestroyMode.Vanish:
                        return (int count) => 0;
                    case DestroyMode.KillFinalize:
                        return (int count) => GenMath.RoundRandom((float)count * 0.5f);
                    case DestroyMode.Deconstruct:
                        return (int count) => GenMath.RoundRandom(Mathf.Min((float)count * t.def.resourcesFractionWhenDeconstructed, (float)(count)));
                    case DestroyMode.Cancel:
                        return (int count) => GenMath.RoundRandom((float)count * 1f);
                    case DestroyMode.FailConstruction:
                        return (int count) => GenMath.RoundRandom((float)count * 0.5f);
                    case DestroyMode.Refund:
                        return (int count) => count;
                    default:
                        throw new ArgumentException("Unknown destroy mode " + d + " (Deconstruct Return Fix error)");
                }
            }
        }//end CalcFix
    }//end DeconstructReturnFix
}//end namespace
