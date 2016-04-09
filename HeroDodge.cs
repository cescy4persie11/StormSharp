using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;
using System.Diagnostics;

namespace StormSharp
{
    class HeroDodge
    {
        private ZipZap Zip = new ZipZap();
        private static TreadSwitch PowerTread = new TreadSwitch();
        private bool FingerOnMe = false;
        private bool LagunaBladeOnMe = false;
        private bool ZRazeOnMe, XRazeOnMe, CRazeOnMe;
        private bool attackOrbDispatched;
        private static Stopwatch stopwatch = new Stopwatch();
        private static Stopwatch stopwatch1 = new Stopwatch();
        private static Stopwatch stopwatch2 = new Stopwatch();
        public void Doom()
        {
            var me = ObjectManager.LocalHero;
            Ability zip = me.Spellbook.Spell4;
            Hero Doom = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_DoomBringer && x.Team != me.Team && x.IsAlive && !x.IsIllusion).FirstOrDefault();

            if (Doom != null)
            {

                Ability Ult = Doom.Spellbook.SpellR;
                var EnemyFacingto = ObjectManager.GetEntities<Hero>()
                                    .Where(x => Distance2D(x, Doom) < Ult.CastRange + 150 && x.Team == me.Team) // Valid targets within range (over slightly to account for movement during abilityphase)
                                    .OrderBy(x => RadiansToFace(Doom, x))  // Orders targets based on radians required to be facing exactly
                                    .FirstOrDefault();
                if (EnemyFacingto == me
                    && me.Distance2D(Doom) <= 1000
                    && Ult.IsInAbilityPhase
                    && Utils.SleepCheck("DoomDodge")
                    )
                {
                    var zipTarget = ZipZap.StartDodgeWhenInAbilityPhase_ZipTo(me, Doom, Ult);
                    PowerTread.changePowerTread();
                    zip.UseAbility(zipTarget);
                    Utils.Sleep(Game.Ping + 500, "DoomDodge");
                }
            }
        }
        
        public void BeastMaster()
        {
            var me = ObjectManager.LocalHero;
            Ability zip = me.Spellbook.Spell4;
            Hero Beast = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Beastmaster && x.Team != me.Team && x.IsAlive && !x.IsIllusion).FirstOrDefault();

            if (Beast != null)
            {

                Ability Ult = Beast.Spellbook.SpellR;
                var EnemyFacingto = ObjectManager.GetEntities<Hero>()
                                    .Where(x => Distance2D(x, Beast) < Ult.CastRange + 150 && x.Team == me.Team) // Valid targets within range (over slightly to account for movement during abilityphase)
                                    .OrderBy(x => RadiansToFace(Beast, x))  // Orders targets based on radians required to be facing exactly
                                    .FirstOrDefault();
                if (EnemyFacingto == me
                    && me.Distance2D(Beast) <= 1000
                    && Ult.IsInAbilityPhase
                    && Utils.SleepCheck("RoarDodge")
                    )
                {
                    var zipTarget = ZipZap.StartDodgeWhenInAbilityPhase_ZipTo(me, Beast, Ult);
                    PowerTread.changePowerTread();
                    zip.UseAbility(zipTarget);
                    Utils.Sleep(Game.Ping + 500, "RoarDodge");
                }
            }
        }

        public void Lion()
        {
            var me = ObjectManager.LocalHero;
            Ability zip = me.Spellbook.Spell4;
            #region Lion; finger of death
            try
            {
                Hero Lion = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Lion).FirstOrDefault();

                if (Lion != null)
                {
                    if (!FingerOnMe && Lion.Spellbook.SpellR.IsInAbilityPhase)
                    {
                        if (ObjectManager.GetEntities<Hero>()
                                        .Where(x => Distance2D(x, Lion) < 1500 && x.Team == me.Team) // Valid targets within range (slightly over, to account for movement during abilityphase)
                                        .OrderBy(x => RadiansToFace(Lion, x))  // Ordering targets based on rads, facing exactly towards me
                                        .FirstOrDefault() == me) //facing towards me when casting spells
                        {
                            if (!FingerOnMe)
                            {
                                stopwatch.Start();
                            }
                            FingerOnMe = true;
                        }
                    }
                    float distance = (float)SharpDX.Vector3.Distance(Lion.Position, me.Position);
                    var myHero = ObjectManager.LocalHero;
                    var zipTarget_FingerDodge = ZipZap.ZipFacingDirection(me, 200);
                    if (IsFacing(Lion, me) && FingerOnMe && stopwatch.ElapsedMilliseconds >= (90 - (me.GetTurnTime(zipTarget_FingerDodge) * 1000) - 2 * Game.Ping))
                    {
                        stopwatch.Restart();
                        stopwatch.Stop();
                        if (FingerOnMe && Utils.SleepCheck("FingerDodge"))
                        {
                            me.Stop();
                            PowerTread.changePowerTread();
                            zip.UseAbility(zipTarget_FingerDodge);
                            Utils.Sleep(1000, "FingerDodge");
                        }
                        FingerOnMe = false;
                    }
                }
            }
            catch
            {

            }
            #endregion
        }

        public void Bane()
        {
            var me = ObjectManager.LocalHero;
            Ability zip = me.Spellbook.Spell4;
            Hero Bane = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bane && x.Team != me.Team && x.IsAlive && !x.IsIllusion).FirstOrDefault();

            if (Bane != null)
            {

                Ability Ult = Bane.Spellbook.SpellR;
                Ability Sap = Bane.Spellbook.SpellW;
                var EnemyFacingto = ObjectManager.GetEntities<Hero>()
                                    .Where(x => Distance2D(x, Bane) < Ult.CastRange + 150 && x.Team == me.Team) // Valid targets within range (over slightly to account for movement during abilityphase)
                                    .OrderBy(x => RadiansToFace(Bane, x))  // Orders targets based on radians required to be facing exactly
                                    .FirstOrDefault();
                if (EnemyFacingto == me
                    && me.Distance2D(Bane) <= 1000
                    && (Ult.IsInAbilityPhase || Sap.IsInAbilityPhase)
                    && Utils.SleepCheck("BaneDodge")
                    )
                {
                    var zipTarget = ZipZap.StartDodgeWhenInAbilityPhase_ZipTo(me, Bane, Ult);
                    PowerTread.changePowerTread();
                    zip.UseAbility(zipTarget);
                    Utils.Sleep(Game.Ping + 1000, "BaneDodge");
                }
            }
        }

        public void Centaur()
        {
            var me = ObjectManager.LocalHero;
            Ability zip = me.Spellbook.Spell4;
            Hero Centaur = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Centaur).FirstOrDefault();
            if (
                Centaur != null)
            {
                Ability HoofStomp = Centaur.Spellbook.Spell1;
                Ability DoubleEdge = Centaur.Spellbook.Spell2;
                try
                {
                    var zipTarget = ZipZap.ZipFacingDirection(me, 200);
                    if (Utils.SleepCheck("HoofDodge") && HoofStomp.IsInAbilityPhase && Distance2D(Centaur, me) <= 350 - me.HullRadius)
                    {
                        PowerTread.changePowerTread();
                        zip.UseAbility(zipTarget);
                        Utils.Sleep(1000, "HoofDodge");
                    }
                }
                catch
                {

                }
            }
        }

        public void Lina()
        {
            var me = ObjectManager.LocalHero;
            Ability zip = me.Spellbook.Spell4;
            Hero Lina = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Lina).FirstOrDefault();
            if (Lina != null)
            {
                if (!LagunaBladeOnMe && Lina.Spellbook.SpellR.IsInAbilityPhase)
                {
                    if (ObjectManager.GetEntities<Hero>()
                                    .Where(x => Distance2D(x, Lina) < 1000 && x.Team == me.Team) // Valid targets within range (over slightly to account for movement during abilityphase)
                                    .OrderBy(x => RadiansToFace(Lina, x))  // Orders targets based on radians required to be facing exactly
                                    .FirstOrDefault() == me) //facing towards me when casting spells
                    {
                        if (!LagunaBladeOnMe)
                        {
                            stopwatch.Start();
                        }
                        LagunaBladeOnMe = true;
                    }
                }
                float distance = (float)SharpDX.Vector3.Distance(Lina.Position, me.Position);
                var myHero = ObjectManager.LocalHero;
                var zipTarget_LagunaDodge = ZipZap.StartDodgeWhenInAbilityPhase_ZipTo(myHero, Lina, Lina.Spellbook.SpellR);
                if (IsFacing(Lina, me) && LagunaBladeOnMe && stopwatch.ElapsedMilliseconds >= (250 - 2 * Game.Ping - (me.GetTurnTime(zipTarget_LagunaDodge) * 1000)))
                {
                    stopwatch.Restart();
                    stopwatch.Stop();
                    if (LagunaBladeOnMe && Utils.SleepCheck("LagunaDodge"))
                    {
                        PowerTread.changePowerTread();
                        zip.UseAbility(zipTarget_LagunaDodge);
                        Utils.Sleep(1000, "LagunaDodge");
                    }
                    LagunaBladeOnMe = false;
                }
            }
        }

        public void DeathProphet()
        {
            var me = ObjectManager.LocalHero;
            Ability zip = me.Spellbook.Spell4;
            Hero DP = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_DeathProphet).FirstOrDefault();
            if (DP != null)
            {
                var Silence = DP.Spellbook.SpellW;
                var CirclePoint = me.Position;
                CirclePoint.X = DP.Position.X + Convert.ToSingle(450 * Math.Cos(DP.RotationRad));
                CirclePoint.Y = DP.Position.Y + Convert.ToSingle(450 * Math.Sin(DP.RotationRad));
                var zipTarget = ZipZap.ZipFacingDirection(me, 150);
                if (Silence.IsInAbilityPhase
                    && me.Distance2D(DP) < 1000
                    && Distance2D(me.Position, CirclePoint) <= 500 // slightly inaccurate, but should be ok
                    && Utils.SleepCheck("DPDodge")
                    )
                {
                    me.Stop();
                    PowerTread.changePowerTread();
                    zip.UseAbility(zipTarget);
                    Utils.Sleep(700, "DPDodge");
                }
            }
        }

        public void Riki()
        {
            var me = ObjectManager.LocalHero;
            Ability zip = me.Spellbook.Spell4;
            Hero Riki = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Riki).FirstOrDefault();
            if (Riki != null)
            {
                var Smoke = Riki.Spellbook.Spell1;
                var SmokePosition = me.Position;
                var Smokelvl = Smoke.Level;
                SmokePosition.X = Riki.Position.X + Convert.ToSingle(200 * Math.Cos(Riki.Rotation));
                SmokePosition.Y = Riki.Position.Y + Convert.ToSingle(200 * Math.Sin(Riki.Rotation));
                var zipTarget = ZipZap.ZipFacingDirection(me, (350 + 25 * Smokelvl));
                if (//
                    IsFacing(Riki, me)
                    && Riki.IsAttacking()
                    && Smoke.Cooldown == 0
                    && Distance2D(Riki, SmokePosition) <= 250 + 25 * Smokelvl + 50
                   )
                {

                    if (Utils.SleepCheck("RikiDodge"))
                    {
                        me.Stop();
                        PowerTread.changePowerTread();
                        zip.UseAbility(zipTarget);
                        Utils.Sleep(700, "RikiDodge");
                    }
                }
            }
        }

        public void ShadowFiend()
        {
            var me = ObjectManager.LocalHero;
            Ability zip = me.Spellbook.Spell4;
            Hero SF = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Nevermore).FirstOrDefault();
            if (SF != null)
            {
                Ability ZRaze = SF.Spellbook.Spell1;
                Ability XRaze = SF.Spellbook.Spell2;
                Ability CRaze = SF.Spellbook.Spell3;
                // ZRaze situation

                if (!ZRazeOnMe && ZRaze.IsInAbilityPhase)
                {
                    if (!ZRazeOnMe)
                    {
                        stopwatch.Start();
                    }
                    ZRazeOnMe = true;
                }
                var myHero = ObjectManager.LocalHero;
                var zipTarget = ZipZap.ZipFacingDirection(me, 150);
                var ZRazeTarget = new Vector3(Convert.ToSingle(SF.Position.X + Math.Cos(SF.RotationRad) * (200 + SF.HullRadius)), Convert.ToSingle(SF.Position.Y + Math.Sin(SF.RotationRad) * (200 + SF.HullRadius)), Convert.ToSingle(SF.Position.Z));
                if (ZRazeOnMe
                    && stopwatch.ElapsedMilliseconds >= 250 - 2 * Game.Ping
                    && Distance2D(ZRazeTarget, me) < 270)
                {
                    stopwatch.Restart();
                    stopwatch.Stop();
                    if (ZRazeOnMe && Utils.SleepCheck("ZRazeDodge"))
                    {
                        me.Stop();
                        PowerTread.changePowerTread();
                        zip.UseAbility(zipTarget);
                        Utils.Sleep(1000, "ZRazeDodge");
                    }
                    ZRazeOnMe = false;
                }

                if (stopwatch.ElapsedMilliseconds >= 1300)
                {
                    stopwatch.Restart();
                    stopwatch.Stop();
                    ZRazeOnMe = false;
                }
                //XRaze
                if (!XRazeOnMe && XRaze.IsInAbilityPhase)
                {
                    if (!XRazeOnMe)
                    {
                        stopwatch1.Start();
                    }
                    XRazeOnMe = true;
                }
                zipTarget = ZipZap.ZipFacingDirection(me, 150);
                var XRazeTarget = new Vector3(Convert.ToSingle(SF.Position.X + Math.Cos(SF.RotationRad) * (450 + SF.HullRadius)), Convert.ToSingle(SF.Position.Y + Math.Sin(SF.RotationRad) * (450 + SF.HullRadius)), Convert.ToSingle(SF.Position.Z));
                if (XRazeOnMe
                    && stopwatch1.ElapsedMilliseconds >= 250 - 2 * Game.Ping
                    && Distance2D(XRazeTarget, me) < 270)
                {
                    stopwatch1.Restart();
                    stopwatch1.Stop();
                    if (XRazeOnMe && Utils.SleepCheck("XRazeDodge"))
                    {
                        PowerTread.changePowerTread();
                        zip.UseAbility(zipTarget);
                        Utils.Sleep(1000, "XRazeDodge");
                    }
                    XRazeOnMe = false;
                }
                if (stopwatch1.ElapsedMilliseconds >= 1300)
                {
                    stopwatch1.Restart();
                    stopwatch1.Stop();
                    XRazeOnMe = false;
                }

                //CRaze
                if (!CRazeOnMe && CRaze.IsInAbilityPhase)
                {
                    if (!CRazeOnMe)
                    {
                        stopwatch2.Start();
                    }
                    CRazeOnMe = true;
                }

                zipTarget = ZipZap.ZipFacingDirection(me, 150);
                var CRazeTarget = new Vector3(Convert.ToSingle(SF.Position.X + Math.Cos(SF.RotationRad) * (700 + SF.HullRadius)), Convert.ToSingle(SF.Position.Y + Math.Sin(SF.RotationRad) * (700 + SF.HullRadius)), Convert.ToSingle(SF.Position.Z));
                if (CRazeOnMe
                    && stopwatch2.ElapsedMilliseconds >= 250 - 2 * Game.Ping
                    && Distance2D(XRazeTarget, me) < 270)
                {
                    stopwatch2.Restart();
                    stopwatch2.Stop();
                    if (CRazeOnMe && Utils.SleepCheck("CRazeDodge"))
                    {
                        PowerTread.changePowerTread();
                        zip.UseAbility(zipTarget);
                        Utils.Sleep(1000, "CRazeDodge");
                    }
                    CRazeOnMe = false;
                }

                if (stopwatch2.ElapsedMilliseconds >= 1300)
                {
                    stopwatch2.Restart();
                    stopwatch2.Stop();
                    CRazeOnMe = false;
                }
            }
        }

        public void AutoAttackDodge_OD_Enchantress()
        {
            var me = ObjectManager.LocalHero;
            Ability zip = me.Spellbook.Spell4;
            Hero OD = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Obsidian_Destroyer).FirstOrDefault();
            Hero Enchantress = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Enchantress).FirstOrDefault();
            var NearbyEnemyHeroes =
                ObjectManager.GetEntities<Hero>()
                    .Where(
                        x =>
                            x.Team == me.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                            && x.Distance2D(me.Position) <= 1000).ToList();
            #region OD autoattack
            if (OD != null)
            {
                try
                {
                    var EnemyProjectiles = ObjectManager.TrackingProjectiles.Where(x => x.Source.Team != me.Team && NearbyEnemyHeroes.Exists(y => OD.Name == x.Source.Name)).ToList();
                    if (EnemyProjectiles.Count() != 0)
                    {
                        attackOrbDispatched = true;

                    }
                    else
                    {
                        attackOrbDispatched = false;
                    }
                    AutoAttackDodge(OD, me, 10, 250, 100, EnemyProjectiles.FirstOrDefault(), attackOrbDispatched);
                }
                catch
                {

                }
            }
            #endregion

            #region Enchantress auto attack
            if (Enchantress != null)
            {
                try
                {
                    var EnemyProjectiles = ObjectManager.TrackingProjectiles.Where(x => x.Source.Team != me.Team && NearbyEnemyHeroes.Exists(y => Enchantress.Name == x.Source.Name)).ToList();
                    if (EnemyProjectiles.Count() != 0)
                    {
                        attackOrbDispatched = true;

                    }
                    else
                    {
                        attackOrbDispatched = false;
                    }
                    if (Enchantress.Mana >= 60 && Enchantress.Level >= 6)
                    {
                        AutoAttackDodge(Enchantress, me, 10, 200, 100, EnemyProjectiles.FirstOrDefault(), attackOrbDispatched);
                    }
                }
                catch
                {

                }
            }
            #endregion
        }

        private static void AutoAttackDodge(Hero enemy, Hero me, int ZipDistance, int DistanceInPosition, int projectileDistanceThreshold, TrackingProjectile attackProjectile, bool projectileInAir)
        {
            var zip = me.Spellbook.SpellR;
            if (enemy.IsAttacking() && ObjectManager.GetEntities<Hero>()
                                .Where(x => Distance2D(x, enemy) < enemy.AttackRange + 150 && x.Team == me.Team) // Valid targets within range (over slightly to account for movement during abilityphase)
                                .OrderBy(x => RadiansToFace(enemy, x))  // Orders targets based on radians required to be facing exactly
                                .FirstOrDefault() == me
                                && Distance2D(enemy, me) > DistanceInPosition
                                && (!projectileInAir)
                                // || (projectileInAir && Distance2D(attackProjectile.Position, me.Position) > projectileDistanceThreshold))
                                )
            {
                var zipTarget = ZipZap.ZipFacingDirection(me, ZipDistance);
                if (Utils.SleepCheck("AutoAttackDodge"))
                {
                    me.Stop();
                    PowerTread.changePowerTread();
                    zip.UseAbility(zipTarget);
                    Utils.Sleep(1000, "AutoAttackDodge");
                }
            }
        }

        private static float RadiansToFace(Unit StartUnit, dynamic Target)
        {
            if (!(Target is Unit || Target is Vector3)) throw new ArgumentException("RadiansToFace -> INVALID PARAMETERS!", "Target");
            if (Target is Unit) Target = Target.Position;

            float deltaY = StartUnit.Position.Y - Target.Y;
            float deltaX = StartUnit.Position.X - Target.X;
            float angle = (float)(Math.Atan2(deltaY, deltaX));

            return (float)(Math.PI - Math.Abs(Math.Atan2(Math.Sin(StartUnit.RotationRad - angle), Math.Cos(StartUnit.RotationRad - angle))));
        }

        private static bool IsFacing(Unit StartUnit, dynamic Target)
        {
            if (!(Target is Unit || Target is Vector3)) throw new ArgumentException("IsFacing => INVALID PARAMETERS!", "Target");
            if (Target is Unit) Target = Target.Position;

            float deltaY = StartUnit.Position.Y - Target.Y;
            float deltaX = StartUnit.Position.X - Target.X;
            float angle = (float)(Math.Atan2(deltaY, deltaX));

            float n1 = (float)Math.Sin(StartUnit.RotationRad - angle);
            float n2 = (float)Math.Cos(StartUnit.RotationRad - angle);

            return (Math.PI - Math.Abs(Math.Atan2(n1, n2))) < 0.15;
        }

        private static double Distance2D(dynamic A, dynamic B)
        {
            if (!(A is Unit || A is Vector3)) throw new ArgumentException("Not valid parameters, accepts Unit|Vector3 only", "A");
            if (!(B is Unit || B is Vector3)) throw new ArgumentException("Not valid parameters, accepts Unit|Vector3 only", "B");
            if (A is Unit) A = A.Position;
            if (B is Unit) B = B.Position;

            return Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
        }
    }
}
