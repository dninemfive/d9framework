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
                for(int i = 0; i < instructions.Count(); i++)
                {
                    // Looking for the first half of (oldDef == ThingDefOf.SteamGeyser && !newDef.ForceAllowPlaceOver(oldDef))
                    if (instructions.ElementAt(i).opcode == OpCodes.Ldarg                                                               // IL 0074: ldarg.1
                        && instructions.ElementAt(i+1).opcode == OpCodes.Ldsfld                                                         // IL 0075: ldsfld class Verse.ThingDef RimWorld.ThingDefOf::SteamGeyser
                        && instructions.ElementAt(i+1).operand as FieldInfo == AccessTools.Field(typeof(ThingDefOf), nameof(ThingDefOf.SteamGeyser))    
                        && instructions.ElementAt(i+2).opcode == OpCodes.Bne_Un_S)                                                      // IL 007A: bne.un.s IL_0087
                    {
                        // we should be at index 0x74 or so
                        // the number will be indexed once it loops back up
                        // we start at, let's say, 1, and want to skip 2 and 3 and have the next index be 4
                        i = i + 2;
                        continue;
                    } else yield return instructions.ElementAt(i);
                }
            }
        }

        [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.BlocksConstruction), new Type[] { typeof(Thing), typeof(Thing) })]
        class GenConstructBlocksConstruction
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> CanPlaceBlueprintOverTranspiler(IEnumerable<CodeInstruction> instructions)
            {
                for (int i = 0; i < instructions.Count(); i++)
                {
                    // Looking for the first half of (t.def == ThingDefOf.SteamGeyser && thingDef.entityDefToBuild.ForceAllowPlaceOver(t.def))
                    if (instructions.ElementAt(i).opcode == OpCodes.Ldarg                                                           // IL 00D1: ldarg.1
                        && instructions.ElementAt(i+1).opcode == OpCodes.Ldfld                                                      // IL 00D2: ldfld class Verse.ThingDef Verse.Thing::def
                        && instructions.ElementAt(i + 2).opcode == OpCodes.Ldsfld                                                   // IL 00D7: ldsfld class Verse.ThingDef RimWorld.ThingDefOf::SteamGeyser
                        && instructions.ElementAt(i + 2).operand as FieldInfo == AccessTools.Field(typeof(ThingDefOf), nameof(ThingDefOf.SteamGeyser))
                        && instructions.ElementAt(i + 3).opcode == OpCodes.Bne_Un_S)                                                // IL 00DC: bne.un.s IL_0087
                    {
                        // same reasoning as above, but there are four entries now
                        i = i + 3;
                        continue;
                    }
                    else yield return instructions.ElementAt(i);
                }
            }
        }
    }
}
