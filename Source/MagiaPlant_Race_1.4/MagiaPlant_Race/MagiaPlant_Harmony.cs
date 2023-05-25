using AlienRace;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MagiaPlant_Race
{

    [StaticConstructorOnStartup]
    static class MagiaPlant_Harmony
    {
        static MagiaPlant_Harmony()
        {
            var harmony = new Harmony("BEP.MagiaPlant");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

}
