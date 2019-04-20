using System;
using System.Collections.Generic;
using Verse;

namespace CombatExtended
{
    // Token: 0x02000035 RID: 53
    public class ThingDef_LaserProjectile : AmmoDef
    {
        // Token: 0x040000CA RID: 202
        public float preFiringInitialIntensity = 0f;

        // Token: 0x040000CB RID: 203
        public float preFiringFinalIntensity = 0f;

        // Token: 0x040000CC RID: 204
        public float postFiringInitialIntensity = 0f;

        // Token: 0x040000CD RID: 205
        public float postFiringFinalIntensity = 0f;

        // Token: 0x040000CE RID: 206
        public string warmupGraphicPathSingle = null;

        // Token: 0x040000CF RID: 207
        public int preFiringDuration = 0;

        // Token: 0x040000D0 RID: 208
        public int postFiringDuration = 0;

        // Token: 0x040000D1 RID: 209
        public float StartFireChance;

        // Token: 0x040000D2 RID: 210
        public bool CanStartFire = false;

        // Token: 0x040000D3 RID: 211
        public List<Projectile_LaserConfig> graphicSettings = null;

        // Token: 0x040000D4 RID: 212
        public bool cycleThroughFiringPositions = false;

        // Token: 0x040000D5 RID: 213
        public bool createsExplosion = false;
    }
}
