using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace MagiaPlant_Race
{
    public class Shot_Homing : Bullet
	{
		protected override void Impact(Thing hitThing, bool blockedByShield = false)
		{
			IEnumerable<Pawn> pawns = (IEnumerable<Pawn>)launcher.Map.mapPawns.AllPawnsSpawned.Where(x => (hitThing == null || x != hitThing) && x.HostileTo(launcher) && x.Position.DistanceTo(this.Position) <= 20.9f);
			if (!pawns.EnumerableNullOrEmpty())
			{
				int i = 0;
				foreach (Pawn tag in pawns)
				{
					if (i >= 5)
					{
						break;
					}
					i++;
					SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(this.Position, this.Map));
					Bullet addbullet = (Bullet)GenSpawn.Spawn(ThingDef.Named("MgP_Arrow"), this.Position, this.Map);
					addbullet.Launch(launcher, tag, this.Position, ProjectileHitFlags.All, false);
				}
			}
			base.Impact(hitThing);
		}
	}
}
