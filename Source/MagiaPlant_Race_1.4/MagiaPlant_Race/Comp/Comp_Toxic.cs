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
	public class CompProperties_ToxicMonster : CompProperties
	{
		public CompProperties_ToxicMonster()
		{
			compClass = typeof(Comp_ToxicMonster);
		}
	}

    public class Comp_ToxicMonster : ThingComp
    {

        // 消滅時間
        public int DestoryTick = 0;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref DestoryTick, "DestoryTick");
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (this.parent.Destroyed || this.parent.Map == null)
            {
                return;
            }
            MoteMaker.MakeStaticMote(this.parent.Position, this.parent.Map, ThingDef.Named("MgP_Mote_Tentacle"), 10.0f);
            List<Thing> things = this.parent.Map.listerThings.AllThings.Where(x => x.Position.DistanceTo(this.parent.Position) <= 5.5f && x != this.parent).ToList();
            if (!things.EnumerableNullOrEmpty())
            {
                for (int i = things.Count() - 1; i >= 0; i--)
                {
                    things.ElementAt(i).TakeDamage(new DamageInfo(DefDatabase<DamageDef>.GetNamed("ScratchToxic"), 50.0f, 1.5f, -1, this.parent, null, this.parent.def));
                }
            }
        }

        public override void CompTick()
        {
            DestoryTick++;
            if (DestoryTick >= 60000)
            {
                SoundDef.Named("MgP_Trans").PlayOneShot(this.parent);
                Messages.Message("MgP.UI.TransEnd".Translate(), MessageTypeDefOf.PositiveEvent, false);
                if (!this.parent.Destroyed)
                {
                    this.parent.Destroy();
                }
            }
        }
    }
}