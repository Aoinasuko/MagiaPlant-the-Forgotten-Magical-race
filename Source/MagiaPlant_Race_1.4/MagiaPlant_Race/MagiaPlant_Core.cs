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

    [DefOf]
    public static class Faction_MagiaPlant
    {
        public static FactionDef MgP_MagiaPlantGroup;
    }

    [DefOf]
    public static class FleshType_MagiaPlant
    {
        public static FleshTypeDef MagiaPlant;
    }

    [StaticConstructorOnStartup]
    public static class Icon_MagiaPlant
    {
        public static readonly Texture2D ContinuousHealing_Icon = ContentFinder<Texture2D>.Get("MagiaPlant/UI/Skill/ContinuousHealing");
        public static readonly Texture2D ShadowEmotions_Icon = ContentFinder<Texture2D>.Get("MagiaPlant/UI/Skill/ShadowEmotions");
        public static readonly Texture2D RelaxMode_Icon = ContentFinder<Texture2D>.Get("MagiaPlant/UI/Skill/RelaxMode");
    }

}
