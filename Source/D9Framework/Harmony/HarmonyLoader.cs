using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Verse;
using HarmonyLib;

namespace D9Framework
{
    [StaticConstructorOnStartup]
    class HarmonyLoader
    {
        static HarmonyLoader()
        {
            ULog.Message("Applying Harmony patches...");
            var harmony = new Harmony("com.dninemfive.D9Framework");
            if (D9FModSettings.ApplyCompFromStuff)
            {
                PatchAll(harmony, typeof(CompFromStuff));                
                ULog.DebugMessage("\tCompFromStuff enabled.", false);
            }
            if (D9FModSettings.ApplyDeconstructReturnFix)
            {
                PatchAll(harmony, typeof(DeconstructReturnFix));
                ULog.DebugMessage("\tDeconstruct Return Fix enabled.", false);
            }
            if (D9FModSettings.ApplyOrbitalTradeHook)
            {
                PatchAll(harmony, typeof(OrbitalTradeHook));
                ULog.DebugMessage("\tOrbital Trade Hook enabled.", false);
            }
            if (D9FModSettings.DEBUG)
            {
                Log.Message("The following methods were successfully patched:", false);
                foreach (MethodBase mb in harmony.GetPatchedMethods()) Log.Message("\t" + mb.DeclaringType.Name + "." + mb.Name, false);
            }            
        }

        // also thanks to lbmaian
        static void PatchAll(Harmony harmony, Type parentType)
        {
            foreach (var type in parentType.GetNestedTypes(AccessTools.all))
            {
                new PatchClassProcessor(harmony, type).Patch();
            }
        }
    }
}
