using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MagiaPlant_Race
{
	public class Comp_Generator : CompPowerPlant
	{
		private int powertick = 0;
		private int count = 0;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref powertick, "Powertick", 0);
			Scribe_Values.Look(ref count, "Count", 0);
		}

		public override void PostPostMake()
		{
			base.PostPostMake();
			base.PowerOutput = 0f;
		}

		public override void CompTick()
		{
			base.CompTick();
			UpdatePowerOutput();
		}

		public void UpdatePowerOutput()
		{
			if ((flickableComp != null && !flickableComp.SwitchIsOn) || !base.PowerOn)
			{
				base.PowerOutput = 0f;
				powertick = 0;
				return;
			}
			if (powertick > 0)
            {
				powertick--;
				base.PowerOutput *= 5f;
				return;
			}
			if (this.parent.IsHashIntervalTick(300))
            {
				List<Pawn> pawns = this.parent.Map.mapPawns.AllPawnsSpawned.Where(x => x.def.defName == "MagiaPlant_Pawn").ToList();
				count = Math.Min(pawns.Count(), 5);
				if (!pawns.EnumerableNullOrEmpty())
				{
					for (int i = pawns.Count() - 1; i >= 0; i--)
					{
						pawns.ElementAt(i).TryGetComp<Comp_MagiaPlant>().UseMS(2);
					}
				}
			}
			base.PowerOutput *= count;
		}

		public void addFuel()
		{
			powertick += 180000;
			return;
		}

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.CompInspectStringExtra() + "\n");
			stringBuilder.Append("MagiaPlant.Build.GeneratorFual".Translate() + powertick.TicksToSeconds().ToString());
			return stringBuilder.ToString();
		}

	}
}
