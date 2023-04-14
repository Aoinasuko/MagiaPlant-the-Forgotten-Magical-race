using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MagiaPlant_Race
{
    public class CompProperties_UseMS : CompProperties
    {
        public int amount = 10;

        public CompProperties_UseMS()
        {
            compClass = typeof(Comp_UseMS);
        }
    }

    public class Comp_UseMS : ThingComp
    {
        public CompProperties_UseMS Props => (CompProperties_UseMS)props;

        public override void Notify_UsedWeapon(Pawn pawn)
        {
            Comp_MagiaPlant magia = pawn.TryGetComp<Comp_MagiaPlant>();
            if (magia != null)
            {
                magia.UseMS(Props.amount);
            }
        }
    }
}
