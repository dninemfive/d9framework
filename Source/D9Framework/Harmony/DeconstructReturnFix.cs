using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace D9Framework
{
    /// <summary>
    /// Fixes the bug where the game will never drop a single item set to drop 100% of the time. 
    /// </summary>
    [StaticConstructorOnStartup]
    static class DeconstructReturnFix
    {
        [HarmonyPatch(typeof(GenLeaving), "DoLeavingsFor", new Type[] { typeof(Thing), typeof(Map), typeof(DestroyMode), typeof(CellRect), typeof(Predicate<IntVec3>), typeof(List<Thing>)})]
        class CalcFix
        {
            [HarmonyPrefix]
            public static bool DoLeavingsForPrefix(Thing diedThing, Map map, DestroyMode mode, CellRect leavingsRect, Predicate<IntVec3> nearPlaceValidator = null)
            {
                if (mode != DestroyMode.Deconstruct) return true; //just to make it more readable
                ThingOwner<Thing> thingOwner = new ThingOwner<Thing>();
                if (GenLeaving.CanBuildingLeaveResources(diedThing, mode))
                {                        
                    Frame frame = diedThing as Frame;
                    if (frame != null)
                    {
                        for (int frameResCt = frame.resourceContainer.Count - 1; frameResCt >= 0; frameResCt--)
                        {                                
                            int gblrc;
                            if ((gblrc = GBRLC(diedThing)(frame.resourceContainer[frameResCt].stackCount)) > 0) frame.resourceContainer.TryTransferToContainer(frame.resourceContainer[frameResCt], thingOwner, gblrc, true);
                        }
                        frame.resourceContainer.ClearAndDestroyContents(mode);
                    }
                    else
                    {
                        // TODO: ModExtension specifying drop rates per ThingDef. Needs to be relatively optimized.
                        List<ThingDefCountClass> list = diedThing.CostListAdjusted();
                        for (int l = 0; l < list.Count; l++)
                        {                                
                            ThingDefCountClass tdcc = list[l];
                            int gblrc;
                            if ((gblrc = GBRLC(diedThing)(tdcc.count)) > 0)
                            {
                                Thing thing = ThingMaker.MakeThing(tdcc.thingDef, null);
                                thing.stackCount = gblrc;
                                thingOwner.TryAdd(thing, true);
                            }
                        }
                    }
                }
                List<IntVec3> cellList = leavingsRect.Cells.InRandomOrder(null).ToList();
                int cellInd = 0;
                while (true)
                {
                    if (thingOwner.Count > 0)
                    {
                        ThingOwner<Thing> thingOwner2 = thingOwner;
                        Thing thing = thingOwner[0];
                        IntVec3 dropLoc = cellList[cellInd];
                        ThingPlaceMode mode2 = ThingPlaceMode.Near;
                        Thing thing4 = default(Thing);
                        if (thingOwner2.TryDrop(thing, dropLoc, map, mode2, out thing4, null, nearPlaceValidator))
                        {
                            cellInd++;
                            if (cellInd >= cellList.Count)
                            {
                                cellInd = 0;
                            }
                            continue;
                        }
                        break;
                    }
                    return false;
                }
                ULog.Warning("Deconstruct Return Fix: Failed to place all leavings for destroyed thing " + diedThing + " at " + leavingsRect.CenterCell, false);
                return false;
            }//end DoLeavingsForPrefix
            public static Func<int, int> GBRLC(Thing t)
            {
                if (!GenLeaving.CanBuildingLeaveResources(t, DestroyMode.Deconstruct))
                {
                    return (int count) => 0;
                }
                return (int count) => GenMath.RoundRandom(Mathf.Min((float)count * t.def.resourcesFractionWhenDeconstructed, (float)(count))); //other destroy modes deleted because I always know the mode
            }
        }//end CalcFix
    }//end DeconstructReturnFix
}//end namespace
