using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Vanya_Test
{
    // Token: 0x02000003 RID: 3
    [StaticConstructorOnStartup]
    public class Vanya_ShieldBelt : Apparel
    {
        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000005 RID: 5 RVA: 0x00002114 File Offset: 0x00000314
        private float EnergyMax
        {
            get
            {
                return this.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true);
            }
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000006 RID: 6 RVA: 0x00002134 File Offset: 0x00000334
        private float EnergyGainPerTick
        {
            get
            {
                return this.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true) / 60f;
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000007 RID: 7 RVA: 0x00002158 File Offset: 0x00000358
        public float Energy
        {
            get
            {
                return this.energy;
            }
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000008 RID: 8 RVA: 0x00002170 File Offset: 0x00000370
        public ShieldState ShieldState
        {
            get
            {
                bool flag = this.ticksToReset > 0;
                ShieldState result;
                if (flag)
                {
                    result = ShieldState.Resetting;
                }
                else
                {
                    result = ShieldState.Active;
                }
                return result;
            }
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000009 RID: 9 RVA: 0x00002198 File Offset: 0x00000398
        private bool ShouldDisplay
        {
            get
            {
                Pawn wearer = base.Wearer;
                return !wearer.Dead && !wearer.Downed && (!wearer.IsPrisonerOfColony || (wearer.MentalStateDef != null && wearer.MentalStateDef.IsAggro)) && (wearer.Drafted || wearer.Faction.HostileTo(Faction.OfPlayer) || Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks);
            }
        }

        // Token: 0x0600000A RID: 10 RVA: 0x00002217 File Offset: 0x00000417
        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            yield return new Gizmo_Vanya_EnergyShieldStatus
            {
                shield = this
            };
            yield break;
        }

        // Token: 0x0600000B RID: 11 RVA: 0x00002228 File Offset: 0x00000428
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.energy, "energy", 0f, false);
            Scribe_Values.Look<int>(ref this.ticksToReset, "ticksToReset", -1, false);
            Scribe_Values.Look<int>(ref this.lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
        }

        // Token: 0x0600000C RID: 12 RVA: 0x0000227C File Offset: 0x0000047C
        public override float GetSpecialApparelScoreOffset()
        {
            return this.EnergyMax * this.ApparelScorePerEnergyMax;
        }

        // Token: 0x0600000D RID: 13 RVA: 0x0000229C File Offset: 0x0000049C
        public override void Tick()
        {
            base.Tick();
            bool flag = base.Wearer == null;
            if (flag)
            {
                this.energy = 0f;
            }
            else
            {
                bool flag2 = this.ShieldState == ShieldState.Resetting;
                if (flag2)
                {
                    this.ticksToReset--;
                    bool flag3 = this.ticksToReset <= 0;
                    if (flag3)
                    {
                        this.Reset();
                    }
                }
                else
                {
                    bool flag4 = this.ShieldState == ShieldState.Active;
                    if (flag4)
                    {
                        this.energy += this.EnergyGainPerTick;
                        bool flag5 = this.energy > this.EnergyMax;
                        if (flag5)
                        {
                            this.energy = this.EnergyMax;
                        }
                    }
                }
            }
        }

        // Token: 0x0600000E RID: 14 RVA: 0x00002348 File Offset: 0x00000548
        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            bool flag = this.ShieldState == ShieldState.Active && (dinfo.Instigator != null || dinfo.Def.isExplosive);
            bool result;
            if (flag)
            {
                bool flag2 = dinfo.Instigator != null;
                if (flag2)
                {
                    AttachableThing attachableThing;
                    bool flag3 = (attachableThing = (dinfo.Instigator as AttachableThing)) != null && attachableThing.parent == base.Wearer;
                    if (flag3)
                    {
                        return false;
                    }
                }
                this.energy -= (float)dinfo.Amount * this.EnergyLossPerDamage;
                bool flag4 = this.energy < 0f;
                if (flag4)
                {
                    this.Break();
                }
                else
                {
                    this.AbsorbedDamage(dinfo);
                }
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600000F RID: 15 RVA: 0x0000240C File Offset: 0x0000060C
        public void KeepDisplaying()
        {
            this.lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        // Token: 0x06000010 RID: 16 RVA: 0x00002420 File Offset: 0x00000620
        private void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 vector = base.Wearer.TrueCenter() + Vector3Utility.RotatedBy(this.impactAngleVect, 180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + (float)dinfo.Amount / 10f);
            MoteMaker.MakeStaticMote(vector, base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                MoteMaker.ThrowDustPuff(vector, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
            this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
            this.KeepDisplaying();
        }

        // Token: 0x06000011 RID: 17 RVA: 0x00002520 File Offset: 0x00000720
        private void Break()
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
            MoteMaker.MakeStaticMote(base.Wearer.TrueCenter(), base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
            for (int i = 0; i < 6; i++)
            {
                Vector3 vector = base.Wearer.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                MoteMaker.ThrowDustPuff(vector, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
            this.energy = 0f;
            this.ticksToReset = this.StartingTicksToReset;
        }

        // Token: 0x06000012 RID: 18 RVA: 0x00002600 File Offset: 0x00000800
        private void Reset()
        {
            bool spawned = base.Wearer.Spawned;
            if (spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
                MoteMaker.ThrowLightningGlow(base.Wearer.TrueCenter(), base.Wearer.Map, 3f);
            }
            this.ticksToReset = -1;
            this.energy = this.EnergyOnReset;
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00002680 File Offset: 0x00000880
        public override void DrawWornExtras()
        {
            bool flag = this.ShieldState == ShieldState.Active && this.ShouldDisplay;
            if (flag)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, this.energy);
                Vector3 vector = base.Wearer.Drawer.DrawPos;
                vector.y = AltitudeLayer.Blueprint.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - this.lastAbsorbDamageTick;
                bool flag2 = num2 < 8;
                if (flag2)
                {
                    float num3 = (float)(8 - num2) / 8f * 0.05f;
                    vector += this.impactAngleVect * num3;
                    num -= num3;
                }
                float num4 = (float)Rand.Range(0, 360);
                Vector3 vector2 = new Vector3(num, 1f, num);
                Matrix4x4 matrix4x = default(Matrix4x4);
                matrix4x.SetTRS(vector, Quaternion.AngleAxis(num4, Vector3.up), vector2);
                Graphics.DrawMesh(MeshPool.plane10, matrix4x, Vanya_ShieldBelt.BubbleMat, 0);
            }
        }

        // Token: 0x06000014 RID: 20 RVA: 0x00002774 File Offset: 0x00000974
        public Vanya_ShieldBelt()
        {
        }

        // Token: 0x06000015 RID: 21 RVA: 0x000027DC File Offset: 0x000009DC
        // Note: this type is marked as 'beforefieldinit'.
        static Vanya_ShieldBelt()
        {
        }

        // Token: 0x04000004 RID: 4
        private const float MinDrawSize = 1.2f;

        // Token: 0x04000005 RID: 5
        private const float MaxDrawSize = 1.4f;

        // Token: 0x04000006 RID: 6
        private const float MaxDamagedJitterDist = 0.05f;

        // Token: 0x04000007 RID: 7
        private const int JitterDurationTicks = 8;

        // Token: 0x04000008 RID: 8
        private float energy;

        // Token: 0x04000009 RID: 9
        private int ticksToReset = -1;

        // Token: 0x0400000A RID: 10
        private int lastKeepDisplayTick = -9999;

        // Token: 0x0400000B RID: 11
        private Vector3 impactAngleVect;

        // Token: 0x0400000C RID: 12
        private int lastAbsorbDamageTick = -9999;

        // Token: 0x0400000D RID: 13
        private int StartingTicksToReset = 3000;

        // Token: 0x0400000E RID: 14
        private float EnergyOnReset = 0.2f;

        // Token: 0x0400000F RID: 15
        private float EnergyLossPerDamage = 0.025f;

        // Token: 0x04000010 RID: 16
        private int KeepDisplayingTicks = 1000;

        // Token: 0x04000011 RID: 17
        private float ApparelScorePerEnergyMax = 0.25f;

        // Token: 0x04000012 RID: 18
        private static readonly Material BubbleMat = MaterialPool.MatFrom("Vanya_Shield/Vanya_Shield", ShaderDatabase.Transparent);
    }
}
