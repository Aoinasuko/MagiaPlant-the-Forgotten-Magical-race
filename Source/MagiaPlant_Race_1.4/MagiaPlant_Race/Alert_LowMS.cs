using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace MagiaPlant_Race
{
    public class Alert_LowMS : Alert
	{
		private const float NutritionThresholdPerColonist = 4f;

		private int lastActiveFrame = -1;

		public IEnumerable<Pawn> MidMSPawns => PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep.Where(delegate (Pawn pawn)
		{
			if (pawn.def.defName == "MagiaPlant_Pawn")
            {
				if (pawn.TryGetComp<Comp_MagiaPlant>().NowMS <= 500 && pawn.TryGetComp<Comp_MagiaPlant>().NowMS > 100)
                {
					return true;
                }
            }
			return false;
		});

		public IEnumerable<Pawn> LowMSPawns => PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep.Where(delegate (Pawn pawn)
		{
			if (pawn.def.defName == "MagiaPlant_Pawn")
			{
				if (pawn.TryGetComp<Comp_MagiaPlant>().NowMS <= 100)
                {
					return true;
				}
			}
			return false;
		});

		public Alert_LowMS()
		{
			defaultLabel = "MagiaPlant.UI.AlertMS_Label".Translate();
			defaultPriority = AlertPriority.High;
		}

		public bool IsCritical => LowMSPawns.Any();

		protected override Color BGColor
		{
			get
			{
				if (IsCritical)
				{
					float num = Pulser.PulseBrightness(0.5f, Pulser.PulseBrightness(0.5f, 0.6f));
					return new Color(num, num, num) * Color.red;
				}
				return base.BGColor;
			}
		}

		public override AlertPriority Priority
		{
			get
			{
				if (LowMSPawns.Any())
				{
					return AlertPriority.Critical;
				}
				return AlertPriority.High;
			}
		}

		public override void AlertActiveUpdate()
		{
			if (IsCritical)
            {
				if (lastActiveFrame < Time.frameCount - 1)
				{
					Messages.Message("MessageCriticalAlert".Translate(base.Label.CapitalizeFirst()), new LookTargets(GetReport().AllCulprits), MessageTypeDefOf.ThreatBig);
				}
				lastActiveFrame = Time.frameCount;
			}
		}

		public override TaggedString GetExplanation()
		{
			StringBuilder stringBuilder_Text = new StringBuilder();
			StringBuilder stringBuilder_Low = new StringBuilder();
			if (!LowMSPawns.EnumerableNullOrEmpty())
            {
				foreach (Pawn lowMSPawn in LowMSPawns)
				{
					stringBuilder_Low.AppendLine("  - " + lowMSPawn.NameShortColored.Resolve());
				}
				stringBuilder_Text.Append("MagiaPlant.UI.AlertMS_LowDesc".Translate(stringBuilder_Low).Resolve());
			}

			StringBuilder stringBuilder_Mid = new StringBuilder();
			if (!MidMSPawns.EnumerableNullOrEmpty())
			{
				foreach (Pawn midMSPawn in MidMSPawns)
				{
					stringBuilder_Mid.AppendLine("  - " + midMSPawn.NameShortColored.Resolve());
				}
				stringBuilder_Text.Append("MagiaPlant.UI.AlertMS_MidDesc".Translate(stringBuilder_Mid).Resolve());
			}
			return stringBuilder_Text.ToString();
		}

		public override AlertReport GetReport()
		{
			if (!LowMSPawns.Any() && !MidMSPawns.Any())
			{
				return false;
			}
			List<Pawn> pawns = new List<Pawn>();
			pawns.AddRange(LowMSPawns);
			pawns.AddRange(MidMSPawns);
			AlertReport result = default(AlertReport);
			result.culpritsPawns = pawns;
			result.active = result.AnyCulpritValid;
			return result;
		}
	}
}
