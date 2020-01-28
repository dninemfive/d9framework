using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace D9Framework
{
    /// <summary>
    /// Allows modder to add comps to any items created with a certain Stuff using the <c>CompsToAddWhenStuff</c> <c>ModExtension</c>.
    /// </summary>
    static class CompFromStuff
    {
        [HarmonyPatch(typeof(ThingMaker), "MakeThing", new Type[] { typeof(ThingDef), typeof(ThingDef) })]
        class AddCompPrefix
        {
            [HarmonyPrefix]
            public static void MakeThingPostfix(ref Thing __result)
            {
                if (__result is ThingWithComps twc)
                {
                    ULog.Message("twc: " + twc.ToString());
                    CompsToAddWhenStuff ext = twc.def.GetModExtension<CompsToAddWhenStuff>();
                    if(ext != null && ext.comps != null && ext.comps.Count > 0)
                    {
                        ULog.Message("ext: " + ext.ToString());
                        for(int i = 0; i < ext.comps.Count; i++)
                        {
                            ULog.Message("" + i);
                            ThingComp comp = (ThingComp)Activator.CreateInstance(ext.comps[i].compClass);
                            comp.parent = twc;
                            twc.AllComps.Add(comp);
                            comp.Initialize(ext.comps[i]);
                        }
                    }
                }
            }
        }
    }
}