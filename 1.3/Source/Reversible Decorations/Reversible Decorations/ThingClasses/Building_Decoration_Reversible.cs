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
        public int InterpolationPeriod = 2400; // Length between counter start times
        public int Period = 0; // A counter.
        private float NewXCoord = 0; // Initial value we don't care cause it get's updated in Tick()
        private float ExtraRotation = 0;
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void Tick()
        {
            base.Tick();
            Color color1 = new(0.145f, 0.588f, 0.745f, 1f); // for debugging text
            // Do all the actual calculations here dumbass...
            // and don't forget to store the results!

            float startMarker = DefaultGraphic.data.drawOffset.x; // starting x value
            int absTick = Find.TickManager.TicksAbs; // the absolute tick, since startup
            int curTick = GenLocalDate.DayTick(Map); // actual current tick of the current day
            int thisHash = GetHashCode();

            if ((HitPoints < MaxHitPoints) && (BuildingExt.reversedGraphicData != null))
            {
                if (def != null)
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

            // LEGACY STUFF
            //int curHour = GenLocalDate.HourOfDay(Map); // an int, 1 - 24
            //bool onTheHour = curTick % GenDate.TicksPerHour == 0; // is the curTick the start of a new hour in-game?
        }

        public override void Draw()
        {
            // Do the actual fucking drawing here Steve...

            if (BuildingExt.reversedGraphicData != null && def != null)
            {
                if (Graphic == ReversedGraphic)
                {
                    // for x movement
                    reversedGraphic.data.drawOffset = new Vector3(
                        NewXCoord,                                   // x
                        reversedGraphic.data.drawOffset.y,           // y
                        reversedGraphic.data.drawOffset.z);          // z

                    Graphic.DrawWorker(DrawPos, Rotation, def, this, ExtraRotation);
                }
                else 
                {
                    DefaultGraphic.DrawWorker(DrawPos, Rotation, def, this, 0f);
                }
            }
            else 
            {
                DefaultGraphic.DrawWorker(DrawPos, Rotation, def, this, 0f);
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

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Period, "Period", 0);
            Scribe_Values.Look(ref NewXCoord, "NewXCoord", 0);
            Scribe_Values.Look(ref ExtraRotation, "ExtraRotation", 0);
        }
    }
}
