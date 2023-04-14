using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MagiaPlant_Race
{
	public class Shot_Star : Projectile
	{
		protected override void Impact(Thing hitThing, bool blockedByShield = false)
		{
			if (DestinationCell.InBounds(base.Map))
			{
				base.Position = DestinationCell;
			}
			SoundDef.Named("MgP_Explosion").PlayOneShot(new TargetInfo(this.Position, this.Map, false));
			Thing owner = this.launcher;
			CellRect cellRect = CellRect.CenteredOn(base.Position, 5);
			cellRect.ClipInsideMap(Map);
			for (int k = 0; k < 10; k++)
			{
				IntVec3 randomCell = cellRect.RandomCell;
				MoteMaker.MakeStaticMote(randomCell, this.Map, ThingDef.Named("MgP_Mote_EMP"), 2.0f);
			}
			List<Pawn> press = Map.mapPawns.AllPawnsSpawned.Where(x => x.Position.DistanceTo(this.Position) <= 5.0f).ToList();
			for (int j = press.Count() - 1; j >= 0; j--)
			{
				press.ElementAt(j).TakeDamage(new DamageInfo(DamageDefOf.Burn, this.DamageAmount, this.ArmorPenetration, -1, owner, null, equipmentDef));
			}
			base.Impact(hitThing, false);
		}

	}
}
