﻿using RimWorld;
using System.Linq;
using Verse;

namespace CombatExtended
{
    public class Verb_ShootCEOG : Verb_ShootCE
    {
        protected virtual float Reliable
        {
            get
            {
                return EquipmentSource.GetStatValue(StatDefOf_OG.reliability);
            }
        }

        public VerbPropertiesCEOG VerbPropsOG
        {
            get
            {
                return verbProps as VerbPropertiesCEOG;
            }
        }
        protected override int ShotsPerBurst
        {
            get
            {
                bool flag = base.CompFireModes != null;
                if (flag)
                {
                    bool flag2 = base.CompFireModes.CurrentFireMode == FireMode.SingleFire;
                    if (flag2)
                    {
                        if (VerbPropsOG.rapidfire == true && caster.Position.InHorDistOf(this.currentTarget.Cell, this.verbProps.range / 2))
                        {
                            return 2;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    bool flag3 = base.CompFireModes.CurrentFireMode == FireMode.BurstFire && base.CompFireModes.Props.aimedBurstShotCount > 0;
                    if (flag3)
                    {
                        if (VerbPropsOG.rapidfire == true && caster.Position.InHorDistOf(this.currentTarget.Cell, this.verbProps.range / 2))
                        {
                            return base.CompFireModes.Props.aimedBurstShotCount * 2;
                        }
                        else
                        {
                            return base.CompFireModes.Props.aimedBurstShotCount;
                        }
                    }
                }
                if (VerbPropsOG.rapidfire == true && caster.Position.InHorDistOf(this.currentTarget.Cell, this.verbProps.range / 2))
                {
                    return base.VerbPropsCE.burstShotCount * 2;
                }
                else
                {
                    return base.VerbPropsCE.burstShotCount;
                }

                //return this.verbProps.warmupTime;
                //return this.verbProps.defaultCooldownTime;
            }
        }
        protected override bool TryCastShot()
        {
            int logcount = 0;
            bool logging = VerbPropsOG.logging;
        //    logging = true;
        //    bool canDamageWeapon = VerbPropsOG.canDamageWeapon;
        //    float extraWeaponDamage = VerbPropsOG.extraWeaponDamage;
            bool canJam = VerbPropsOG.canJam;
            string msg;
            string lmsg = string.Format("log {0} Reliable:{1}", logcount, Reliable);
            string reliabilityString;
            float jamsOn;
            StatPart_Reliability.GetReliability((ThingDef_GunOG)EquipmentSource, out reliabilityString, out jamsOn);
                logcount++;
                if (logging == true) { Log.Message(lmsg); }
            jamsOn = jamsOn++;
            float jamRoll = 0;
                logcount++;
                lmsg = string.Format("log {0} jamsOn {1}", logcount, jamsOn);
                if (logging == true) { Log.Message(lmsg); }
            if (VerbPropsOG.overheat == true) { jamRoll = (Rand.Range(0, 100)); }
            else { jamRoll = (Rand.Range(0, 1000)) / 10f; }
                logcount++;
                lmsg = string.Format("log {0} jamRoll {1}", logcount, jamRoll);
                if (logging == true) { Log.Message(lmsg); }
            if (jamRoll < jamsOn && canJam == true)
            {
                logcount++;
                lmsg = string.Format("log {0} VerbPropsOG.overheat {1}", logcount, VerbPropsOG.overheat);
                if (logging == true) { Log.Message(lmsg); }
                if (VerbPropsOG.overheat == true)
                {
                    DamageDef damageDef = projectilePropsCE.damageDef;
                    if (damageDef!=null)
                    {
                        lmsg = string.Format("log {0} damageDef is null?:{1}", logcount, projectilePropsCE.damageDef.hediff);
                        if (logging == true) { Log.Message(lmsg); }
                    }
                    HediffDef HediffToAdd = damageDef.hediff;
                    float ArmorPenetration = projectilePropsCE.GetArmorPenetration(EquipmentSource, null);
                    float overheatsOn = VerbPropsOG.overheatsOn;
                    logcount++;
                    lmsg = string.Format("log {0} overheatsOn {1}", logcount, overheatsOn);
                    if (logging == true) { Log.Message(lmsg); }
                    int DamageAmount = 0;
                    float overheatRoll = (Rand.Range(0, 1000)) / 10f;
                    logcount++;
                    lmsg = string.Format("log {0} overheatRoll {1}", logcount, overheatRoll);
                    if (logging == true) { Log.Message(lmsg); }
                    Pawn launcherPawn = CasterPawn;
                    if (overheatRoll < overheatsOn)
                    {
                        DamageAmount = Projectile.projectile.GetDamageAmount(EquipmentSource, null);
                        msg = string.Format("{0}'s {1} critically overheated. ({2}/{3}) causing {4} damage", caster.LabelCap, EquipmentSource.LabelCap, jamRoll, jamsOn, DamageAmount);
                        if (VerbPropsOG.criticaloverheatExplosion == true) { CriticalOverheatExplosion(); }
                    }
                    else
                    {
                        DamageAmount = Projectile.projectile.GetDamageAmount(EquipmentSource, null) / 10;
                        msg = string.Format("{0}'s {1} overheated. ({2}/{3}) causing {4} damage", caster.LabelCap, EquipmentSource.LabelCap, jamRoll, jamsOn, DamageAmount);
                    }
                    bool hashediff = launcherPawn.health.hediffSet.HasHediff(HediffToAdd);
                    logcount++;
                    lmsg = string.Format("log {0} hashediff {1}", logcount, hashediff);
                    if (logging == true) { Log.Message(lmsg); }
                    var overheatOnPawn = launcherPawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffToAdd);
                    logcount++;
                    lmsg = string.Format("log {0} overheatOnPawn {1}", logcount, overheatOnPawn);
                    if (logging == true) { Log.Message(lmsg); }
                    if (hashediff == true)
                    {
                        logcount++;
                        lmsg = string.Format("log {0} overheatOnPawn Severity? {1}", logcount, overheatOnPawn.Severity);
                        if (logging == true) { Log.Message(lmsg); }
                        overheatOnPawn.Severity += DamageAmount;
                        logcount++;
                        lmsg = string.Format("log {0} overheatOnPawn.Severity {1}", logcount, overheatOnPawn.Severity);
                        if (logging == true) { Log.Message(lmsg); }
                    } else
                    {
                        logcount++;
                        lmsg = string.Format("log {0} hashediff {1}", logcount, hashediff);
                        if (logging == true) { Log.Message(lmsg); }
                        logcount++;
                        lmsg = string.Format("log {0} overheatOnPawn null?:{1}", logcount, overheatOnPawn);
                        if (logging == true) { Log.Message(lmsg); }
                        int affected = 0;
                        foreach (var part in launcherPawn.RaceProps.body.AllParts.Where(x => x.def.defName.Contains("Hand") || x.def.defName.Contains("hand")))
                        {
                            logcount++;
                            lmsg = string.Format("log {0} part.def.hitPoints {1}", logcount, launcherPawn.health.hediffSet.PartIsMissing(part));
                            if (logging == true) { Log.Message(lmsg); }
                            if (launcherPawn.health.hediffSet.PartIsMissing(part) == false&&Rand.Chance(0.5f))
                            {
                                logcount++;
                                lmsg = string.Format("log {0} part.customLabel {1}", logcount, part.def.hitPoints);
                                if (logging == true) { Log.Message(lmsg); }
                                Hediff hediff = HediffMaker.MakeHediff(HediffToAdd, launcherPawn, null);
                                hediff.Severity = Rand.Range(0, DamageAmount);
                                launcherPawn.health.AddHediff(hediff, part, null);
                                affected++;
                            }
                        }
                        /*
                        */
                    }
                    Messages.Message(msg, MessageTypeDefOf.NegativeHealthEvent);
                }
                else
                {
                    msg = string.Format("{0}'s {1} had a weapon jam. ({2}/{3})", caster.LabelCap, EquipmentSource.LabelCap, jamRoll, jamsOn);
                    Messages.Message(msg, MessageTypeDefOf.SilentInput);
                }
                if (EquipmentSource.HitPoints > 0)
                {
                    EquipmentSource.HitPoints--;
                }
                float defaultCooldownTime = this.verbProps.defaultCooldownTime * 2;
                return false;
            }
            /*
            */
            if (VerbPropsOG.canDamageWeapon)
            {
                if (VerbPropsOG.extraWeaponDamage != 0f)
                {
                    if (EquipmentSource.HitPoints - (int)VerbPropsOG.extraWeaponDamage >= 0)
                    {
                        EquipmentSource.HitPoints = EquipmentSource.HitPoints - (int)VerbPropsOG.extraWeaponDamage;
                    }
                    else if (EquipmentSource.HitPoints - (int)VerbPropsOG.extraWeaponDamage < 0)
                    {
                        EquipmentSource.HitPoints = 0;
                    }
                }
                else
                {
                    if (EquipmentSource.HitPoints > 0)
                    {
                        EquipmentSource.HitPoints--;
                    }
                }
            }

            bool flag = base.TryCastShot();
            if (flag && base.CasterIsPawn)
            {
                base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);

            }
            bool flag2 = flag && VerbPropsOG.pelletCount - 1 > 0;
            bool flag3 = flag2;
            if (flag3)
            {
                for (int i = 0; i < VerbPropsOG.pelletCount - 1; i++)
                {
                    base.TryCastShot();
                }
            }
            return flag;
        }
        /*
                protected override bool TryCastShot()
                {
                    int logcount = 0;
                    bool logging = VerbPropsOG.logging;
                    bool canDamageWeapon = VerbPropsOG.canDamageWeapon;
                    float extraWeaponDamage = VerbPropsOG.extraWeaponDamage;
                    bool canJam = VerbPropsOG.canJam;
                    string msg = string.Format("");
                    string lmsg = string.Format("log {0}", logcount);
                    string reliabilityString;
                    float jamsOn;
                    StatPart_Reliability.GetReliability((ThingDef_GunOG)EquipmentSource, out reliabilityString, out jamsOn);
                    logcount++;
                    if (logging == true) { Log.Message(lmsg); }
                    jamsOn = jamsOn++;
                    float jamRoll = 0;
                    logcount++;
                    lmsg = string.Format("log {0} jamsOn {1}", logcount, jamsOn);
                    if (logging == true) { Log.Message(lmsg); }
                    if (VerbPropsOG.overheat == true) { jamRoll = (Rand.Range(0, 100)); }
                    else { jamRoll = (Rand.Range(0, 1000)) / 10f; }
                    logcount++;
                    lmsg = string.Format("log {0} jamRoll {1}", logcount, jamRoll);
                    if (logging == true) { Log.Message(lmsg); }
                    if (jamRoll < jamsOn && canJam == true)
                    {
                        logcount++;
                        lmsg = string.Format("log {0} VerbPropsCP.overheat {1}", logcount, VerbPropsOG.overheat);
                        if (logging == true) { Log.Message(lmsg); }
                        if (VerbPropsOG.overheat == true)
                        {
                            DamageDef damageDef = Projectile.projectile.damageDef;
                            HediffDef HediffToAdd = damageDef.hediff;
                            float ArmorPenetration = Projectile.projectile.GetArmorPenetration(EquipmentSource, null);
                            float overheatsOn = VerbPropsOG.overheatsOn;
                            logcount++;
                            lmsg = string.Format("log {0} overheatsOn {1}", logcount, overheatsOn);
                            if (logging == true) { Log.Message(lmsg); }
                            int DamageAmount = 0;
                            float overheatRoll = (Rand.Range(0, 1000)) / 10f;
                            logcount++;
                            lmsg = string.Format("log {0} overheatRoll {1}", logcount, overheatRoll);
                            if (logging == true) { Log.Message(lmsg); }
                            Pawn launcherPawn = caster as Pawn;
                            if (overheatRoll < overheatsOn)
                            {
                                DamageAmount = Projectile.projectile.GetDamageAmount(EquipmentSource, null);
                                msg = string.Format("{0}'s {1} critically overheated. ({2}/{3}) causing {4} damage", caster.LabelCap, EquipmentSource.LabelCap, jamRoll, jamsOn, DamageAmount);
                                if (VerbPropsOG.criticaloverheatExplosion == true) { CriticalOverheatExplosion(); }
                            }
                            else
                            {
                                DamageAmount = Projectile.projectile.GetDamageAmount(EquipmentSource, null) / 10;
                                msg = string.Format("{0}'s {1} overheated. ({2}/{3}) causing {4} damage", caster.LabelCap, EquipmentSource.LabelCap, jamRoll, jamsOn, DamageAmount);
                            }
                            var overheatOnPawn = launcherPawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffToAdd);
                            if (overheatOnPawn != null)
                            {
                                overheatOnPawn.Severity += DamageAmount;
                            }
                            else
                            {
                                foreach (var part in launcherPawn.RaceProps.body.AllParts.Where(x => x.def.labelShort == "Hand"))
                                {
                                    logcount++;
                                    lmsg = string.Format("log {0} part.def.hitPoints {1}", logcount, launcherPawn.health.hediffSet.PartIsMissing(part));
                                    if (logging == true) { Log.Message(lmsg); }
                                    if (launcherPawn.health.hediffSet.PartIsMissing(part) == false)
                                    {
                                        logcount++;
                                        lmsg = string.Format("log {0} part.customLabel {1}", logcount, part.def.hitPoints);
                                        if (logging == true) { Log.Message(lmsg); }
                                        Hediff hediff = HediffMaker.MakeHediff(HediffToAdd, launcherPawn, null);
                                        hediff.Severity = Rand.Range(0, DamageAmount);
                                        launcherPawn.health.AddHediff(hediff, part, null);
                                    }
                                }
                            }
                            Messages.Message(msg, MessageTypeDefOf.NegativeHealthEvent);
                        }
                        else
                        {
                            msg = string.Format("{0}'s {1} had a weapon jam. ({2}/{3})", caster.LabelCap, EquipmentSource.LabelCap, jamRoll, jamsOn);
                            Messages.Message(msg, MessageTypeDefOf.SilentInput);
                        }
                        if (EquipmentSource.HitPoints > 0)
                        {
                            EquipmentSource.HitPoints--;
                        }
                        float defaultCooldownTime = this.verbProps.defaultCooldownTime * 2;
                        return false;
                    }
                    if (canDamageWeapon)
                    {
                        if (extraWeaponDamage != 0f)
                        {
                            if (EquipmentSource.HitPoints - (int)extraWeaponDamage >= 0)
                            {
                                EquipmentSource.HitPoints = EquipmentSource.HitPoints - (int)extraWeaponDamage;
                            }
                            else if (EquipmentSource.HitPoints - (int)extraWeaponDamage < 0)
                            {
                                EquipmentSource.HitPoints = 0;
                            }
                        }
                        else
                        {
                            if (EquipmentSource.HitPoints > 0)
                            {
                                EquipmentSource.HitPoints--;
                            }
                        }
                    }
                    bool flag = base.TryCastShot();
                    if (flag && base.CasterIsPawn)
                    {
                        base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);

                    }
                    bool flag2 = flag && VerbPropsOG.pelletCount - 1 > 0;
                    bool flag3 = flag2;
                    if (flag3)
                    {
                        for (int i = 0; i < VerbPropsOG.pelletCount - 1; i++)
                        {
                            base.TryCastShot();
                        }
                    }
                    return flag;
                }
        */
        public virtual void CriticalOverheatExplosion()
        {
            int logcount = 0;
            //bool logging = VerbPropsOG.logging;
            bool logging = true;
            string lmsg = string.Format("log {0}", logcount);
            logcount++; lmsg = string.Format("log {0}", logcount); if (logging == true) { Log.Message(lmsg); }
            Map map = caster.Map;
            logcount++; lmsg = string.Format("log {0} EquipmentSource.def.projectile.explosionEffect: {1}", logcount, Projectile.projectile.explosionEffect); if (logging == true) { Log.Message(lmsg); }
            if (Projectile.projectile.explosionEffect != null)
            {
                Effecter effecter = Projectile.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(EquipmentSource.Position, map, false), new TargetInfo(EquipmentSource.Position, map, false));
                effecter.Cleanup();
            }
            IntVec3 position = caster.Position;
            Map map2 = map;
            float explosionRadius = Projectile.projectile.explosionRadius;
            DamageDef damageDef = Projectile.projectile.damageDef;
            Thing launcher = EquipmentSource;
            int DamageAmount = Projectile.projectile.GetDamageAmount(EquipmentSource, null);
            logcount++; lmsg = string.Format("log {0} DamageAmount {1}", logcount, DamageAmount); if (logging == true) { Log.Message(lmsg); }
            float ArmorPenetration = Projectile.projectile.GetArmorPenetration(EquipmentSource, null);
            SoundDef soundExplode = Projectile.projectile.soundExplode;
            ThingDef equipmentDef = EquipmentSource.def;
            ThingDef def = EquipmentSource.def;
            Thing thing = EquipmentSource;
            ThingDef postExplosionSpawnThingDef = Projectile.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = Projectile.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = Projectile.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = Projectile.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, DamageAmount, ArmorPenetration, soundExplode);//, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, EquipmentSource.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, EquipmentSource.def.projectile.preExplosionSpawnChance, EquipmentSource.def.projectile.preExplosionSpawnThingCount, EquipmentSource.def.projectile.explosionChanceToStartFire, EquipmentSource.def.projectile.explosionDamageFalloff);
            return;
        }
    }
}
