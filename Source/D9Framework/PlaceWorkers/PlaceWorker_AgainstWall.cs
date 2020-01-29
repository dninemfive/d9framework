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
    public class PlaceWorker_OnWall : PlaceWorker
    {
        // on wall and not on (the same ThingDef facing the same way)
    }
}

