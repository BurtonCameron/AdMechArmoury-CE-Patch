using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CombatExtended
{
    // Token: 0x02000022 RID: 34
    public class Projectile_LaserConfig2
    {
        // Token: 0x0400009C RID: 156
        public Vector3 offset;
    }
    // Token: 0x02000023 RID: 35
    public class Projectile_Laser2 : ProjectileCE
    {
        // Token: 0x060000F0 RID: 240 RVA: 0x00008BAC File Offset: 0x00006DAC
        protected virtual void Explode(Thing hitThing, bool destroy = false)
        {
            Map map = base.Map;
            IntVec3 intVec = (hitThing != null) ? hitThing.PositionHeld : IntVec3Utility.ToIntVec3(this.destinationInt);
            if (destroy)
            {
                this.Destroy(0);
            }
            bool flag = this.def.projectile.explosionEffect != null;
            if (flag)
            {
                Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(intVec, map, false), new TargetInfo(intVec, map, false));
                effecter.Cleanup();
            }
            IntVec3 intVec2 = intVec;
            Map map2 = map;
            float explosionRadius = this.def.projectile.explosionRadius;
            DamageDef damageDef = this.def.projectile.damageDef;
            Thing launcher = this.launcher;
            int damageAmount = this.def.projectile.GetDamageAmount(1f, null);
            SoundDef soundExplode = this.def.projectile.soundExplode;
            ThingDef equipmentDef = this.equipmentDef;
            ThingDef def = this.def;
            ThingDef postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(intVec2, map2, explosionRadius, damageDef, launcher, damageAmount, 0f, soundExplode, equipmentDef, def, null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff);
        }

        // Token: 0x060000F1 RID: 241 RVA: 0x00008D5F File Offset: 0x00006F5F
        public override void SpawnSetup(Map map, bool blabla)
        {
            base.SpawnSetup(map, blabla);
            this.drawingTexture = this.def.DrawMatSingle;
        }

        // Token: 0x060000F2 RID: 242 RVA: 0x00008D7C File Offset: 0x00006F7C
        public void GetParametersFromXml()
        {
            ThingDef_LaserProjectile thingDef_LaserProjectile = this.def as ThingDef_LaserProjectile;
            this.preFiringDuration = thingDef_LaserProjectile.preFiringDuration;
            this.postFiringDuration = thingDef_LaserProjectile.postFiringDuration;
            this.preFiringInitialIntensity = thingDef_LaserProjectile.preFiringInitialIntensity;
            this.preFiringFinalIntensity = thingDef_LaserProjectile.preFiringFinalIntensity;
            this.postFiringInitialIntensity = thingDef_LaserProjectile.postFiringInitialIntensity;
            this.postFiringFinalIntensity = thingDef_LaserProjectile.postFiringFinalIntensity;
            this.startFireChance = thingDef_LaserProjectile.StartFireChance;
            this.canStartFire = thingDef_LaserProjectile.CanStartFire;
        }

        // Token: 0x060000F3 RID: 243 RVA: 0x00008DF8 File Offset: 0x00006FF8
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.tickCounter, "tickCounter", 0, false);
            bool flag = Scribe.mode == (LoadSaveMode)4;
            if (flag)
            {
                this.GetParametersFromXml();
            }
        }

        // Token: 0x060000F4 RID: 244 RVA: 0x00008E38 File Offset: 0x00007038
        public override void Tick()
        {
            try
            {
                bool flag = this.tickCounter == 0;
                if (flag)
                {
                    this.GetParametersFromXml();
                    this.PerformPreFiringTreatment();
                }
                bool flag2 = this.tickCounter < this.preFiringDuration;
                if (flag2)
                {
                    this.GetPreFiringDrawingParameters();
                }
                else
                {
                    bool flag3 = this.tickCounter == this.preFiringDuration;
                    if (flag3)
                    {
                        this.Fire();
                        this.GetPostFiringDrawingParameters();
                    }
                    else
                    {
                        this.GetPostFiringDrawingParameters();
                    }
                }
                bool flag4 = this.tickCounter == this.preFiringDuration + this.postFiringDuration && !base.Destroyed;
                if (flag4)
                {
                    this.Destroy(0);
                }
                bool flag5 = this.launcher != null;
                if (flag5)
                {
                    bool flag6 = this.launcher is Pawn;
                    if (flag6)
                    {
                        Pawn pawn = this.launcher as Pawn;
                        bool flag7 = pawn.Dead && !base.Destroyed;
                        if (flag7)
                        {
                            this.Destroy(0);
                        }
                    }
                }
                this.tickCounter++;
            }
            catch
            {
                this.Destroy(0);
            }
        }

        // Token: 0x060000F5 RID: 245 RVA: 0x00008F70 File Offset: 0x00007170
        public virtual void PerformPreFiringTreatment()
        {
            this.DetermineImpactExactPosition();
            Vector3 a = (this.destinationInt - this.OriginIV3.ToVector3()).normalized * 0.9f;
            bool flag = GenList.NullOrEmpty<Projectile_LaserConfig>(this.Def.graphicSettings);
            if (flag)
            {
                Vector3 s = new Vector3(1f, 1f, (this.Destination - this.origin).magnitude - a.magnitude);
                Vector3 pos = this.OriginIV3.ToVector3() + a / 2f + (this.destinationInt - this.OriginIV3.ToVector3()) / 2f + Vector3.up * this.def.Altitude;
                this.drawingMatrix = new List<Matrix4x4>();
                Matrix4x4 item = default(Matrix4x4);
                item.SetTRS(pos, this.ExactRotation, s);
                this.drawingMatrix.Add(item);
            }
            else
            {
                this.drawingMatrix = new List<Matrix4x4>();
                bool flag2 = !this.Def.cycleThroughFiringPositions;
                if (flag2)
                {
                    foreach (Projectile_LaserConfig setting in this.Def.graphicSettings)
                    {
                        this.AddLaserGraphicUsing(setting);
                    }
                }
            }
        }

        // Token: 0x060000F6 RID: 246 RVA: 0x00009180 File Offset: 0x00007380
        private void AddLaserGraphicUsing(Projectile_LaserConfig setting)
        {
            Vector3 a = (this.Destination - this.origin).normalized * 0.9f;
            Vector3 s = new Vector3(1f, 1f, (this.Destination - this.origin).magnitude - a.magnitude);
            Vector3 vector = this.OriginIV3.ToVector3() + a / 2f + (this.destinationInt - this.OriginIV3.ToVector3()) / 2f + Vector3.up * this.def.Altitude;
            float num = 0f;
            bool flag = GenGeo.MagnitudeHorizontalSquared(this.destinationInt - this.OriginIV3.ToVector3()) > 0.001f;
            if (flag)
            {
                num = Vector3Utility.AngleFlat(this.Destination - this.origin);
            }
            vector += Vector3Utility.RotatedBy(setting.offset, num);
            Matrix4x4 item = default(Matrix4x4);
            item.SetTRS(vector, this.ExactRotation, s);
            this.drawingMatrix.Add(item);
        }

        // Token: 0x17000025 RID: 37
        // (get) Token: 0x060000F7 RID: 247 RVA: 0x000092B2 File Offset: 0x000074B2
        public ThingDef_LaserProjectile Def
        {
            get
            {
                return this.def as ThingDef_LaserProjectile;
            }
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x000092C0 File Offset: 0x000074C0
        public virtual void GetPreFiringDrawingParameters()
        {
            bool flag = this.preFiringDuration != 0;
            if (flag)
            {
                this.drawingIntensity = this.preFiringInitialIntensity + (this.preFiringFinalIntensity - this.preFiringInitialIntensity) * (float)this.tickCounter / (float)this.preFiringDuration;
            }
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x00009308 File Offset: 0x00007508
        public virtual void GetPostFiringDrawingParameters()
        {
            bool flag = this.postFiringDuration != 0;
            if (flag)
            {
                this.drawingIntensity = this.postFiringInitialIntensity + (this.postFiringFinalIntensity - this.postFiringInitialIntensity) * (((float)this.tickCounter - (float)this.preFiringDuration) / (float)this.postFiringDuration);
            }
        }

        // Token: 0x060000FA RID: 250 RVA: 0x00009358 File Offset: 0x00007558
        protected void DetermineImpactExactPosition()
        {
            Vector3 a = this.Destination - this.origin;
            int num = (int)a.magnitude;
            Vector3 b = a / a.magnitude;
            Vector3 destination = this.origin;
            Vector3 vector = this.origin;
            IntVec3 intVec = IntVec3Utility.ToIntVec3(vector);
            for (int i = 1; i <= num; i++)
            {
                vector += b;
                intVec = IntVec3Utility.ToIntVec3(vector);
                bool flag = !GenGrid.InBounds(vector, base.Map);
                if (flag)
                {
                    this.destinationInt = destination;
                    break;
                }
                bool flag2 = !this.def.projectile.flyOverhead && i >= 5;
                if (flag2)
                {
                    List<Thing> list = base.Map.thingGrid.ThingsListAt(base.Position);
                    for (int j = 0; j < list.Count; j++)
                    {
                        Thing thing = list[j];
                        bool flag3 = thing.def.Fillage == (FillCategory)2;
                        if (flag3)
                        {
                            this.destinationInt = intVec.ToVector3Shifted() + new Vector3(Rand.Range(-0.3f, 0.3f), 0f, Rand.Range(-0.3f, 0.3f));
                            this.hitThing = thing;
                            break;
                        }
                        bool flag4 = thing.def.category == (ThingCategory)1;
                        if (flag4)
                        {
                            Pawn pawn = thing as Pawn;
                            float num2 = 0.45f;
                            bool downed = pawn.Downed;
                            if (downed)
                            {
                                num2 *= 0.1f;
                            }
                            float num3 = GenGeo.MagnitudeHorizontal(this.ExactPosition - this.OriginIV3.ToVector3());
                            bool flag5 = num3 < 4f;
                            if (flag5)
                            {
                                num2 *= 0f;
                            }
                            else
                            {
                                bool flag6 = num3 < 7f;
                                if (flag6)
                                {
                                    num2 *= 0.5f;
                                }
                                else
                                {
                                    bool flag7 = num3 < 10f;
                                    if (flag7)
                                    {
                                        num2 *= 0.75f;
                                    }
                                }
                            }
                            num2 *= pawn.RaceProps.baseBodySize;
                            bool flag8 = Rand.Value < num2;
                            if (flag8)
                            {
                                this.destinationInt = intVec.ToVector3Shifted() + new Vector3(Rand.Range(-0.3f, 0.3f), 0f, Rand.Range(-0.3f, 0.3f));
                                this.hitThing = pawn;
                                break;
                            }
                        }
                    }
                }
                destination = vector;
            }
        }

        // Token: 0x060000FB RID: 251 RVA: 0x000095E6 File Offset: 0x000077E6
        public virtual void Fire()
        {
            this.ApplyDamage(this.hitThing);
        }

        // Token: 0x060000FC RID: 252 RVA: 0x000095F8 File Offset: 0x000077F8
        protected void ApplyDamage(Thing hitThing)
        {
            bool flag = hitThing != null;
            if (flag)
            {
                this.Impact(hitThing);
            }
            else
            {
                this.ImpactSomething();
            }
        }

        // Token: 0x060000FD RID: 253 RVA: 0x00009624 File Offset: 0x00007824
        protected void ImpactSomething()
        {
            bool flyOverhead = this.def.projectile.flyOverhead;
            if (flyOverhead)
            {
                RoofDef roofDef = base.Map.roofGrid.RoofAt(base.destinationInt.ToIntVec3());
                bool flag = roofDef != null && roofDef.isThickRoof;
                if (flag)
                {
                    SoundInfo soundInfo = SoundInfo.InMap(new TargetInfo(base.destinationInt.ToIntVec3(), base.Map, false), 0);
                    SoundStarter.PlayOneShot(this.def.projectile.soundHitThickRoof, soundInfo);
                    return;
                }
            }
            bool flag2 = this.intendedTarget != null;
            if (flag2)
            {
                Pawn pawn = this.intendedTarget as Pawn;
                bool flag3 = pawn != null && pawn.Downed && (this.origin - this.Destination).magnitude > 5f && Rand.Value < 0.2f;
                if (flag3)
                {
                    this.Impact(null);
                }
                else
                {
                    this.Impact(this.intendedTarget);
                }
            }
            else
            {
                Thing thing = base.Map.thingGrid.ThingAt(base.destinationInt.ToIntVec3(), (ThingCategory)1);
                bool flag4 = thing != null;
                if (flag4)
                {
                    this.Impact(thing);
                }
                else
                {
                    foreach (Thing thing2 in base.Map.thingGrid.ThingsAt(base.destinationInt.ToIntVec3()))
                    {
                        bool flag5 = thing2.def.fillPercent > 0f || thing2.def.passability > 0;
                        if (flag5)
                        {
                            this.Impact(thing2);
                            return;
                        }
                    }
                    this.Impact(null);
                }
            }
        }
        
        /*
        // Token: 0x060000FE RID: 254 RVA: 0x00009800 File Offset: 0x00007A00
        protected override void Impact(Thing hitThing)
        {
            bool createsExplosion = this.Def.createsExplosion;
            if (createsExplosion)
            {
                this.Explode(hitThing, false);
                GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
            }
            bool flag = hitThing != null;
            if (flag)
            {
                Map map = base.Map;
                BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget, this.equipmentDef, this.def, this.targetCoverDef);
                Find.BattleLog.Add(battleLogEntry_RangedImpact);
                int damageAmount = this.def.projectile.GetDamageAmount(1f, null);
                DamageDef damageDef = this.def.projectile.damageDef;
                int num = damageAmount;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo damageInfo;
                damageInfo..ctor(damageDef, (float)num, this.def.projectile.GetArmorPenetration(1f, null), y, launcher, null, equipmentDef, 0, null);
                hitThing.TakeDamage(damageInfo).AssociateWithLog(battleLogEntry_RangedImpact);
                bool flag2 = this.canStartFire && Rand.Range(0f, 1f) > this.startFireChance;
                if (flag2)
                {
                    FireUtility.TryAttachFire(hitThing, 0.05f);
                }
                Pawn pawn = hitThing as Pawn;
                bool flag3 = pawn != null;
                if (flag3)
                {
                    this.PostImpactEffects(this.launcher as Pawn, pawn);
                    MoteMaker.ThrowMicroSparks(this.destinationInt, base.Map);
                    MoteMaker.MakeStaticMote(this.destinationInt, base.Map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                }
            }
            else
            {
                SoundInfo soundInfo = SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), 0);
                SoundStarter.PlayOneShot(SoundDefOf.BulletImpact_Ground, soundInfo);
                MoteMaker.MakeStaticMote(this.ExactPosition, base.Map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                MoteMaker.ThrowMicroSparks(this.ExactPosition, base.Map);
            }
        }
        */
        // Token: 0x060000FF RID: 255 RVA: 0x000035BB File Offset: 0x000017BB
        public virtual void PostImpactEffects(Pawn launcher, Pawn hitTarget)
        {
        }

        // Token: 0x06000100 RID: 256 RVA: 0x00009A08 File Offset: 0x00007C08
        public override void Draw()
        {
            base.Comps_PostDraw();
            bool flag = !GenList.NullOrEmpty<Matrix4x4>(this.drawingMatrix);
            if (flag)
            {
                foreach (Matrix4x4 matrix in this.drawingMatrix)
                {
                    Graphics.DrawMesh(MeshPool.plane10, matrix, FadedMaterialPool.FadedVersionOf(this.drawingTexture, this.drawingIntensity), 0);
                }
            }
        }

        // Token: 0x0400009D RID: 157
        public int tickCounter = 0;

        // Token: 0x0400009E RID: 158
        public Thing hitThing = null;

        // Token: 0x0400009F RID: 159
        public float preFiringInitialIntensity = 0f;

        // Token: 0x040000A0 RID: 160
        public float preFiringFinalIntensity = 0f;

        // Token: 0x040000A1 RID: 161
        public float postFiringInitialIntensity = 0f;

        // Token: 0x040000A2 RID: 162
        public float postFiringFinalIntensity = 0f;

        // Token: 0x040000A3 RID: 163
        public int preFiringDuration = 0;

        // Token: 0x040000A4 RID: 164
        public int postFiringDuration = 0;

        // Token: 0x040000A5 RID: 165
        public float startFireChance = 0f;

        // Token: 0x040000A6 RID: 166
        public bool canStartFire = false;

        // Token: 0x040000A7 RID: 167
        public Material preFiringTexture;

        // Token: 0x040000A8 RID: 168
        public Material postFiringTexture;

        // Token: 0x040000A9 RID: 169
        public List<Matrix4x4> drawingMatrix = null;

        // Token: 0x040000AA RID: 170
        public float drawingIntensity = 0f;

        // Token: 0x040000AB RID: 171
        public Material drawingTexture;

        // Token: 0x040000AC RID: 172
        private int ticksToDetonation;
    }
}
