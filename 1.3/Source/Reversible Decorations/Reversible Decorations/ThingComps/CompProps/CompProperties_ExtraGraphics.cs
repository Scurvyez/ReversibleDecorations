using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;

namespace Reversible_Decorations
{
    public class CompProperties_ExtraGraphics : CompProperties
    {
        public List<GraphicData> extraGraphics = null;

        public CompProperties_ExtraGraphics()
        {
            compClass = typeof(Comp_ExtraGraphics);
        }
    }
}
