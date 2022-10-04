using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using HarmonyLib;

namespace Reversible_Decorations
{
    public static class Reversible_DecorationsMain
    {
        static Reversible_DecorationsMain()
        {
            Log.Message("<color=#4494E3FF>Thanks for using the decorations!</color>");

            Harmony harmony = new ("com.reversible_decorations");
            harmony.PatchAll();
        }
    }
}
