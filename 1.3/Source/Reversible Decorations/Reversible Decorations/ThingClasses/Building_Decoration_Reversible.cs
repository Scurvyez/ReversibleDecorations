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
    [StaticConstructorOnStartup]
    public class Building_Decoration_Reversible : Building
    {
        private Graphic ReversedGraphicCache;
        public Graphic reversedGraphic => ReversedGraphicCache;
        public Building_Decoration_Reversible_ModExtension BuildingExt;
        private readonly float MaxAngle = 360.0f; // For rotation of graphic.
        public int InterpolationPeriod = 30; // Length of counter.
        public int Period = 0; // A counter.
        private float NewXCoord = 0; // Initial value we don't care cause it get's updated in Tick()
        private float ExtraRotation = 0; // Initial value we don't care cause it get's updated in Tick()



        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void Tick()
        {
            base.Tick();
            // Do all the actual calculations here dumbass...
            // and don't forget to store the results!

            float startMarker = DefaultGraphic.data.drawOffset.x; // starting x value
            int absTick = Find.TickManager.TicksAbs; // the absolute tick, since startup
            int curTick = GenLocalDate.DayTick(Map); // actual current tick of the current day
            int curHour = GenLocalDate.HourOfDay(Map); // an int, 1 - 24
            bool onTheHour = curTick % GenDate.TicksPerHour == 0; // is the curTick the start of a new hour in-game?

            Color color1 = new(0.145f, 0.588f, 0.745f, 1f); // for debugging text

            if ((HitPoints < MaxHitPoints) && (BuildingExt.reversedGraphicData != null))
            {
                if (def != null)
                {
                    Period += absTick;
                    if (Period <= InterpolationPeriod)
                    {
                        Period -= InterpolationPeriod;

                        // oscillatingValue = [amplitude * Sin(rad * current tick / speed)] - [middle of sin wave]
                        NewXCoord = (0.50f * Mathf.Sin(Mathf.PI * curTick / 120f) - startMarker);

                        // rotationVaule = [(current tick * spin speed) per (360 degrees)]
                        ExtraRotation = (absTick * 0.5f) % (MaxAngle);

                        // messages for debugging
                        //Log.Message("x = " + NewXCoord.ToString().Colorize(color1) + " , Time of day = " + curHour.ToString().Colorize(color1));
                    }
                }
            }
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            // Do the actual fucking drawing here Steve...
            // the shake n' bake
            // and the ole' basket weave

            Color color1 = new(0.145f, 0.588f, 0.745f, 1f); // for debugging text
            //Log.Message("drawLoc = " + drawLoc.ToString().Colorize(color1));
            //Log.Message("flip = " + flip.ToString().Colorize(color1));
            //Log.Message("def = " + def.ToString().Colorize(color1));
            //Log.Message("ExtraRotation = " + ExtraRotation.ToString().Colorize(color1));
            //Log.Message("reversedGraphic.data.drawOffset = " + reversedGraphic.data.drawOffset.ToString().Colorize(color1));
            //Log.Message("NewXCoord = " + NewXCoord.ToString().Colorize(color1));
            //Log.Message("drawLoc = " + drawLoc.ToString().Colorize(color1));
            //Log.Message("DefaultGraphic = " + DefaultGraphic.ToString().Colorize(color1));
            //Log.Message("reversedGraphic = " + reversedGraphic.ToString().Colorize(color1));

            if (BuildingExt.reversedGraphicData != null)
            {
                if (HitPoints < MaxHitPoints)
                {
                    if (def != null && NewXCoord != 0 && drawLoc != null)
                    {
                        // for x movement
                        reversedGraphic.data.drawOffset = new Vector3(
                            NewXCoord, // x
                            reversedGraphic.data.drawOffset.y, // y
                            reversedGraphic.data.drawOffset.z); // z

                        drawLoc = reversedGraphic.data.drawOffset;

                        // needed for rotation
                        Graphic.DrawWorker(drawLoc, Rotation, def, this, ExtraRotation);
                    }
                }
                else
                {
                    drawLoc = DrawPos;
                    Graphic.DrawWorker(drawLoc, Rotation, def, this, 0);
                }
            }
        }

        public override void PostMake()
        {
            base.PostMake();
            BuildingExt = def.GetModExtension<Building_Decoration_Reversible_ModExtension>();
        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostApplyDamage(dinfo, totalDamageDealt);
            DirtyMapMesh(Map);
        }

        private void DetermineReversedGraphicColor()
        {
            if (BuildingExt == null)
                return;
            ReversedGraphicCache = BuildingExt.reversedGraphicData?.GraphicColoredFor(this);
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

        /// <summary>
        /// Changes graphic if buildings' hitpoints are at 100% or not.
        /// </summary>
        public override Graphic Graphic
        {
            get
            {
                if ((HitPoints < MaxHitPoints) && (BuildingExt.reversedGraphicData.texPath != null))
                {
                    return ReversedGraphic;
                }
                return DefaultGraphic;

            }
        }
    }
}
