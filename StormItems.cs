using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage.Items;
using Ensage.Common.Extensions;
using Ensage.Common;
using Ensage;

namespace StormSharp
{
    class StormItems
    {
        public void Urn(Unit _target)
        {
            var _me = ObjectManager.LocalHero;
            Item Urn = _me.FindItem("item_urn_of_shadows");
            bool inUrn = _target.HasModifier("modifier_item_urn_damage");
            if (_me.Inventory.Items.Any(x => x.Name == "item_urn_of_shadows"))
            {
                if (_me.IsAlive && !_target.IsMagicImmune() && !_me.IsInvisible()
                      && _target.Distance2D(_me) <= Urn.CastRange + 100 && Urn.CanBeCasted()
                      && !inUrn)
                {
                    if (Utils.SleepCheck("urn"))
                    {
                        Urn.UseAbility(_target);
                        Utils.Sleep(100, "urn");
                    }
                }
            }
            else
            {
                return;
            }

        }

        public void Veil(Unit _target)
        {
            var _me = ObjectManager.LocalHero;
            Item Veil = _me.FindItem("item_veil_of_discord");
            if (_me.Inventory.Items.Any(x => x.Name == "item_veil_of_discord"))
            {
                if (_me.IsAlive && !_target.IsMagicImmune() && !_me.IsInvisible()
                      && _target.Distance2D(_me) <= Veil.CastRange + 100 && Veil.CanBeCasted()
                      )
                {
                    if (Utils.SleepCheck("Veil"))
                    {
                        Veil.UseAbility(_target.Position);
                        Utils.Sleep(100, "Veil");
                    }
                }
            }
            else
            {
                return;
            }

        }

        public void Orchid(Unit _target)
        {
            var _me = ObjectManager.LocalHero;
            Item Orchid = _me.FindItem("item_orchid");
            if (_me.Inventory.Items.Any(x => x.Name == "item_orchid"))
            {
                if (_me.IsAlive && !_target.IsMagicImmune() && !_me.IsInvisible()
                      && _target.Distance2D(_me) <= Orchid.CastRange + 100 && Orchid.CanBeCasted()
                      )
                {
                    if (Utils.SleepCheck("Orchid"))
                    {
                        Orchid.UseAbility(_target);
                        Utils.Sleep(100, "Orchid");
                    }
                }
            }
            else
            {
                return;
            }

        }

        public void Bloodthorn(Unit _target)
        {
            var _me = ObjectManager.LocalHero;
            Item Bloodthorn = _me.FindItem("item_bloodthorn");
            if (_me.Inventory.Items.Any(x => x.Name == "item_bloodthorn"))
            {
                if (_me.IsAlive && !_target.IsMagicImmune() && !_me.IsInvisible()
                      && _target.Distance2D(_me) <= Bloodthorn.CastRange + 100 && Bloodthorn.CanBeCasted()
                      )
                {
                    if (Utils.SleepCheck("Bloodthorn"))
                    {
                        Bloodthorn.UseAbility(_target);
                        Utils.Sleep(100, "Bloodthorn");
                    }
                }
            }
            else
            {
                return;
            }

        }

        public void Medalion(Unit _target)
        {
            var _me = ObjectManager.LocalHero;
            Item Medalion = _me.FindItem("item_medallion_of_courage");
            if (_me.Inventory.Items.Any(x => x.Name == "item_medallion_of_courage"))
            {
                if (_me.IsAlive && !_target.IsMagicImmune() && !_me.IsInvisible()
                      && _target.Distance2D(_me) <= Medalion.CastRange + 100 && Medalion.CanBeCasted()
                      )
                {
                    if (Utils.SleepCheck("Medalion"))
                    {
                        Medalion.UseAbility(_target);
                        Utils.Sleep(100, "Medalion");
                    }
                }
            }
            else
            {
                return;
            }

        }

        public void SolarCrest(Unit _target)
        {
            var _me = ObjectManager.LocalHero;
            Item SolarCrest = _me.FindItem("item_solar_crest");
            if (_me.Inventory.Items.Any(x => x.Name == "item_solar_crest"))
            {
                if (_me.IsAlive && !_target.IsMagicImmune() && !_me.IsInvisible()
                      && _target.Distance2D(_me) <= SolarCrest.CastRange + 100 && SolarCrest.CanBeCasted()
                      )
                {
                    if (Utils.SleepCheck("SolarCrest"))
                    {
                        SolarCrest.UseAbility(_target);
                        Utils.Sleep(100, "SolarCrest");
                    }
                }
            }
            else
            {
                return;
            }

        }









    }
}
