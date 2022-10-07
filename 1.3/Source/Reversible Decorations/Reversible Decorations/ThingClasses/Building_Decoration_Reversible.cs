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
        public bool BackAndForth = true;
        public int InterpolationPeriod = 2500 / Rand.Int;
        public int Period = 0;
        private readonly float MaxAngle = 360.0f;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void Tick()
        {
            base.Tick();
            float startMarker = DefaultGraphic.data.drawOffset.x; // starting x value

            if ((HitPoints < MaxHitPoints) && (BuildingExt.reversedGraphicData != null))
            {
                if (BackAndForth == true) // move between two points
                {
                    Period += Find.TickManager.TicksAbs;
                    if (Period <= InterpolationPeriod)
                    {
                        Period -= InterpolationPeriod;

                        // oscillatingValue = [amplitude * Sin(rad * current tick / speed)] - [middle of sin wave]
                        reversedGraphic.data.drawOffset = new Vector3
                            (0.50f * Mathf.Sin(Mathf.PI * Find.TickManager.TicksAbs / 120f) - startMarker, // x
                            reversedGraphic.data.drawOffset.y, // y
                            reversedGraphic.data.drawOffset.z); // z

                        float extraRotation;
                        extraRotation = (Find.TickManager.TicksAbs * 0.5f) % (MaxAngle);
                        reversedGraphic.DrawWorker(reversedGraphic.data.drawOffset, Rotation, def, this, extraRotation);
                    }
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
