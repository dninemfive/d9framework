using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Harmony;

namespace D9Framework.Harmony
{
    [StaticConstructorOnStartup]
    class HarmonyLoader
    {
        static HarmonyLoader()
        {//
            ULog.Message("Applying Harmony patches...");
            var harmony = HarmonyInstance.Create("com.dninemfive.D9Framework");
            if (D9FModSettings.ApplyCompFromStuff)
            {
                PatchAll(harmony, typeof(CompFromStuff));                
                ULog.DebugMessage("\tCompFromStuff loaded.", false);
            }
            if (D9FModSettings.ApplyDeconstructReturnFix)
            {
                PatchAll(harmony, typeof(DeconstructReturnFix));
                ULog.DebugMessage("\tDeconstruct Return Fix loaded.", false);
            }
            if (D9FModSettings.ApplyOrbitalTradeHook)
            {
                PatchAll(harmony, typeof(OrbitalTradeHook));
                ULog.DebugMessage("\tOrbital Trade Hook loaded.", false);
            }
        }

        // Thanks to lbmaian for this
        static void PatchAll(HarmonyInstance harmony, Type parentType)
        {
            foreach (var type in parentType.GetNestedTypes(AccessTools.all))
            {
                // Following copied from HarmonyInstance.PatchAll(Assembly).
                var harmonyMethods = type.GetHarmonyMethods();
                if (harmonyMethods != null && harmonyMethods.Count > 0)
                {
                    var attributes = HarmonyMethod.Merge(harmonyMethods);
                    var patchProcessor = new PatchProcessor(harmony, type, attributes);
                    patchProcessor.Patch();
                }
            }
        }
    }
}
