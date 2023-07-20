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
    public class Comp_ExtraGraphics : ThingComp
    {
        public CompProperties_ExtraGraphics Props => (CompProperties_ExtraGraphics)props;
        public int Period = 0; // A counter.
        public int InterpolationPeriod = 2400; // Length between counter start times
        private float NewXCoord = 0; // Initial value we don't care cause it get's updated in Tick()
        private float ExtraRotation = 0;

        private Graphic ReversedGraphicCache;
        public Graphic reversedGraphic => ReversedGraphicCache;

        public override void CompTick()
        {
            base.CompTick();

            float startMarker = parent.DefaultGraphic.data.drawOffset.x; // starting x value
            int absTick = Find.TickManager.TicksAbs; // the absolute tick, since startup
            int curTick = GenLocalDate.DayTick(parent.Map); // actual current tick of the current day
            int thisHash = GetHashCode();

            if ((parent.HitPoints < parent.MaxHitPoints) && (Props.extraGraphics != null))
            {
                if (parent.def != null)
                {
                    Period += absTick;
                    if (Period <= InterpolationPeriod + thisHash)
                    {
                        Period -= InterpolationPeriod;

                        // oscillation calculation
                        // oscillatingValue = [amplitude * Sin(rad * current tick / speed)] - [middle of sin wave]
                        // lower speed = faster movement
                        NewXCoord = (0.08f * Mathf.Sin(Mathf.PI * curTick / 60f) - startMarker);

                        // rotation calculation
                        float angle = Mathf.LerpAngle(30, 330, curTick);
                        ExtraRotation = Mathf.PingPong(curTick, Mathf.Sin(curTick * angle)); // between two angles

                        // LEGACY STUFF
                        // rotationValue = [(current tick * spin speed) per (360 degrees)]
                        //ExtraRotation = (absTick * 3f) % (MaxAngle); // (CW) // full 360 degrees
                    }
                }
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            Color color1 = new Color(0.145f, 0.588f, 0.745f, 1f); // for debugging text

            if (Props.extraGraphics != null && parent.def != null)
            {
                if (parent.Graphic == ReversedGraphic)
                {
                    // for x movement
                    reversedGraphic.data.drawOffset = new Vector3(
                        NewXCoord,                                   // x
                        reversedGraphic.data.drawOffset.y,           // y
                        reversedGraphic.data.drawOffset.z);          // z

                    parent.Graphic.DrawWorker(parent.DrawPos, parent.Rotation, parent.def, parent, ExtraRotation);
                }
                else
                {
                    parent.DefaultGraphic.DrawWorker(parent.DrawPos, parent.Rotation, parent.def, parent, 0f);
                }
            }
            else
            {
                parent.DefaultGraphic.DrawWorker(parent.DrawPos, parent.Rotation, parent.def, parent, 0f);
            }
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            parent.DirtyMapMesh(parent.Map);
        }

        private void DetermineReversedGraphicColor()
        {
            if (Props == null)
                return;
            for (int i = 0; i < Props.extraGraphics.Count; i++)
            {
                ReversedGraphicCache = Props.extraGraphics.Count()?.GraphicColoredFor(this);
            }
        }

        private Graphic ReversedGraphic
        {
            get
            {
                if (Scribe.mode != LoadSaveMode.Inactive)
                {
                    return null;
                }
                if (ReversedGraphicCache == null)
                {
                    DetermineReversedGraphicColor();
                }
                return ReversedGraphicCache;
            }
        }

        public Graphic Graphic
        {
            get
            {
                if ((parent.HitPoints < parent.MaxHitPoints) && (Props.extraGraphics != null))
                {
                    return ReversedGraphic;
                }
                return parent.DefaultGraphic;

            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
        }
    }
}
