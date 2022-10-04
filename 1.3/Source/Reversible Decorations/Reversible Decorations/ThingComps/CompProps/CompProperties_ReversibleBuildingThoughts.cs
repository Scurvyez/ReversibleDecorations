using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using UnityEngine;

namespace Reversible_Decorations
{
    public class CompProperties_ReversibleBuildingThoughts : CompProperties
    {
        public string defaultThoughtDef = null;
        public string reversedThoughtDef = null;
        public SoundDef defaultSoundDef = null;
        public SoundDef reversedSoundDef = null;
        public int? tickInterval = 2500;

        public CompProperties_ReversibleBuildingThoughts() => compClass = typeof(Comp_ReversibleBuildingThoughts);

    }
}
