using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;

namespace D9Framework
{
    class ITab_CompSlottable : ITab
    {
        List<CompSlottable.Slot> Slots => SelThing?.TryGetComp<CompSlottable>()?.Slots;
        private List<IStoreSettingsParent> issps = null;
        public List<IStoreSettingsParent> ISSPs
        {
            get
            {
                if (issps != null) return issps;
                issps = new List<IStoreSettingsParent>();
                foreach (CompSlottable.Slot s in Slots) issps.Add(s as IStoreSettingsParent);
                return issps;
            }
        }
        private static readonly Vector2 WinSize = new Vector2(300f, 480f);
        public override bool IsVisible
        {
            get
            {
                if (SelThing?.Faction != null && SelThing?.Faction != Faction.OfPlayer) return false;
                if (ISSPs == null) return false;
                return true;
            }
        }
        public bool ShowPrioritySetting => SelThing?.TryGetComp<CompSlottable>()?.ShowPrioritySetting ?? false;
        public float TopAreaHeight => (float)(ShowPrioritySetting ? 35 : 20); // Needs larger buffer for dropdown if priority shown
        public int CurrentSlotIndex = 0;
        public IStoreSettingsParent CurrentISSP => !ISSPs.NullOrEmpty() ? ISSPs[CurrentSlotIndex] : null;

        public ITab_CompSlottable()
        {
            base.size = WinSize;
            base.labelKey = "D9F_CompSlottable_ITabLabel";
        }

        protected override void FillTab()
        {
            if (ISSPs.NullOrEmpty())
            {
                ULog.Error("ITab_CompSlottable but no slots defined! ThingDef: " + SelThing.def.defName);
                return;
            }
            // place radio buttons on the left
            // fill screen with current selected tab
        }
    }
}