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
        public bool BackAndForth = true; // utilize Mathf.PingPong()?

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void Tick()
        {
            base.Tick();
            Building_Decoration_Reversible_ModExtension buildingExt = def.GetModExtension<Building_Decoration_Reversible_ModExtension>();

            float startMarker = reversedGraphic.data.drawOffset.x; // starting x value

            if ((HitPoints < MaxHitPoints) && (buildingExt.reversedGraphicData != null))
            {
                if (BackAndForth == true)
                {
                    // oscillatingValue = [amplitude * Sin(rad * current tick / speed)] - [middle of sin wave]
                    reversedGraphic.data.drawOffset = new Vector3
                        (0.50f * Mathf.Sin(Mathf.PI * Find.TickManager.TicksAbs / 120f) - startMarker, // x
                        reversedGraphic.data.drawOffset.y, // y
                        reversedGraphic.data.drawOffset.z); // z
                }
            }
        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostApplyDamage(dinfo, totalDamageDealt);
            DirtyMapMesh(Map);
        }

        private void DetermineReversedGraphicColor()
        {
            Building_Decoration_Reversible_ModExtension buildingExt = def.GetModExtension<Building_Decoration_Reversible_ModExtension>();
            if (buildingExt == null)
                return;
            ReversedGraphicCache = buildingExt.reversedGraphicData?.GraphicColoredFor(this);
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
                Building_Decoration_Reversible_ModExtension buildingExt = def.GetModExtension<Building_Decoration_Reversible_ModExtension>();
                if ((HitPoints < MaxHitPoints) && (buildingExt.reversedGraphicData.texPath != null))
                {
                    return ReversedGraphic;
                }
                return DefaultGraphic;

            }
        }
    }
}
