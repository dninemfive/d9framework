using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace D9Framework
{
    /// <summary>
    /// Restrict buildings to being placed under a roof
    /// </summary>
    /// <para>Originally by CuproPanda, for Additional Joy Objects.</para>
    public class PlaceWorker_Roofed : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            foreach (IntVec3 current in GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size))
            {
                if (!map.roofGrid.Roofed(current))
                {
                    return new AcceptanceReport("D9F_Roofed_NeedsRoof".Translate(checkingDef.label));
                }
            }
            return true;
        }
    }
    /// <summary>
    /// Require that the building be placed "hanging" from a roof, i.e. that the tile is roofed, it's not above anything which touches the roof, and it's not on the same tile as another RoofHanger
    /// </summary>
    /// <para>Originally by CuproPanda, for Additional Joy Objects.</para>
    public class PlaceWorker_RoofHanger : PlaceWorker
    {
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null) {
            AcceptanceReport roofedReport = base.AllowsPlacing(checkingDef, loc, rot, map, thingToIgnore); //check if tile is roofed
            if (!roofedReport.Accepted) return roofedReport;            
            foreach (IntVec3 c in GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size)) // Don't allow placing on big things
            {
                if (c.GetEdifice(map) != null)
                {
                    if (c.GetEdifice(map).def.blockWind == true || c.GetEdifice(map).def.holdsRoof == true)
                    {
                        return new AcceptanceReport("D9F_Chandelier_TooTall".Translate(c.GetEdifice(map).LabelCap, checkingDef.LabelCap));
                    }
                }
                IEnumerable<Thing> things = c.GetThingList(map);
                if (things.Where(x => x.def.placeWorkers.Where(y => y.GetType() == typeof(PlaceWorker_RoofHanger)).Any()).Any()) // don't hang if there's already a chandelier here
                {
                    return new AcceptanceReport("IdenticalThingExists".Translate());
                }
            }
            // Otherwise, accept placing
            return true;
        }
    }
}
