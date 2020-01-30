using RimWorld;
using Verse;
using Verse.Sound;

namespace D9Framework {
    /// <summary>
    /// Minifies a minifiable object if the thing it's hanging on is missing
    /// </summary>
    public class CompHanger : CompWithCheapHashInterval
    {
        CompProperties_Hanger Props => (CompProperties_Hanger)base.props;
        /// <summary> Checks for a disconnection, then minifies an object if needed</summary>
        public override void CompTick()
        {
            if (Props.ShouldUse && IsCheapIntervalTick(250))
            {
                if (Props.hangingType == HangingType.Wall)
                {
                    // Get the tile behind this object
                    IntVec3 c = parent.Position - parent.Rotation.FacingCell;
                    // Minify this if the wall is missing
                    Building edifice = c.GetEdifice(parent.Map);
                    if (!PlaceWorkerUtility.IsWall(edifice)) Minify();
                }

                if (Props.hangingType == HangingType.Ceiling)
                {
                    // Minify this if the ceiling is missing
                    int occCells = 0;
                    int unroofedCells = 0;
                    foreach (IntVec3 current in parent.OccupiedRect())
                    {
                        occCells++;
                        if (!parent.Map.roofGrid.Roofed(current)) unroofedCells++;
                        if (current.GetEdifice(parent.Map) == null || current.GetEdifice(parent.Map).def == null) continue;
                        if (current.GetEdifice(parent.Map).def.blockWind == true || current.GetEdifice(parent.Map).def.holdsRoof == true) Minify();
                    }
                    if (((float)(occCells - unroofedCells) / occCells) < 0.5f) Minify();
                }
            }
        }

        /// <summary> Minifies the package</summary>
        public virtual void Minify()
        {
            Map map = parent.Map;
            MinifiedThing package = MinifyUtility.MakeMinified(parent);
            GenPlace.TryPlaceThing(package, parent.Position, map, ThingPlaceMode.Near);
            SoundDef.Named("ThingUninstalled").PlayOneShot(new TargetInfo(parent.Position, map));
        }

        public enum HangingType
        {
            Invalid,
            Wall,
            Ceiling
        }
    }
}
