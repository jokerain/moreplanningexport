using System;
using Verse;

namespace MorePlanningExport.Data
{
    public class PlanInfoData : IEquatable<PlanInfoData>, IExposable
    {
        public IntVec3 Pos;

        public int Color;

        public PlanInfoData()
        {
        }

        public PlanInfoData(IntVec3 pos, int color)
        {
            Pos = pos;
            Color = color;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref Pos, "Pos");
            Scribe_Values.Look(ref Color, "Color");
        }

        public bool Equals(PlanInfoData other)
        {
            return other != null && other.Pos.Equals(Pos) && other.Color == Color;
        }
    }
}