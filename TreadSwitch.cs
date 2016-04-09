using System.Linq;
using Ensage;
using Ensage.Items;
using Ensage.Common.Extensions;

namespace StormSharp
{
    class TreadSwitch
    {
        public void changePowerTread()
        {
            var me = ObjectManager.LocalHero;
            var powerTreads = me.FindItem("item_power_treads") as PowerTreads;
            if (me.Inventory.Items.Any(x => x.Name == "item_power_treads"))
            {
                switch (powerTreads.ActiveAttribute)
                {
                    case Ensage.Attribute.Intelligence:
                        //powerTreads.UseAbility();
                        break;
                    case Ensage.Attribute.Strength:
                        powerTreads.UseAbility();
                        break;
                    case Ensage.Attribute.Agility:
                        powerTreads.UseAbility();
                        powerTreads.UseAbility();
                        break;
                }
            }
            else
            {
                return;
            }
        }
    }
}
