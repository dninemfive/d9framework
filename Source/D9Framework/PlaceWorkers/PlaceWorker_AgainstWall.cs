using System.Collections.Generic;
using RimWorld;
using Verse;

namespace D9Framework
{
    /// <summary>
    /// Only allows an object to be placed against a wall (Checks the cell behind this object)
    /// <para>Originally by CuproPanda, for Additional Joy Objects.</para>
    /// </summary>
    public class PlaceWorker_AgainstWall : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            IntVec3 c = loc - rot.FacingCell;                               // Get the tile behind this object
            Building edifice = c.GetEdifice(map);                           // Determine if the tile is an edifice            
            if (!c.InBounds(map) || !loc.InBounds(map)) return false;       // Don't place outside of the map
            if (!IsWall(edifice) || edifice.Faction != Faction.OfPlayer)    // Only allow placing on walls, and not if another faction owns the wall
                return new AcceptanceReport("D9F_MustBePlacedOnWall".Translate(checkingDef.LabelCap));
            return true;                                                    // Otherwise, accept placing
        }
        public bool IsWall(Building b)
        {
            ThingDef def = b?.def;
            return def != null && (def.holdsRoof && def.blockLight && def.coversFloor);
        }
    }
    /// <summary>
    /// Only allows an object to be placed on a wall, as long as there isn't the same def facing the same way.
    /// </summary>
    public class PlaceWorker_OnWall : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            Building buil = loc.GetEdifice(map);
            if (!loc.InBounds(map)) return false;
            if (!IsWall(buil) || buil.Faction != Faction.OfPlayer) return new AcceptanceReport("D9F_MustBePlacedOnWall".Translate(checkingDef.LabelCap));
            if (ConflictingThing(checkingDef, loc, rot, map)) return new AcceptanceReport("IdenticalThingExists".Translate());
            return true;
        }
        public bool IsWall(Building b)
        {
            ThingDef def = b?.def;
            return def != null && (def.holdsRoof && def.blockLight && def.coversFloor);
        }
        public bool ConflictingThing(BuildableDef bd, IntVec3 c, Rot4 r, Map map)
        {
            List<Thing> things = map.thingGrid.ThingsListAtFast(c);
            foreach (Thing t in things) if (t.def as BuildableDef == bd && t.Rotation == r) return false;
            return true;
        }
    }
}

