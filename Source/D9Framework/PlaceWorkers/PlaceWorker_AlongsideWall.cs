using RimWorld;
using Verse;

namespace D9Framework
{
    /// <summary>
    /// Only allows an object to be placed against a wall
    /// <para>(Checks the cell behind this object)</para>
    /// <para>Originally by CuproPanda, for Additional Joy Objects.</para>
    /// </summary>
    public class PlaceWorker_AlongsideWall : PlaceWorker
    {
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {        
            IntVec3 c = loc - rot.FacingCell;                         // Get the tile behind this object
            Building edifice = c.GetEdifice(map);                     // Determine if the tile is an edifice            
            if (!c.InBounds(map) || !loc.InBounds(map)) return false; // Don't place outside of the map
            /*  // Only allow placing on a natural or constructed wall
            if (edifice == null || edifice.def == null || (edifice.def != ThingDefOf.Wall && !edifice.def.building.isNaturalRock &&
                ((edifice.Faction == null || edifice.Faction != Faction.OfPlayer) ||
                edifice.def.graphicData == null || edifice.def.graphicData.linkFlags == 0 || (LinkFlags.Wall & edifice.def.graphicData.linkFlags) == LinkFlags.None))) {
                return new AcceptanceReport("AJO_MustBePlacedOnWall".Translate(new object[] { checkingDef.LabelCap }));
            }
            */
            if (edifice == null || 
                edifice.def == null || 
                ((edifice.def != ThingDefOf.Wall && !edifice.def.IsSmoothed) && 
                ((edifice.Faction == null || edifice.Faction != Faction.OfPlayer) ||
                edifice.def.graphicData == null || edifice.def.graphicData.linkFlags == 0 || (LinkFlags.Wall & edifice.def.graphicData.linkFlags) == LinkFlags.None)))
                    return new AcceptanceReport("AJO_MustBePlacedOnWall".Translate(checkingDef.LabelCap));        
            return true; // Otherwise, accept placing
        }
  }
}

