using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MagiaPlant_Race
{
	public class HediffCompProperties_Heal : HediffCompProperties
	{
		public HediffCompProperties_Heal()
		{
			compClass = typeof(HediffComp_Heal);
		}
	}

	public class HediffComp_Heal : HediffComp
	{
		public HediffCompProperties_Heal Props => (HediffCompProperties_Heal)props;

		public override void CompPostTick(ref float severityAdjustment)
		{
			Pawn comppawn = this.Pawn;
			if (comppawn.IsHashIntervalTick(60))
            {
				if (Util.HealInjury(comppawn, 1))
                {
                    comppawn.TryGetComp<Comp_MagiaPlant>().UseMS(3);
                }
                comppawn.TryGetComp<Comp_MagiaPlant>().UseMS(Util.HealFirstMissingBodyPart(comppawn));
            }
		}
	}

    public class HediffCompProperties_Runaway : HediffCompProperties
    {
        public HediffCompProperties_Runaway()
        {
            compClass = typeof(HediffComp_Runaway);
        }
    }

    public class HediffComp_Runaway : HediffComp
    {
        public HediffCompProperties_Runaway Props => (HediffCompProperties_Runaway)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            Pawn comppawn = this.Pawn;
            if (comppawn.needs.mood.CurLevel > 0.15f)
            {
                comppawn.needs.mood.CurLevel = 0.15f;
            }
            if (comppawn.IsHashIntervalTick(60))
            {
                comppawn.TryGetComp<Comp_MagiaPlant>().UseMS(9);
            }
        }
    }

    public static class Util
    {
        // 傷と傷跡を治癒
        public static bool HealInjury(Pawn pawn, int amount)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            bool flag = false;
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                Hediff_Injury hediff_Injury = hediffs[i] as Hediff_Injury;
                if (hediff_Injury != null && hediff_Injury.Visible && hediff_Injury.def.everCurableByItem)
                {
                    hediff_Injury.Heal(amount);
                    flag = true;
                }
            }
            return flag;
        }

        public static int HealFirstMissingBodyPart(Pawn pawn)
        {
            List<Hediff_MissingPart> hediffs = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
            int flag = 0;
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                pawn.health.RemoveHediff(hediffs[i]);
                flag += 8;
            }
            return flag;
        }

    }

}
