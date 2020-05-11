using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace D9Framework
{
    public class D9FModSettings : ModSettings
    {        
        public static bool DEBUG = false; //for release set false by default

        public static bool ApplyCompFromStuff => !DEBUG || applyCFS;
        public static bool ApplyOrbitalTradeHook => !DEBUG || applyOTH;
        public static bool ApplyDeconstructReturnFix => !DEBUG || applyDRF;
        public static bool ApplyForceAllowPlaceOverFix => applyFAF;
        public static bool ApplyCarryMassFramework => !DEBUG || applyCMF;
        public static bool PrintPatchedMethods => DEBUG && printPatchedMethods;
        // despite the public flag, don't reference these; they're only public for the purposes of the mod settings screen below. Reference the above variables instead.
        public static bool applyCFS = true, applyOTH = true, applyDRF = true, applyFAF = false, applyCMF = true;
        public static bool printPatchedMethods = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DEBUG, "debug", false);
            Scribe_Values.Look(ref applyCFS, "ApplyCompFromStuff", true);
            Scribe_Values.Look(ref applyOTH, "ApplyOrbitalTradeHook", true);
            Scribe_Values.Look(ref applyDRF, "ApplyDeconstructReturnFix", true);
            Scribe_Values.Look(ref applyFAF, "ApplyForceAllowPlaceOverFix", false);
            Scribe_Values.Look(ref applyCMF, "ApplyCarryMassFramework", true);
        }
    }
    public class D9FrameworkMod : Mod
    {
        D9FModSettings settings;
        public D9FrameworkMod(ModContentPack con) : base(con)
        {
            this.settings = GetSettings<D9FModSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("D9FSettingsDebug".Translate(), ref D9FModSettings.DEBUG, "D9FSettingsDebugTooltip".Translate());
            if (D9FModSettings.DEBUG)
            {
                listing.CheckboxLabeled("D9FSettingsPPM".Translate(), ref D9FModSettings.printPatchedMethods, "D9FSettingsPPMTooltip".Translate());
                listing.Label("D9FSettingsApplyAtOwnRisk".Translate());
                listing.Label("D9FSettingsRestartToApply".Translate());
                listing.Label("D9FSettingsDebugModeRequired".Translate());
                listing.CheckboxLabeled("D9FSettingsApplyCFS".Translate(), ref D9FModSettings.applyCFS, "D9FSettingsApplyCFSTooltip".Translate());
                listing.CheckboxLabeled("D9FSettingsApplyOTH".Translate(), ref D9FModSettings.applyOTH, "D9FSettingsApplyOTHTooltip".Translate());
                listing.CheckboxLabeled("D9FSettingsApplyDRF".Translate(), ref D9FModSettings.applyDRF, "D9FSettingsApplyDRFTooltip".Translate());
                listing.CheckboxLabeled("D9FSettingsApplyFAF".Translate(), ref D9FModSettings.applyFAF, "D9FSettingsApplyFAFTooltip".Translate());
                listing.CheckboxLabeled("D9FSettingsApplyCMF".Translate(), ref D9FModSettings.applyCMF, "D9FSettingsApplyCMFTooltip".Translate());                
            }
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "D9FSettingsCategory".Translate();
        }
    }
}
