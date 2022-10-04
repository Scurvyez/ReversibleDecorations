using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;

namespace Reversible_Decorations
{
    public class Comp_ReversibleBuildingThoughts : ThingComp
    {
        public CompProperties_ReversibleBuildingThoughts Props
        {
            get
            {
                return (CompProperties_ReversibleBuildingThoughts)props;
            }
        }
        
        public void SoundsAndThoughts()
        {
            SoundDef defaultSound = Props.defaultSoundDef;
            SoundDef reversedSound = Props.reversedSoundDef;
            SoundInfo sI = new TargetInfo(parent.Position, parent.Map);

            if (parent != null && parent.Map != null)
            {
                //foreach (Thing t in GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, 2.0f, true))
                foreach (Thing t in parent.GetRoom().ContainedAndAdjacentThings)
                {
                    if (t is Pawn p && !p.Dead)
                    {
                        if (parent.Graphic == parent.DefaultGraphic)
                        {
                            p.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named(Props.reversedThoughtDef));
                            p.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named(Props.defaultThoughtDef), null);
                            if (Props.defaultSoundDef != null)
                            {
                                defaultSound.PlayOneShot(sI);
                            }
                        }
                        else
                        {
                            p.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named(Props.defaultThoughtDef));
                            p.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named(Props.reversedThoughtDef), null);
                            if (Props.reversedSoundDef != null)
                            {
                                reversedSound.PlayOneShot(sI);
                            }
                        }
                    }
                }
            }
        }
    }
}
