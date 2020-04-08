using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace D9Framework
{
    class PlaceWorkerUtility
    {
        public static bool IsWall(Building b)
        {
            ThingDef def = b?.def;
            return def != null && (def.holdsRoof && def.blockLight && def.coversFloor);
        }
        public static bool ConflictingThing(BuildableDef bd, IntVec3 c, Rot4 r, Map map)
        {
            List<Thing> things = map.thingGrid.ThingsListAtFast(c);            
            foreach (Thing t in things) if (t.def as BuildableDef == bd && t.Rotation == r) return true; 
            return false;
        }
    }
}
