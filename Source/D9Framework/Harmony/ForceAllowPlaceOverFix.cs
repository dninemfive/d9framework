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
                        Log.Message("Are you there, God? It's me, Margaret.");
                        /*// we want to not return these CodeInstructions
                        // the number will be indexed once it loops back up
                        // we start at, let's say, 1, and want to skip 2 and 3 and have the next index be 4
                        i += 2;
                        // we need to add an equivalent number of nops to avoid breaking jump statements elsewhere
                        for (int j = 0; j < 3; j++) yield return new CodeInstruction(OpCodes.Nop);
                        continue;*/
                        instrList[i] = new CodeInstruction(OpCodes.Nop);
                        instrList[i + 1] = new CodeInstruction(OpCodes.Nop);
                        instrList[i + 2] = new CodeInstruction(OpCodes.Nop);
                    } //else yield return instructions.ElementAt(i);
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
                        Log.Message("Are you scared, God? You should be.");
                        /*                        
                        // same reasoning as above, but there are four entries now
                        i += 3;
                        // ditto
                        for (int j = 0; j < 4; j++) yield return new CodeInstruction(OpCodes.Nop);
                        continue;
                        */
                        instrList[i]   = new CodeInstruction(OpCodes.Nop);
                        instrList[i+1] = new CodeInstruction(OpCodes.Nop);
                        instrList[i+2] = new CodeInstruction(OpCodes.Nop);
                        instrList[i+3] = new CodeInstruction(OpCodes.Nop);
                    }
                    // else yield return instructions.ElementAt(i);
                }
                foreach (CodeInstruction ci in instrList) yield return ci;
            }
        }
    }
}
