using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace D9Framework
{
    /// <summary>
    /// Hook allowing modders to easily create custom trade beacons by extending <c>D9Framework.Building_OrbitalTradeBeacon</c>
    /// </summary>
    // TODO: convert to transpilers smh
    [StaticConstructorOnStartup]
    public class OrbitalTradeHook
    {
        static OrbitalTradeHook()
        {
            var harmony = HarmonyInstance.Create("com.dninemfive.d9framework.oth");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            ULog.DebugMessage("Orbital Trade Hook loaded.");
        }

        public static IEnumerable<Building> AllPowered(Map map)
        {
            foreach (RimWorld.Building_OrbitalTradeBeacon b in RimWorld.Building_OrbitalTradeBeacon.AllPowered(map)) yield return b; 
            foreach(D9Framework.Building_OrbitalTradeBeacon b in map.listerBuildings.AllBuildingsColonistOfClass<D9Framework.Building_OrbitalTradeBeacon>())
            {                
                CompPowerTrader power = b.GetComp<CompPowerTrader>();
                CompRefuelable fuel = b.GetComp<CompRefuelable>();
                if ((power == null || (power != null && power.PowerOn)) && (fuel == null || (fuel != null && fuel.HasFuel))) yield return b;                
            }
        }

        public static IEnumerable<IntVec3> AllTradeableCells(Map map)
        {
            foreach(Building b in AllPowered(map))
            {
                RimWorld.Building_OrbitalTradeBeacon rb = (RimWorld.Building_OrbitalTradeBeacon)b;
                if (rb != null) foreach (IntVec3 cell in rb.TradeableCells) yield return cell;
                D9Framework.Building_OrbitalTradeBeacon ob = b as D9Framework.Building_OrbitalTradeBeacon;
                if (ob != null) foreach (IntVec3 cell in ob.TradeableCells()) yield return cell;
            }
        }
        [HarmonyPatch(typeof(PassingShip))]
        [HarmonyPatch("CommFloatMenuOption")]
        [HarmonyPatch(new Type[] { typeof(Building_CommsConsole), typeof(Pawn) })]
        class CommFloatMenuOptionHook
        {
            [HarmonyPrefix]
            public static bool CommFloatMenuOptionPrefix(Building_CommsConsole console, Pawn negotiator, PassingShip __instance, ref FloatMenuOption __result)
            {
                string label = "CallOnRadio".Translate(__instance.GetCallLabel());
                Action action = delegate
                {
                    if (!AllPowered(__instance.Map).Any())
                    {
                        Messages.Message("MessageNeedBeaconToTradeWithShip".Translate(), console, MessageTypeDefOf.RejectInput, false);
                    }
                    else
                    {
                        console.GiveUseCommsJob(negotiator, __instance);
                    }
                };
                __result = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, MenuOptionPriority.InitiateSocial, null, null, 0f, null, null), negotiator, console, "ReservedBy");
                return false;
            }

            // instead of a destructive prefix, just replace two IL calls
            // thanks to Smash Phil bc I used a Boats transpiler as an example
            /*[HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> CommFloatMenuOptionTranspiler(IEnumerable<CodeInstruction> instr)
            {
                for(int i = 0; i < instr.Count(); i++)
                {
                    CodeInstruction ci = instr.ElementAt(i);
                    if(ci.opcode == OpCodes.Call && ci.operand == 
                }
            }*///end
            
        }
        [HarmonyPatch(typeof(TradeUtility))]
        [HarmonyPatch("AllLaunchableThingsForTrade")]
        [HarmonyPatch(new Type[] { typeof(Map) })]
        class AllLaunchableThingsForTradeHook
        {
            [HarmonyPrefix]
            public static bool AllLaunchableThingsForTradePrefix(Map map, ref IEnumerable<Thing> __result)
            {
                __result = AllLaunchableThingsForTrade(map);
                return false;
            }
            private static IEnumerable<Thing> AllLaunchableThingsForTrade(Map map)
            {
                HashSet<Thing> yielded = new HashSet<Thing>();
                foreach (IntVec3 cell in AllTradeableCells(map))
                {
                    List<Thing> thingList = cell.GetThingList(map);
                    for (int i = 0; i < thingList.Count(); i++)
                    {
                        Thing t = thingList[i];
                        if (t.def.category == ThingCategory.Item && TradeUtility.PlayerSellableNow(t) && !yielded.Contains(t))
                        {
                            yielded.Add(t);
                            yield return t;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(TradeUtility))]
        [HarmonyPatch("LaunchThingsOfType")]
        [HarmonyPatch(new Type[] { typeof(ThingDef), typeof(int), typeof(Map), typeof(TradeShip) })]
        class LaunchThingsOfTypeHook
        {
            [HarmonyPrefix]
            public static bool LaunchThingsOfTypePrefix(ThingDef resDef, int debt, Map map, TradeShip trader)
            {
                return false;
            }
            [HarmonyPostfix]
            public static void LaunchThingsOfTypePostfix(ThingDef resDef, int debt, Map map, TradeShip trader)
            {
                while (debt > 0)
                {
                    Thing thing = null;
                    foreach (IntVec3 current2 in AllTradeableCells(map))
                    {
                        foreach (Thing current3 in map.thingGrid.ThingsAt(current2))
                        {
                            if (current3.def == resDef)
                            {
                                thing = current3;
                                goto IL_CC;
                            }
                        }
                    }
                    IL_CC:
                    if (thing == null)
                    {
                        ULog.Error("Orbital Trade Hook: Could not find any " + resDef + " to transfer to trader.", false);
                        break;
                    }
                    int num = Math.Min(debt, thing.stackCount);
                    if (trader != null)
                    {
                        trader.GiveSoldThingToTrader(thing, num, TradeSession.playerNegotiator);
                    }
                    else
                    {
                        thing.SplitOff(num).Destroy(DestroyMode.Vanish);
                    }
                    debt -= num;
                }
            }
            //TODO: patch DropCellFinder.TradeDropSpot() to find any cell near either base or this OrbitalTradeBeacon. Not super high priority since it always returns something, but would be nice
        }
    }
}
