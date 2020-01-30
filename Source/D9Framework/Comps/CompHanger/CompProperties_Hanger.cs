using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace D9Framework {
    public class CompProperties_Hanger : CompProperties
    {
        private bool shouldUse = true;
        public bool ShouldUse => shouldUse;

        public CompHanger.HangingType hangingType;

        public CompProperties_Hanger() {
            compClass = typeof(CompHanger);
        }
        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            Func<PlaceWorker, bool> IsValid() => delegate(PlaceWorker pw)
            {
                return pw is PlaceWorker_AgainstWall
                    || pw is PlaceWorker_OnWall 
                    || pw is PlaceWorker_Roofed
                    || pw is PlaceWorker_RoofHanger;
            };
            List<PlaceWorker> validPWs = parentDef.PlaceWorkers.Where(IsValid()).ToList();
            if (validPWs.Count <= 0)
            {                
                shouldUse = false;
                hangingType = CompHanger.HangingType.Invalid;
                yield return "CompHanger is used but no appropriate PlaceWorkers are provided.";
            }
            else if (validPWs.Count > 1)
            {                
                shouldUse = false;
                hangingType = CompHanger.HangingType.Invalid;
                yield return "CompHanger is used on an object with multiple conflicting PlaceWorkers.";
            }
            else
            {
                PlaceWorker validPW = validPWs[0];
                if(validPW is PlaceWorker_AgainstWall || validPW is PlaceWorker_OnWall)
                {
                    hangingType = CompHanger.HangingType.Wall;
                }
                else if(validPW is PlaceWorker_Roofed || validPW is PlaceWorker_RoofHanger)
                {
                    hangingType = CompHanger.HangingType.Ceiling;
                }
            }
        }
    }
}
