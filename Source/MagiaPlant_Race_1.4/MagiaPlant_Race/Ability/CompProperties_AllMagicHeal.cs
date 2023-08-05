using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorannMagic;
using Verse;

namespace MagiaPlant_Race
{

    /// <summary>
    /// マナと魔力の衰弱を治癒、すべてのクールダウンを削除
    /// </summary>
    public class CompProperties_AllMagicHeal : CompProperties_AbilityEffect
	{
		public CompProperties_AllMagicHeal()
		{
			compClass = typeof(CompAbilityEffect_AllMagicHeal);
		}
	}

	// 回復
	public class CompAbilityEffect_AllMagicHeal : CompAbilityEffect
	{
		public new CompProperties_AllMagicHeal Props => (CompProperties_AllMagicHeal)props;


		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				CompAbilityUserMagic comp_magic = pawn.GetComp<CompAbilityUserMagic>();
				if (comp_magic.IsMagicUser)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			ApplyInner(target.Pawn, parent.pawn);
		}

		protected void ApplyInner(Pawn target, Pawn caster)
		{
			if (target != null)
			{
				CompAbilityUserMagic comp_magic = target.GetComp<CompAbilityUserMagic>();
				if (comp_magic.IsMagicUser)
				{
					Need_Mana needmana = target.needs.TryGetNeed<Need_Mana>();
					if (needmana != null)
					{
						needmana.CurLevel = needmana.MaxLevel;
					}
					Hediff hediff = target.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("TM_ArcaneWeakness"));
					if (hediff != null)
					{
						hediff.Heal(10.0f);
					}
					foreach (PawnAbility allPower in comp_magic.AbilityData.AllPowers)
					{
						if (allPower.CooldownTicksLeft > 0)
						{
							allPower.CooldownTicksLeft = 0;
						}
					}
				}
			}
		}
	}
}
