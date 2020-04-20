using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using RimWorld;

namespace D9Framework
{
    /// <summary>
    /// Makes BuildableDef.ForceAllowPlaceOver actually cause the appropriate things to ignore the defs in question
    /// </summary>
    static class ForceAllowPlaceOverFix
    {
        /// <summary>
        /// Inserts the equivalent of "if newDef.ForceAllowPlaceOver(oldDef) return true;" into GenConstruct.CanPlaceBlueprintOver
        /// </summary>
        [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.CanPlaceBlueprintOver), new Type[] { typeof(BuildableDef), typeof(ThingDef) })]
        class GenConstructPlaceOver
        {            
            [HarmonyPostfix]
            static void CanPlaceBlueprintOverPostfix(ref bool __result, BuildableDef newDef, ThingDef oldDef)
            {
                if (newDef.ForceAllowPlaceOver(oldDef)) __result = true;
            }
        }

        /// <summary>
        /// Inserts the equivalent of "if newDef.ForceAllowPlaceOver(oldDef) return true;" into GenConstruct.BlocksConstruction
        /// </summary>
        [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.BlocksConstruction), new Type[] { typeof(Thing), typeof(Thing) })]
        class GenConstructBlocksConstruction
        {
            [HarmonyPostfix]
            static void CanPlaceBlueprintOverPostfix(ref bool __result, Thing constructible, Thing t)
            {
                ThingDef thingDef = (!(constructible is Blueprint)) ? ((!(constructible is Frame)) ? constructible.def.blueprintDef : constructible.def.entityDefToBuild.blueprintDef) : constructible.def;
                if (thingDef.entityDefToBuild.ForceAllowPlaceOver(t.def)) __result = true;
            }
        }
    }
}
