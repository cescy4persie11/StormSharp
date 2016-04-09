using System;
using Ensage;
using Ensage.Common.Extensions;
using SharpDX;

namespace StormSharp
{
    class ZipZap
    {
        public static Vector3 StartDodgeWhenInAbilityPhase_ZipTo(Hero me, Hero enemy, Ability spell)
        {
            var zipTarget = new Vector3(0, 0, 0);
            float distance = (float)SharpDX.Vector3.Distance(enemy.Position, me.Position);
            if (
                spell.Name == "lion_finger_of_death"
                    )
            {
                zipTarget.X = me.Position.X - 200 * (enemy.Position.X - me.Position.X) / distance;
                zipTarget.Y = me.Position.Y - 200 * (enemy.Position.Y - me.Position.Y) / distance;

            }
            else if (spell.Name == "lina_laguna_blade")
            {
                zipTarget.X = me.Position.X - 200 * (enemy.Position.X - me.Position.X) / distance;
                zipTarget.Y = me.Position.Y - 200 * (enemy.Position.Y - me.Position.Y) / distance;
            }
            else if (spell.Name == "doom_bringer_doom")
            {
                //zipTarget.X = me.Position.X - 200 * (enemy.Position.X - me.Position.X) / distance;
                //zipTarget.Y = me.Position.Y - 200 * (enemy.Position.Y - me.Position.Y) / distance;
                zipTarget.X = me.Position.X + Convert.ToSingle(100 * Math.Cos(me.RotationRad));
                zipTarget.Y = me.Position.Y + Convert.ToSingle(100 * Math.Sin(me.RotationRad));
            }

            else if (spell.Name == "beastmaster_primal_roar")
            {
                var X = me.Position.X + 150 * Math.Cos(me.RotationRad);
                var Y = me.Position.Y + 150 * Math.Sin(me.RotationRad);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;
            }

            else if (spell.Name == "necrolyte_reapers_scythe")
            {

                var X = me.Position.X + 150 * Math.Cos(me.RotationRad);
                var Y = me.Position.Y + 150 * Math.Sin(me.RotationRad);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;
            }

            else if (spell.Name == "zuus_thundergods_wrath")
            {
                var X = me.Position.X + 150 * Math.Cos(me.RotationRad);
                var Y = me.Position.Y + 150 * Math.Sin(me.RotationRad);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;
            }
            else if (spell.Name == "bane_fiends_grip")
            {
                var X = me.Position.X + 150 * Math.Cos(me.RotationRad);
                var Y = me.Position.Y + 150 * Math.Sin(me.RotationRad);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;
            }
            else if (spell.Name == "bane_brain_sap")
            {
                var X = me.Position.X + 150 * Math.Cos(me.RotationRad);
                var Y = me.Position.Y + 150 * Math.Sin(me.RotationRad);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;
            }
            /*
            else if (spell.Name == "nyx_assassin_mana_burn")
            {
                var X = me.Position.X + 0 * Math.Cos(me.RotationRad);
                var Y = me.Position.Y + 0 * Math.Sin(me.RotationRad);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;
            }
            */
            else
            {
                zipTarget.X = me.Position.X;
                zipTarget.Y = me.Position.Y;
            }
            return zipTarget;
        }

        public static Vector3 ZipFacingMouse(Hero me, float D)
        {
            //me.Stop();
            var zipTarget = Game.MousePosition;
            var MouseLoc = Game.MousePosition;
            double X;
            double Y;
            if (me.Distance2D(zipTarget) > 30 && me.Distance2D(zipTarget) < 1200)
            {
                X = me.Position.X + (MouseLoc.X - me.Position.X) * D / me.Distance2D(MouseLoc);
                Y = me.Position.Y + (MouseLoc.Y - me.Position.Y) * D / me.Distance2D(MouseLoc);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;
            }
            else if(me.ClosestToMouseTarget(100) != null)
            {
                zipTarget = me.ClosestToMouseTarget(100).Position;
            }
            else
            {
                zipTarget = me.Position;
            }
            return zipTarget;
        }

        public static Vector3 ZipFacingEnemy(Hero me, Hero enemy, float D)
        {
            //me.Stop();
            
            var zipTarget = enemy.Position;
            var enemyPos = enemy.Position;
            double X;
            double Y;
            if (me.Distance2D(zipTarget) > 100)
            {
                X = me.Position.X + (enemyPos.X - me.Position.X) * D / me.Distance2D(zipTarget);
                Y = me.Position.Y + (enemyPos.Y - me.Position.Y) * D / me.Distance2D(zipTarget);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;
            }
            else if (me.ClosestToMouseTarget(500) != null)
            {
                zipTarget = me.ClosestToMouseTarget(500).Position;
            }
            else
            {
                zipTarget = enemyPos;
            }
            return zipTarget;
        }

        public static Vector3 AutoAttackDodge_ZipTo(TrackingProjectile P, Hero me, float D)
        {
            var zipTarget = new Vector3(0, 0, 0);
            double X;
            double Y;
            switch (P.Source.Name)
            {
                case "npc_dota_hero_obsidian_destroyer":
                    {
                        X = me.Position.X + D * Math.Cos(me.RotationRad);
                        Y = me.Position.Y + D * Math.Sin(me.RotationRad);
                        zipTarget.X = Convert.ToSingle(X);
                        zipTarget.Y = Convert.ToSingle(Y);
                        zipTarget.Z = me.Position.Z;
                        break;
                    }
                case "queenofpain_scream_of_pain":
                    {
                        X = me.Position.X + D * Math.Cos(me.RotationRad);
                        Y = me.Position.Y + D * Math.Sin(me.RotationRad);
                        zipTarget.X = Convert.ToSingle(X);
                        zipTarget.Y = Convert.ToSingle(Y);
                        zipTarget.Z = me.Position.Z;
                        break;
                    }
                case "npc_dota_hero_centaur":
                    {
                        X = me.Position.X + D * Math.Cos(me.RotationRad);
                        Y = me.Position.Y + D * Math.Sin(me.RotationRad);
                        zipTarget.X = Convert.ToSingle(X);
                        zipTarget.Y = Convert.ToSingle(Y);
                        zipTarget.Z = me.Position.Z;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return zipTarget;
        }

        public static Vector3 SpellDodge_ZipFront(TrackingProjectile P, Hero me, float D)
        {
            var zipTarget = new Vector3(0, 0, 0);
            double X;
            double Y;
            if (P.Source.Name == "npc_dota_hero_queenofpain" && P.Speed == 900)
            {
                X = me.Position.X + D * Math.Cos(me.RotationRad);
                Y = me.Position.Y + D * Math.Sin(me.RotationRad);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;
            }
            else
            {
                zipTarget = me.Position;
            }
            return zipTarget;
        }

        public static Vector3 Zipto_AnotherDirection(Hero enemy, Hero me)
        {
            var zipTarget = new Vector3(0, 0, 0);
            double X;
            double Y;
            double k;
            switch (enemy.Name)
            {
                case "npc_dota_hero_nyx_assassin":
                    {
                        if (me.Rotation < 0)
                        {
                            k = me.RotationRad + Math.PI / 2;
                        }
                        else
                        {
                            k = me.RotationRad - Math.PI / 2;
                        }
                        X = me.Position.X + 250 * Math.Cos(k);
                        Y = me.Position.Y + 250 * Math.Sin(k);
                        zipTarget.X = Convert.ToSingle(X);
                        zipTarget.Y = Convert.ToSingle(Y);
                        zipTarget.Z = me.Position.Z;
                        break;
                    }
                default:
                    {
                        zipTarget = me.Position;
                        break;
                    }

            }
            return zipTarget;
        }

        public static Vector3 ZiptoFountain(Hero me, Unit Fountain, int D)
        {
            var zipTarget = Fountain.Position;
            //var Fountain = ObjectManager.GetEntities<Unit>().Where(_x => _x.ClassID == ClassID.CDOTA_Unit_Fountain && _x.Team == _me.Team);
            if (Fountain == null) return zipTarget;
            zipTarget.X = me.Position.X + D / me.Distance2D(Fountain) * (Fountain.Position.X - me.Position.X);
            zipTarget.Y = me.Position.Y + D / me.Distance2D(Fountain) * (Fountain.Position.Y - me.Position.Y);
            return zipTarget;
        }

        public static Vector3 ZiptoSelf(Hero me)
        {
            var zipTarget = me.Position;
            var TargetDistance = me.Distance2D(Game.MousePosition);
            if (TargetDistance < 1200)
            {
                if (!me.IsAttacking())
                {
                    zipTarget = ZipZap.ZipFacingMouse(me,40);
                }
                else
                {
                    zipTarget = ZipZap.ZipFacingMouse(me,40);
                }
            }
            return zipTarget;
        }

        public static Vector3 ChaseToEnemy(Hero me, Hero EnemyTargetHero)
        {
            var zipTarget = ZipZap.ZipFacingMouse(me, (!me.NetworkActivity.Equals(NetworkActivity.Idle)) ? (Convert.ToSingle(me.Distance2D(Game.MousePosition) > 300 ? 100 : 50)) : 10);
            //var zipTarget = ZipZap.ZipFacingEnemy(me, EnemyTargetHero, 100);
            var attackRange = 550;
            var TargetDistance = me.Distance2D(EnemyTargetHero);

            if (TargetDistance > attackRange&& TargetDistance < 2000)
            {
                //Console.WriteLine("Distance > 650");
                //zipTarget.X = me.Position.X + (EnemyTargetHero.Position.X - me.Position.X) * (TargetDistance - attackRange + 450) / TargetDistance;
                //zipTarget.Y = me.Position.Y + (EnemyTargetHero.Position.Y - me.Position.Y) * (TargetDistance - attackRange + 450) / TargetDistance;
                zipTarget = me.Spellbook.SpellR.GetPrediction(EnemyTargetHero);
                

                //zipTarget.X = EnemyTargetHero.Position.X;
                //zipTarget.Y = EnemyTargetHero.Position.Y;
                //zipTarget.Z = me.Position.Z;
            }
            else if (TargetDistance < 300)
            {
                {
                    //Console.WriteLine("Distance < 300");
                    zipTarget = ZipZap.ZipFacingDirection(me, 50);
                }

            }
            else // 300 < D < attackrange + 100
            {
                //Console.WriteLine("300 < Distance < 650");
                zipTarget = ZipZap.ZipFacingDirection(me, 50);
            }
            return zipTarget;
        }

        public static Vector3 ChaseZip(Hero me)
        {
            var EnemyTargetHero = me.ClosestToMouseTarget(1000);
            var zipTarget = ZipZap.ZipFacingMouse(me, (!me.NetworkActivity.Equals(NetworkActivity.Idle)) ? (Convert.ToSingle(me.Distance2D(Game.MousePosition) > 300 ? 100 : 50)) : 10);
            //var zipTarget = ZipZap.ZipFacingEnemy(me, EnemyTargetHero, 100);
            var attackRange = 550;         
            var TargetDistance = me.Distance2D(EnemyTargetHero);
            if(EnemyTargetHero == null) {
                //zipTarget = ZipZap.ZipFacingDirection(me, 100);
            }
            else if (TargetDistance > attackRange + 150 && TargetDistance < 2000)
            {
                zipTarget.X = me.Position.X + (EnemyTargetHero.Position.X - me.Position.X) * (TargetDistance - attackRange + 300) / TargetDistance;
                zipTarget.Y = me.Position.Y + (EnemyTargetHero.Position.Y - me.Position.Y) * (TargetDistance - attackRange + 300) / TargetDistance;
                //zipTarget.X = EnemyTargetHero.Position.X;
                //zipTarget.Y = EnemyTargetHero.Position.Y;
                zipTarget.Z = me.Position.Z;
            }        
            else if(TargetDistance < 300)
            {
                {
                    zipTarget = ZipZap.ZipFacingMouse(me, 50);
                }

            }
            else
            {
                zipTarget = ZipZap.ZipFacingMouse(me, (!me.NetworkActivity.Equals(NetworkActivity.Idle)) ? (Convert.ToSingle(me.Distance2D(Game.MousePosition) > 300 ? 80 : 80)) : 10);
            }
            return zipTarget;
        }

        public static Vector3 AutoAttackDodge_ZipToAroundCircle(TrackingProjectile P, Hero me)
        {
            var zipTarget = new Vector3(0, 0, 0);
            double X;
            double Y;
            double k;
            switch (P.Source.Name)
            {
                case "npc_dota_hero_obsidian_destroyer":
                    {
                        if (me.Rotation < 0)
                        {
                            k = me.RotationRad + Math.PI / 2;
                        }
                        else
                        {
                            k = me.RotationRad - Math.PI / 2;
                        }
                        X = me.Position.X + 100 * Math.Cos(k);
                        Y = me.Position.Y + 100 * Math.Sin(k);
                        zipTarget.X = Convert.ToSingle(X);
                        zipTarget.Y = Convert.ToSingle(Y);
                        zipTarget.Z = me.Position.Z;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
            return zipTarget;
        }

        public static Vector3 ZipFacingDirection(Hero me, float D)
        {
            var zipTarget = new Vector3(0, 0, 0);
            double X;
            double Y;
            X = me.Position.X + D * Math.Cos(me.RotationRad);
            Y = me.Position.Y + D * Math.Sin(me.RotationRad);
            zipTarget.X = Convert.ToSingle(X);
            zipTarget.Y = Convert.ToSingle(Y);
            zipTarget.Z = me.Position.Z;
            return zipTarget;
        }

    }
}
