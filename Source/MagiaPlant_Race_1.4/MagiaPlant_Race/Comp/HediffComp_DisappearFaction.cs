using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MagiaPlant_Race
{
	public class HediffCompProperties_DisappearFaction : HediffCompProperties
	{
		public HediffCompProperties_DisappearFaction()
		{
			compClass = typeof(HediffComp_DisappearFaction);
		}
	}

	public class HediffComp_DisappearFaction : HediffComp
	{
		public override bool CompShouldRemove
		{
			get
			{
				if (this.Pawn.Faction?.IsPlayer ?? false)
				{
					return false;
				}
				return true;
			}
		}
	}
}
