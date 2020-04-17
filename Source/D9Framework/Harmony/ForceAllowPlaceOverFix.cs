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
    static class ForceAllowPlaceOverFix
    {
        [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.CanPlaceBlueprintOver), new Type[] { typeof(BuildableDef), typeof(ThingDef) })]
        class GenConstructPlaceOver
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> CanPlaceBlueprintOverTranspiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> instrList = instructions.ToList();
                for (int i = 0; i < instrList.Count; i++)
                {
                    // Looking for the first half of (oldDef == ThingDefOf.SteamGeyser && !newDef.ForceAllowPlaceOver(oldDef))
                    if (instrList[i].opcode == OpCodes.Ldarg_1                                                             // IL 0074: ldarg.1
                        && instrList[i+1].opcode == OpCodes.Ldsfld                                                         // IL 0075: ldsfld class Verse.ThingDef RimWorld.ThingDefOf::SteamGeyser
                        && instrList[i+1].operand as FieldInfo == AccessTools.Field(typeof(ThingDefOf), nameof(ThingDefOf.SteamGeyser))    
                        && instrList[i+2].opcode == OpCodes.Bne_Un_S)                                                      // IL 007A: bne.un.s IL_0087
                    {
                        instrList[i].opcode   = OpCodes.Nop;
                        instrList[i+1].opcode = OpCodes.Nop;
                        instrList[i+2].opcode = OpCodes.Nop;
                    }
                }
                foreach (CodeInstruction ci in instrList) yield return ci;
            }
        }

        [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.BlocksConstruction), new Type[] { typeof(Thing), typeof(Thing) })]
        class GenConstructBlocksConstruction
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> CanPlaceBlueprintOverTranspiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> instrList = instructions.ToList();
                for (int i = 0; i < instructions.Count(); i++)
                {
                    // Looking for the first half of (t.def == ThingDefOf.SteamGeyser && thingDef.entityDefToBuild.ForceAllowPlaceOver(t.def))
                    if (instrList[i].opcode == OpCodes.Ldarg_1                                                         // IL 00D1: ldarg.1
                        && instrList[i+1].opcode == OpCodes.Ldfld                                                      // IL 00D2: ldfld class Verse.ThingDef Verse.Thing::def
                        && instrList[i+2].opcode == OpCodes.Ldsfld                                                     // IL 00D7: ldsfld class Verse.ThingDef RimWorld.ThingDefOf::SteamGeyser
                        && instrList[i+2].operand as FieldInfo == AccessTools.Field(typeof(ThingDefOf), nameof(ThingDefOf.SteamGeyser))
                        && instrList[i+3].opcode == OpCodes.Bne_Un_S)                                                // IL 00DC: bne.un.s IL_0087
                    {
                        instrList[i].opcode   = OpCodes.Nop;
                        instrList[i+1].opcode = OpCodes.Nop;
                        instrList[i+2].opcode = OpCodes.Nop;
                        instrList[i+3].opcode = OpCodes.Nop;
                    }
                }
                foreach (CodeInstruction ci in instrList) yield return ci;
            }
        }
    }
}
