using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorannMagic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace MagiaPlant_Race
{

	[StaticConstructorOnStartup]
	public class Gizmo_MagiaPlantStatus : Gizmo
	{
		public Comp_MagiaPlant comp;

		private static readonly Texture2D FullNPBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.2f));

		private static readonly Texture2D EmptyNPBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

		public Gizmo_MagiaPlantStatus()
		{
			Order = -100f;
		}

		public override float GetWidth(float maxWidth)
		{
			return 140f;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			Rect rect3 = rect2;
			rect3.height = rect.height / 2f;
			Text.Font = GameFont.Tiny;
			Widgets.Label(rect3, "MagiaPlant.UI.MS".Translate());
			Rect rect4 = rect2;
			rect4.yMin = rect2.y + rect2.height / 2f;
			float fillPercent = comp.NowMS / Mathf.Max(1f, comp.MaxMS);
			Widgets.FillableBar(rect4, fillPercent, FullNPBarTex, EmptyNPBarTex, doBorder: false);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect4, comp.NowMS.ToString() + "/" + comp.MaxMS.ToString());
			Text.Anchor = TextAnchor.UpperLeft;
			return new GizmoResult(GizmoState.Clear);
		}

	}

	public class CompProperties_MagiaPlant : CompProperties
	{
		public CompProperties_MagiaPlant()
		{
			compClass = typeof(Comp_MagiaPlant);
		}
	}

	public class Comp_MagiaPlant : ThingComp
	{

		// 最大MS
		public int MaxMS = 1000;
		// 現在MS
		public int NowMS = 1000;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref MaxMS, "MaxMS");
			Scribe_Values.Look(ref NowMS, "NowMS");
		}

		private void GetMaxMS(CompAbilityUserMagic comp_magic)
		{
			if (comp_magic.IsMagicUser)
            {
				MaxMS = Math.Max(1000, 1000 + (comp_magic.MagicUserLevel * 50));
			} else
            {
				MaxMS = 1000;
			}
		}

		public override void CompTick()
		{
			Pawn comppawn = (Pawn)this.parent;
			CompAbilityUserMagic comp_magic = comppawn.GetComp<CompAbilityUserMagic>();
			GetMaxMS(comp_magic);
			bool lowMood = comppawn.needs.mood.CurLevel <= 0.20f;
			bool isRelax = comppawn.health.hediffSet.HasHediff(HediffDef.Named("MgP_RelaxMode"));
			if (comppawn.IsHashIntervalTick(20))
			{
				if (isRelax)
				{
					NowMS += 1;
					return;
				}
			}
			if (comp_magic.IsMagicUser)
			{
				if (comppawn.IsHashIntervalTick(10))
				{
					Need_Mana needmana = comppawn.needs.TryGetNeed<Need_Mana>();
					if (needmana != null)
					{
						if (needmana.CurLevel < needmana.MaxLevel)
						{
							needmana.CurLevel += 0.01f;
							if (lowMood)
							{
								needmana.CurLevel += 0.03f;
							}
							UseMS(2);
							if (needmana.CurLevel > needmana.MaxLevel)
                            {
								needmana.CurLevel = needmana.MaxLevel;
							}
						}
					}
					Hediff hediff = comppawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("TM_ArcaneWeakness"));
					if (hediff != null)
					{
						hediff.Severity -= 0.1f;
						if (lowMood)
						{
							hediff.Severity -= 0.3f;
						}
						UseMS(4);
					}
				}
				if (!isRelax)
				{
					foreach (PawnAbility allPower in comp_magic.AbilityData.AllPowers)
					{
						if (allPower.CooldownTicksLeft > 0)
						{
							if (lowMood)
							{
								allPower.CooldownTicksLeft -= 3;
							}
							else
							{
								allPower.CooldownTicksLeft -= 1;
							}
						}
					}
				}
			}
			if (comppawn.IsHashIntervalTick(60))
            {
				if (comppawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness) <= 0.3f)
                {
					NowMS++;
					return;
                }
				if (lowMood)
				{
					UseMS(1);
				} else
                {
					NowMS++;
					if (NowMS > MaxMS)
					{
						NowMS = MaxMS;
					}
				}
            }
		}

		public void UseMS(int use)
		{
			if (use <= 0)
            {
				return;
            }
			Pawn comppawn = (Pawn)this.parent;
			NowMS -= use;
			if (comppawn.Dead)
            {
				return;
            }
			if (NowMS <= 0)
            {
				if (comppawn.MapHeld != null)
                {
					PawnKindDef kind = PawnKindDef.Named("MgP_ForestOfDisaster");
					Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, fixedGender: Gender.Female, fixedBiologicalAge: 0f, fixedChronologicalAge: 0f, faction: Faction.OfInsects));
					GenSpawn.Spawn(pawn, comppawn.PositionHeld, comppawn.MapHeld);
					MoteMaker.MakeStaticMote(this.parent.Position, this.parent.Map, ThingDef.Named("MgP_Mote_Tentacle"), 10.0f);
					SoundDef.Named("MgP_Trans").PlayOneShot(pawn);
					Messages.Message("MagiaPlant.UI.Trans".Translate(comppawn), pawn, MessageTypeDefOf.NegativeEvent, false);
					List<Pawn> pawns = new List<Pawn>();
					pawns.Add(pawn);
					LordMaker.MakeNewLord(Faction.OfInsects, new LordJob_DefendPoint(pawn.Position), pawn.Map, pawns);

					List<Thing> things = pawn.Map.spawnedThings.Where(x => x.def.defName == "MgP_EmotionGenerator").ToList();
					foreach (Thing thing in things)
                    {
						thing.TryGetComp<Comp_Generator>().addFuel();
                    }
				}
				comppawn.Kill(null);
				if (!comppawn.Destroyed)
                {
					comppawn.Destroy();
                }
				if (!comppawn.Corpse.Destroyed)
                {
					comppawn.Corpse.Destroy();
                }
			}
			
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			Pawn comppawn = (Pawn)this.parent;
			if (Find.Selector.SelectedPawns.Contains(comppawn))
			{
				Gizmo_MagiaPlantStatus gizmo_FPStatus = new Gizmo_MagiaPlantStatus();
				gizmo_FPStatus.comp = this;
				yield return gizmo_FPStatus;
				if (comppawn.Faction?.IsPlayer ?? false)
				{
					bool isrelax = comppawn.health.hediffSet.HasHediff(HediffDef.Named("MgP_RelaxMode"));
					Gizmo Gizmo = new Command_Toggle
					{
						defaultLabel = "MagiaPlant.UI.label_ContinuousHealing".Translate(),
						icon = Icon_MagiaPlant.ContinuousHealing_Icon,
						defaultDesc = "MagiaPlant.UI.desc_ContinuousHealing".Translate(),
						disabled = isrelax,
						isActive = delegate
						{
							return comppawn.health.hediffSet.HasHediff(HediffDef.Named("MgP_ContinuousHealing"));
						},
						toggleAction = delegate
						{
							if (!comppawn.health.hediffSet.HasHediff(HediffDef.Named("MgP_ContinuousHealing")))
							{
								comppawn.health.AddHediff(HediffDef.Named("MgP_ContinuousHealing"));
							}
							else
							{
								comppawn.health.RemoveHediff(comppawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MgP_ContinuousHealing")));
							}
						}
					};
					yield return Gizmo;
					Gizmo Gizmo_2 = new Command_Toggle
					{
						defaultLabel = "MagiaPlant.UI.label_ShadowEmotions".Translate(),
						icon = Icon_MagiaPlant.ShadowEmotions_Icon,
						defaultDesc = "MagiaPlant.UI.desc_ShadowEmotions".Translate(),
						disabled = isrelax,
						isActive = delegate
						{
							return comppawn.health.hediffSet.HasHediff(HediffDef.Named("MgP_ShadowEmotions"));
						},
						toggleAction = delegate
						{
							if (!comppawn.health.hediffSet.HasHediff(HediffDef.Named("MgP_ShadowEmotions")))
							{
								comppawn.health.AddHediff(HediffDef.Named("MgP_ShadowEmotions"));
							}
							else
							{
								comppawn.health.RemoveHediff(comppawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MgP_ShadowEmotions")));
							}
						}
					};
					yield return Gizmo_2;
					if (ResearchProjectDef.Named("MgP_Charge").IsFinished)
                    {
						Gizmo Gizmo_3 = new Command_Toggle
						{
							defaultLabel = "MagiaPlant.UI.label_RelaxMode".Translate(),
							icon = Icon_MagiaPlant.RelaxMode_Icon,
							defaultDesc = "MagiaPlant.UI.desc_RelaxMode".Translate(),
							isActive = delegate
							{
								return isrelax;
							},
							toggleAction = delegate
							{
								if (!comppawn.health.hediffSet.HasHediff(HediffDef.Named("MgP_RelaxMode")))
								{
									if (comppawn.health.hediffSet.HasHediff(HediffDef.Named("MgP_ShadowEmotions")))
									{
										comppawn.health.RemoveHediff(comppawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MgP_ShadowEmotions")));
									}
									if (comppawn.health.hediffSet.HasHediff(HediffDef.Named("MgP_ContinuousHealing")))
									{
										comppawn.health.RemoveHediff(comppawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MgP_ContinuousHealing")));
									}
									comppawn.health.AddHediff(HediffDef.Named("MgP_RelaxMode"));
								}
								else
								{
									comppawn.health.RemoveHediff(comppawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MgP_RelaxMode")));
								}
							}
						};
						yield return Gizmo_3;
					}
				}
			}
		}
	}

}
