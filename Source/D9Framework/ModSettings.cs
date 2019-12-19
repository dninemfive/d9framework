using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace D9Framework
{
    public class D9FModSettings : ModSettings
    {
        public static bool DEBUG = true; //for release set false by default

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DEBUG, "debug", false);
        }
    }
    public class D9Framework : Mod
    {
        D9FModSettings settings;
        public D9Framework(ModContentPack con) : base(con)
        {
            this.settings = GetSettings<D9FModSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("D9FSettingsDebug".Translate(), ref D9FModSettings.DEBUG, "D9FSettingsDebugTooltip".Translate());
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "D9FSettingsCategory".Translate();
        }
    }
}
