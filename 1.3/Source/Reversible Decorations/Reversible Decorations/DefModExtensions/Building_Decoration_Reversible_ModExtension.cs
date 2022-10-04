using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;

namespace Reversible_Decorations
{
    public class Building_Decoration_Reversible_ModExtension : DefModExtension
    {
        public GraphicData reversedGraphicData;
        public string defaultThoughtDef = null;
        public string reversedThoughtDef = null;

        public override IEnumerable<string> ConfigErrors()
        {
            if (reversedGraphicData == null)
            {
                yield return "[<color=#4494E3FF>Reversible Decorations</color>]" +
                    "Building_Decoration_Reversible_ModExtension must define a reversed graphic.";
            }
        }
    }
}
