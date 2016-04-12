using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Ensage;
using Ensage.Items;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.Common.Objects;
using SharpDX.Direct3D9;
using SharpDX;
using System.Diagnostics;
using System.Windows.Input;
using Ensage.Common.Extensions.SharpDX;

namespace StormSharp
{
    class Program
    {
        private static bool _loaded;
        public static Hero _me;
        public static IEnumerable<TrackingProjectile> myProjectiles;
        public static bool myAttackInAir;
        public static Ability remnant;
        public static Ability vortex;
        public static Ability zip;
        public static bool inUltimate;
        public static bool inVortex;
        public static bool inPassive;
        public static Item soulRing;
        public static Item bottle;
        public static bool inBottle;
        public static bool inFountain;
        public static Item magicStick;
        public static Item magicWand;



            



        private static readonly string Ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private static readonly Menu Menu = new Menu("StormSharp", "StormSharp", true);
        private static readonly int[] travelSpeeds = { 1250, 1875, 2500 };
        private static readonly double[] dmgPerUnit = { 0.08, 0.12, 0.16 };
        private static Font _Font;
        private static int totalDamage;
        private static bool ManaDisplayEn;
        private static float remainingMana;
        public static Hero EnemyTargetHero { get; private set; }
        private static Hero ClosestHero;
        private static Hero InitiateTarget;
        private static Hero ChazeTarget;
        private static int attackRange = 550;
        private static bool bDraw = false;
        // private static bool gothit = false;
        private static readonly Dictionary<string, bool> PreLoadItems = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> PreLoadSpell = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> skill = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> hero0_skill = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> hero1_skill = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> hero2_skill = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> hero3_skill = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> hero4_skill = new Dictionary<string, bool>();
        public static Dictionary<string, bool> EnemySpellListInGame = new Dictionary<string, bool>();
        public static List<KeyValuePair<string, bool>> EnemySpellListInGameList = new List<KeyValuePair<string, bool>>();
        public static readonly List<Hero> enemyHeros = new List<Hero>();
        private static readonly bool[] FirstTimeSpell = new bool[10];
        private static readonly bool[] FirstTimeItem = new bool[10];
        private static readonly bool[] Create = new bool[10];
        private static readonly MenuItem ChaseZipMenu = new MenuItem("Chase", "Chase");
        private static readonly MenuItem InitiateZipMenu = new MenuItem("InitiateZip", "InitiateZip");
        private static readonly MenuItem AutoAttackDodgeMenu = new MenuItem("AutoAttack Dodge", "AutoAttack Dodge").SetValue(true).SetTooltip("");
        private static readonly Menu FleeMenu = new Menu("Flee mode settings", "Flee mode settings");
        private static readonly MenuItem FleeHotkey = new MenuItem("FleeHotkey", "FleeHotkey").SetValue(new KeyBind('C', KeyBindType.Press)).SetTooltip("press to zip towards fountain");
        private static readonly MenuItem FleeDistance = new MenuItem("FleeDistance", "FleeDistance").SetValue(new Slider(500, 100, 1000)).SetTooltip("zip distance in flee mode");
        private static readonly MenuItem FleeTpEnabled = new MenuItem("FleeTpEnabled", "FleeTpEnabled").SetValue(new KeyBind('T', KeyBindType.Toggle)).SetTooltip("Enable Tp while zip-flee");
        //private static Item blink;
        private static Ability CastingDelayedSpell = new Ability();
        public static readonly List<string> SpellsList_StartDodgeWhenInAbilityPhase = new List<string>();
        public static readonly List<string> SpellsList_StartDodgeWithCastDelay = new List<string>();
        public static readonly List<string> DodgeAutoAttack_HeroList = new List<string>();
        public static readonly List<string> SpecialDodgeableSpell_HeroList = new List<string>();
        //private static Vector3 SpecialSpellDodgeZipTarget = new Vector3();
        private static bool SpellCastedInSpellsList_StartDodgeWhenInAbilityPhase = false;
        private static List<Hero> TargetListWhenCasting = new List<Hero>();
        private static List<TrackingProjectile> EnemyProjectiles = new List<TrackingProjectile>();
        private static readonly int[] TravelSpeeds = { 1250, 1875, 2500 };
        private static readonly double[] DamagePerUnit = { 0.08, 0.12, 0.16 };
        private static bool aboutToHit;
        private static List<String> ManaStackItem = new List<String>();
        private static readonly Dictionary<ItemSlot, Item> ItemSlots = new Dictionary<ItemSlot, Item>();
        private static TreadSwitch PowerTread;
        private static ZipZap ZipZap;
        private static bool AbuseDroppedItem;
        private static bool EnemyCasting;
        private static ParticleEffect meToTargetParticleEffect;
        static void Main(string[] args)
        {
            #region
            initSpellList();
            _Font = new SharpDX.Direct3D9.Font(
            Drawing.Direct3DDevice9,
            new FontDescription
            {
                FaceName = "Tahoma",
                Height = 33,
                Weight = FontWeight.Bold,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default
            });
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnDraw += Drawing_OnDraw_TP;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            Menu.AddItem(new MenuItem("Hotkey Setting", "Do you use LegacyHotkey?").SetValue(true).SetTooltip("Enable/Disable : Legacy/QWER"));
            Menu.AddItem(new MenuItem("DropManaItem", "Press to Drop Mana Item").SetValue(new KeyBind('Z', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("LockTarget", "Lock Target in Combo").SetValue(true).SetTooltip("This will lock the target while in combo"));
            //Menu.AddItem(AutoAttackDodgeMenu);
            //Menu.AddItem(NewChaseZipMenu.SetValue(new KeyBind('D', KeyBindType.Toggle)));
            Menu.AddItem(ChaseZipMenu.SetValue(new KeyBind('F', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("SelfZip", "SelfZip").SetValue(new KeyBind('W', KeyBindType.Press)));
            Menu.AddItem(InitiateZipMenu.SetValue(new KeyBind('D', KeyBindType.Toggle)));
            FleeMenu.AddItem(FleeHotkey);
            FleeMenu.AddItem(FleeDistance);
            FleeMenu.AddItem(FleeTpEnabled);
            Player.OnExecuteOrder += Player_OnExecuteAction;
            

            _loaded = false;
            Game.OnUpdate += Game_OnUpdate_myinfos;
            Game.OnUpdate += Game_OnUpdate_ChaseZip;
            Game.OnUpdate += Game_OnUpdate_SelfZip;
            Game.OnUpdate += Game_OnUpdate_Flee;
            Game.OnUpdate += Game_OnUpdate_ShowMana;
            //Game.OnUpdate += Game_OnUpdate_ShowAttackRange;
            Game.OnUpdate += Game_OnUpdate_BallLightningDodge;
            //Game.OnUpdate += Game_OnUpdate_ProjectileDodge;
            Game.OnUpdate += Game_OnUpdate_Initiate;           
            Game.OnUpdate += Game_OnUpdate_DropManaItem;
            Menu.AddSubMenu(FleeMenu);
            Menu.AddToMainMenu();
            #endregion
        }


        public static void initSpellList()
        {
            //Init Spells that will proc dodge upon ability cast
            //Mainly disjointable spells
            //Shuriken Toss, Assassinate has some exceptions
            SpellsList_StartDodgeWhenInAbilityPhase.Add("morphling_adaptive_strike");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("windrunner_shackleshot");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("broodmother_spawn_spiderlings");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("phantom_lancer_spirit_lance");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("naga_siren_ensnare");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("dazzle_poison_touch");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("viper_viper_strike");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("tidehunter_gush");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("bounty_hunter_shuriken_toss");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("chaos_knight_chaos_bolt");
            //SpellsList_StartDodgeWhenInAbilityPhase.Add("lina_laguna_blade"); //dmg calculation exception
            SpellsList_StartDodgeWhenInAbilityPhase.Add("vengefulspirit_magic_missile");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("sven_storm_bolt");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("skeleton_king_hellfire_blast");
            SpellsList_StartDodgeWhenInAbilityPhase.Add("obsidian_destroyer_arcane_orb");
            //SpellsList_StartDodgeWhenInAbilityPhase.Add("doom_bringer_doom");
            //SpellsList_StartDodgeWhenInAbilityPhase.Add("faceless_void_chronosphere");
           // SpellsList_StartDodgeWhenInAbilityPhase.Add("death_prophet_silence");
            //SpellsList_StartDodgeWhenInAbilityPhase.Add("necrolyte_reapers_scythe"); not effective
            //SpellsList_StartDodgeWhenInAbilityPhase.Add("zuus_thundergods_wrath");
            //SpellsList_StartDodgeWhenInAbilityPhase.Add("bane_fiends_grip"); ping <= 30 
            //SpellsList_StartDodgeWhenInAbilityPhase.Add("bane_brain_sap"); ping <= 30 
            //SpellsList_StartDodgeWhenInAbilityPhase.Add("beastmaster_primal_roar");

            //SpellsList_StartDodgeWhenInAbilityPhase.Add("queenofpain_scream_of_pain");
            // Spells list that dodge after a cast delay
            //SpellsList_StartDodgeWithCastDelay.Add("chaos_knight_chaos_bolt");
            //SpellsList_StartDodgeWithCastDelay.Add("lion_finger_of_death");
            //SpellsList_StartDodgeWithCastDelay.Add("beastmaster_primal_roar");
            SpellsList_StartDodgeWithCastDelay.Add("ogre_magi_fireblast");
            //SpellsList_StartDodgeWithCastDelay.Add("nyx_assassin_impale"); undodgeable
            //SpellsList_StartDodgeWithCastDelay.Add("necrolyte_reapers_scythe");
            SpellsList_StartDodgeWithCastDelay.Add("death_prophet_silence");
            //SpellsList_StartDodgeWithCastDelay.Add("zuus_thundergods_wrath");
            //SpellsList_StartDodgeWithCastDelay.Add("witch_doctor_paralyzing_cask"); not recommended
            //SpellsList_StartDodgeWithCastDelay.Add("centaur_hoof_stomp");
            //SpellsList_StartDodgeWithCastDelay.Add("lion_finger_of_death");
            //SpellsList_StartDodgeWithCastDelay.Add("doom_bringer_doom"); undodgeable
            //SpellsList_StartDodgeWithCastDelay.Add("night_stalker_crippling_fear"); undodgeable
            //SpellsList_StartDodgeWithCastDelay.Add(("zuus_lightning_bolt"); undodgeable
            //SpellsList_StartDodgeWithCastDelay.Add("lina_light_strike_array");
            //SpellsList_StartDodgeWithCastDelay.Add("queenofpain_shadow_strike");
            //SpellsList_StartDodgeWithCastDelay.Add("queenofpain_scream_of_pain");
            //
            DodgeAutoAttack_HeroList.Add("npc_dota_hero_obsidian_destroyer");

            SpecialDodgeableSpell_HeroList.Add("queenofpain_scream_of_pain");

            //Init ManaItem List
            ManaStackItem.Add("item_magic_stick");
            ManaStackItem.Add("item_branches");
            ManaStackItem.Add("item_mantle");
            ManaStackItem.Add("item_circlet");
            ManaStackItem.Add("item_robe");
            ManaStackItem.Add("item_staff_of_wizardry");
            ManaStackItem.Add("item_null_talisman");
            ManaStackItem.Add("item_orchid");
            ManaStackItem.Add("item_point_booster");
            ManaStackItem.Add("item_arcane_boots");
            ManaStackItem.Add("item_cyclone");
            ManaStackItem.Add("item_oblivion_staff");
            ManaStackItem.Add("item_energy_booster");
            ManaStackItem.Add("item_bloodstone");
            ManaStackItem.Add("item_soul_booster");
            ManaStackItem.Add("item_veil_of_discord");
            ManaStackItem.Add("item_energy_booster");       
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!_loaded)
            {
                if (!Game.IsInGame || _me == null)
                {
                    return;
                }
                if (_me.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit)
                {
                    ItemSlots.Clear();
                    _loaded = true;
                    PrintSuccess("Zhfyang: StormSharp loaded! v" + Ver);

                }
                if (!Game.IsInGame || _me == null)
                {
                    _loaded = false;
                    PrintInfo("Zhfyang: StormSharp unLoaded");
                    return;
                }
            }
            try {
                if (Menu.Item("LockTarget").GetValue<bool>())
                {

                    if (ClosestHero == null)
                    {
                        ClosestHero = _me.ClosestToMouseTarget(1000);
                    }
                }
                else
                {
                    ClosestHero = _me.ClosestToMouseTarget(1000);
                }

                if (ClosestHero == null) {

                }
                else
                {
                    if (ClosestHero.Distance2D(Game.MousePosition) > 5000)
                    {
                        //ChaseZipMenu.SetValue(new KeyBind(ChaseZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                        //InitiateZipMenu.SetValue(new KeyBind(InitiateZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                        return;
                    }
                }
                //select the target
                //there is no priority between initiate zip and chase zip
                if (!ChaseZipMenu.GetValue<KeyBind>().Active && !InitiateZipMenu.GetValue<KeyBind>().Active)
                {
                    ClosestHero = null;
                    meToTargetParticleEffect.Dispose();
                    meToTargetParticleEffect = null;
                    return;
                }
                else
                {                  
                    if (ChaseZipMenu.GetValue<KeyBind>().Active && !InitiateZipMenu.GetValue<KeyBind>().Active)
                    {
                        //disable initiate 
                        InitiateZipMenu.SetValue(new KeyBind(InitiateZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                        var startPos = new Vector2(Drawing.Width - 100, 100);
                        var size = new Vector2(90, 90);
                        Drawing.DrawRect(startPos, size, new Color(0, 0, 0, 100));
                        Drawing.DrawRect(startPos, size, new Color(0, 0, 0, 255), true);
                        Drawing.DrawText("Chasing!!", startPos + new Vector2(10, 10), new Vector2(20), new Color(0, 155, 255),
                            FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom |
                            FontFlags.StrikeOut);
                        if (ClosestHero != null)
                        {
                            if (ClosestHero.IsAlive)
                            {
                                var name = "materials/ensage_ui/heroes_horizontal/" + ClosestHero.Name.Replace("npc_dota_hero_", "") + ".vmat";
                                size = new Vector2(50, 50);
                                Drawing.DrawRect(startPos + new Vector2(10, 35), size + new Vector2(13, -6),
                                    Drawing.GetTexture(name));
                                Drawing.DrawRect(startPos + new Vector2(10, 35), size + new Vector2(14, -5),
                                    new Color(0, 0, 0, 255), true);
                            }
                        }
                    } else if (InitiateZipMenu.GetValue<KeyBind>().Active)
                    {
                        //disable chase zip
                        ChaseZipMenu.SetValue(new KeyBind(ChaseZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                        var startPos = new Vector2(Drawing.Width - 100, 150);
                        var size = new Vector2(90, 90);
                        Drawing.DrawRect(startPos, size, new Color(0, 0, 0, 100));
                        Drawing.DrawRect(startPos, size, new Color(0, 0, 0, 255), true);
                        Drawing.DrawText("Initiating!!", startPos + new Vector2(10, 10), new Vector2(20), new Color(0, 155, 255),
                            FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom |
                            FontFlags.StrikeOut);
                        if (ClosestHero != null)
                        {
                            if (ClosestHero.IsAlive)
                            {
                                var name = "materials/ensage_ui/heroes_horizontal/" + ClosestHero.Name.Replace("npc_dota_hero_", "") + ".vmat";
                                size = new Vector2(50, 50);
                                Drawing.DrawRect(startPos + new Vector2(10, 35), size + new Vector2(13, -6),
                                    Drawing.GetTexture(name));
                                Drawing.DrawRect(startPos + new Vector2(10, 35), size + new Vector2(14, -5),
                                    new Color(0, 0, 0, 255), true);
                            }
                        }
                    }
                    else
                    {
                        //disable both
                    }

                    if (_me.IsAlive && ClosestHero != null && ClosestHero.IsValid && !ClosestHero.IsIllusion && ClosestHero.IsAlive && ClosestHero.IsVisible)
                        DrawTarget(ClosestHero);
                    else if (meToTargetParticleEffect != null)
                    {
                        meToTargetParticleEffect.Dispose();
                        meToTargetParticleEffect = null;
                    }

                }
            }
            catch
            {

            }
        }

        private static void DrawTarget(Hero _target)
        {
            if (meToTargetParticleEffect == null)
            {
                meToTargetParticleEffect = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", _target);     //target inditcator
                meToTargetParticleEffect.SetControlPoint(2, new Vector3(_me.Position.X, _me.Position.Y, _me.Position.Z));             //start point XYZ
                meToTargetParticleEffect.SetControlPoint(6, new Vector3(1, 0, 0));                                                    // 1 means the particle is visible
                meToTargetParticleEffect.SetControlPoint(7, new Vector3(_target.Position.X, _target.Position.Y, _target.Position.Z)); //end point XYZ
            }
            else //updating positions
            {
                meToTargetParticleEffect.SetControlPoint(2, new Vector3(_me.Position.X, _me.Position.Y, _me.Position.Z));
                meToTargetParticleEffect.SetControlPoint(6, new Vector3(1, 0, 0));
                meToTargetParticleEffect.SetControlPoint(7, new Vector3(_target.Position.X, _target.Position.Y, _target.Position.Z));
            }
        }

        private static void Drawing_OnDraw_TP(EventArgs args)
        {
            if (!_loaded)
            {
                if (!Game.IsInGame || _me == null)
                {
                    return;
                }
                if (_me.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit)
                {
                    ItemSlots.Clear();
                    _loaded = true;
                    PrintSuccess("Zhfyang: StormSharp loaded! v" + Ver);

                }
                if (!Game.IsInGame || _me == null)
                {
                    _loaded = false;
                    PrintInfo("Zhfyang: StormSharp unLoaded");
                    return;
                }
            }

                if (FleeTpEnabled.GetValue<KeyBind>().Active)
            {
                var startPos = new Vector2(Drawing.Width - 120, 50);
                var size = new Vector2(90, 90);
                Drawing.DrawText("Tp Enabled(" + Utils.KeyToText(FleeTpEnabled.GetValue<KeyBind>().Key) + ")", startPos + new Vector2(10, 10), new Vector2(20), new Color(0, 155, 255),
                    FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom |
                    FontFlags.StrikeOut);
            }
            else
            {
                var startPos = new Vector2(Drawing.Width - 120, 50);
                var size = new Vector2(90, 90);
                Drawing.DrawText("Tp Disabled(" + Utils.KeyToText(FleeTpEnabled.GetValue<KeyBind>().Key) + ")", startPos + new Vector2(10, 10), new Vector2(20), new Color(0, 155, 255),
                    FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom |
                    FontFlags.StrikeOut);
            }
        }

        private static void Game_OnUpdate_myinfos(EventArgs args)
        {
            _me = ObjectManager.LocalHero;
            if (!_loaded)
            {
                if (!Game.IsInGame || _me == null)
                {
                    return;
                }
                if (_me.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit)
                {                   
                    
                    _loaded = true;
                    //ItemSlots.Clear();
                    PrintSuccess("Zhfyang: StormSharp loaded! v" + Ver);

                }
                if (!Game.IsInGame || _me == null)
                {
                    _loaded = false;
                    PrintInfo("Zhfyang: StormSharp unLoaded");
                    return;
                }
                remnant = _me.Spellbook.Spell1;
                vortex = _me.Spellbook.Spell2;
                zip = _me.Spellbook.Spell4;             
                
                //if Stop or Hold is issued,          

            }
        }

        private static void Game_OnUpdate_Flee(EventArgs args)
        {
            _me = ObjectManager.LocalHero;
            if (!_loaded)
            {
                if (!Game.IsInGame || _me == null)
                {
                    return;
                }
                if (_me.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit)
                {

                    _loaded = true;
                    //ItemSlots.Clear();
                    PrintSuccess("Zhfyang: StormSharp loaded! v" + Ver);

                }
                if (!Game.IsInGame || _me == null)
                {
                    _loaded = false;
                    PrintInfo("Zhfyang: StormSharp unLoaded");
                    return;
                }               
            }
            remnant = _me.Spellbook.Spell1;
            vortex = _me.Spellbook.Spell2;
            inUltimate = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
            var TP = _me.FindItem("item_tpscroll");
            var BoT_lv1 = _me.FindItem("item_travel_boots");
            var BoT_lv2 = _me.FindItem("item_travel_boots_2");

            var Fountain = ObjectManager.GetEntities<Unit>().Where(_x => _x.ClassID == ClassID.CDOTA_Unit_Fountain && _x.Team == _me.Team).FirstOrDefault();
            if (Fountain == null) return;
            var PowerTread = new TreadSwitch();

            if (!FleeHotkey.GetValue<KeyBind>().Active)
            {
                return;
            }

            // if flee mode enabled
            //disable initiate combo or chasezip combo
            ChaseZipMenu.SetValue(new KeyBind(ChaseZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            InitiateZipMenu.SetValue(new KeyBind(InitiateZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));


            if (Utils.SleepCheck("flee"))
            {
                if (zip.CanBeCasted())
                {
                    PowerTread.changePowerTread();
                    zip.UseAbility(ZipZap.ZiptoFountain(_me, Fountain, FleeDistance.GetValue<Slider>().Value));
                }
                Utils.Sleep(400, "flee");
            }

            if(FleeTpEnabled.GetValue<KeyBind>().Active)
            {
                if (Utils.SleepCheck("tp"))
                {
                    if (TP.CanBeCasted())
                    {
                        TP.UseAbility(Fountain.Position);
                    }else if (BoT_lv1.CanBeCasted())
                    {
                        BoT_lv1.UseAbility(Fountain.Position);
                    }
                    else if (BoT_lv2.CanBeCasted())
                    {
                        BoT_lv2.UseAbility(Fountain.Position);
                    }                       
                    Utils.Sleep(400, "tp");
                }
            }
        }

        private static void Game_OnUpdate_DropManaItem(EventArgs args)
        {
            #region loaded
            var me = ObjectManager.LocalHero;
            
            if (!_loaded)
            {
                if (!Game.IsInGame || me == null)
                {
                    return;
                }
                if (me.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit)
                {
                    _loaded = true;
                    PrintSuccess("> Storm Annihilation Drop loaded! v" + Ver);

                }
            }

            if (!Game.IsInGame || me == null)
            {
                _loaded = false;
                PrintInfo("> Storm Annihilation unLoaded");
                return;
            }
            
            #endregion
            var IsEnemyHeroesNearby =
                ObjectManager.GetEntities<Hero>()
                    .Any(
                        x =>
                            x.Team == me.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                            && x.Distance2D(me.Position) <= 500);

            var soulRing = me.FindItem("item_soul_ring");
            
            if (Menu.Item("DropManaItem").GetValue<KeyBind>().Active && Utils.SleepCheck("DropItems"))
            {
                
                AbuseDroppedItem = true;
                if (!IsEnemyHeroesNearby)
                {
                    //drop all items that are in manastacklist

                    DropItems(me);
                    if (!me.Inventory.Items.Any<Item>(x => ManaStackItem.Exists(y => y == x.Name)) && soulRing != null && soulRing.CanBeCasted())
                    {
                        soulRing.UseAbility();
                    }

                }
                else
                {
                    if (soulRing != null && soulRing.CanBeCasted())
                    {
                        soulRing.UseAbility();
                    }

                }
                Utils.Sleep(500, "DropItems");
            }
            else
            {
                //DropItem = false;
                //pickupItem 
               

            }
        


        }

        public static void Game_OnUpdate_Menu(EventArgs args)
        {
           
        }

        private static void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            ManaDisplayEn = false;
            //Disable Mana Display Once Another EventOrder comes     
            if (!_me.IsAlive)
            {
                ChaseZipMenu.SetValue(new KeyBind(ChaseZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                InitiateZipMenu.SetValue(new KeyBind(InitiateZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }       
            switch (args.Order)
            {
                
                case Order.AttackTarget:
                    {
                        break;
                    }
                case Order.AttackLocation:
                    {
                        
                        break;
                    }
                case Order.AbilityTarget:
                    {
                        ChaseZipMenu.SetValue(new KeyBind(ChaseZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                        InitiateZipMenu.SetValue(new KeyBind(InitiateZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                        break;
                    }
                case Order.AbilityLocation:
                    {
                        break;
                    }
                case Order.Ability:
                    {
                        //Console.WriteLine("Issue Ability");
                        /*
                        if (Utils.SleepCheck("PickupItems") &&
                            ObjectManager.GetEntities<PhysicalItem>().Where(x => x.Distance2D(me) < 250).Reverse().ToList().Count() != 0
                            && AbuseDroppedItem == true
                            )
                        {
                            PickUpItems();
                            Utils.Sleep(1000, "PickUpItems");
                            AbuseDroppedItem = false;
                        }
                        */
                        break;
                    }
                case Order.MoveLocation:
                    {
                        if (Utils.SleepCheck("PickupItems") &&
                            ObjectManager.GetEntities<PhysicalItem>().Where(x => x.Distance2D(_me) < 250).Reverse().ToList().Count() != 0
                            && AbuseDroppedItem == true
                            )
                        {
                            PickUpItems();
                            Utils.Sleep(1000, "PickUpItems");
                            AbuseDroppedItem = false;
                        }

                        break;
                    }
                case Order.MoveTarget:
                    {
                        break;
                    }
                case Order.Stop:
                    {
                        //disable the combo
                        ChaseZipMenu.SetValue(new KeyBind(ChaseZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                        InitiateZipMenu.SetValue(new KeyBind(InitiateZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                        break;
                    }
                case Order.Hold:
                    {
                        //disabled combo
                        ChaseZipMenu.SetValue(new KeyBind(ChaseZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                        InitiateZipMenu.SetValue(new KeyBind(InitiateZipMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                        break;
                    }
                case Order.ToggleAbility:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private static void maxManaEfficiency()
        {
            bottle = _me.FindItem("item_bottle") as Bottle;
            soulRing = _me.FindItem("item_soul_ring");
            inBottle = _me.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration");
            inFountain = _me.Modifiers.Any(x => x.Name == "modifier_fountain_rejuvenation");

            magicStick = _me.FindItem("item_magic_stick");
            magicWand = _me.FindItem("item_magic_wand");
            inBottle = _me.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration");
            EnemyTargetHero = _me.ClosestToMouseTarget(1000);
            if (_me.Health / _me.MaximumHealth > 0.3 && soulRing.CanBeCasted() & soulRing != null
                                && Utils.SleepCheck("sourlring"))
            {
                soulRing.UseAbility();
                Utils.Sleep(300, "sourlring");
            }
            //if (projectileToMe == null)
            //{
                if (bottle != null && bottle.CurrentCharges > 0 && !inBottle && bottle.CanBeCasted()
                    && Utils.SleepCheck("bottle"))
                {
                    bottle.UseAbility();
                    Utils.Sleep(300, "bottle");
                }
            //}
            if(_me.IsAttacking() 
                && _me.ClosestToMouseTarget(1000) != null 
                && magicStick != null && magicStick.CanBeCasted() 
                && magicStick.CurrentCharges > 0
                && _me.Mana/ _me.MaximumMana < 0.2
                && Utils.SleepCheck("magic_stick"))
            {
                magicStick.UseAbility();
                Utils.Sleep(300, "magic_stick");
            }
            if (_me.IsAttacking()
                && _me.ClosestToMouseTarget(1000) != null
                && magicWand != null && magicWand.CanBeCasted()
                && magicWand.CurrentCharges > 0
                && _me.Mana / _me.MaximumMana < 0.2
                && Utils.SleepCheck("magicWand"))
            {
                magicWand.UseAbility();
                Utils.Sleep(300, "magicWand");
            }

        }

        private static void Game_OnUpdate_ChaseZip(EventArgs args)
        {
            _me = ObjectManager.LocalHero;
            if (!_loaded)
            {
                if (!Game.IsInGame || _me == null)
                {
                    return;
                }
                if (_me.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit)
                {
                    _loaded = true;
                    PrintSuccess("ChaseZip loaded! v" + Ver);
                }
            }

            if (!Game.IsInGame || _me == null)
            {
                _loaded = false;
                PrintInfo("ChaseZip unLoaded");
                return;
            }
            if (_me.Spellbook.SpellR == null) return;
            zip = _me.Spellbook.SpellR;
            var zipLvl = _me.Spellbook.SpellR.Level;
            remnant = _me.Spellbook.SpellQ;
            try {           
                inUltimate = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
                inPassive = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload");
                inVortex = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_electric_vortex_self_slow");
                myProjectiles = ObjectManager.TrackingProjectiles.Where(x => x.Source.Name == _me.Name && x.Source.Team != _me.GetEnemyTeam());
                myAttackInAir = myProjectiles.ToList().Count() == 1;              
                var PowerTread = new TreadSwitch();
                if (!ChaseZipMenu.GetValue<KeyBind>().Active)
                {
                    ChazeTarget = null;
                    return;
                }
                if (ChazeTarget == null || !ChazeTarget.IsValid || !Menu.Item("LockTarget").GetValue<bool>())
                {
                    ChazeTarget = _me.ClosestToMouseTarget(1000);
                }
                if (ChazeTarget == null || !ChazeTarget.IsValid || !ChazeTarget.IsAlive) return;         

                var EnemyTarget = ChazeTarget;
                if (EnemyTarget == null) return;

                if (myAttackInAir && EnemyTarget != null)
                {
                    aboutToHit = EnemyTarget.Distance2D(myProjectiles.FirstOrDefault().Position) <= 200;
                }
                maxManaEfficiency();
                var AnyHeroInRemnantRange =
                    ObjectManager.GetEntities<Hero>()
                        .Any(
                            x =>
                                x.Team == _me.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                                && x.Distance2D(_me.Position) <= 100);

                if (Utils.SleepCheck("remnant") && remnant.CanBeCasted() && AnyHeroInRemnantRange)
                {
                    remnant.UseAbility();
                    Orbwalking.Attack(EnemyTarget, true);
                    Utils.Sleep(150, "remnant");
                }
                else
                {
                }
                if (!myAttackInAir)
                {
                    if (Utils.SleepCheck("ChaseZip"))
                    {
                        if (zip.CanBeCasted() && !zip.IsInAbilityPhase && !inUltimate
                            && ((!inPassive) || (inPassive && !_me.IsAttacking() && _me.Distance2D(EnemyTarget) > attackRange)))
                        {
                            PowerTread.changePowerTread();
                            zip.UseAbility(ZipZap.ChaseToEnemy(_me, EnemyTarget));

                        }
                        else if (Orbwalking.AttackOnCooldown())
                        {
                            Orbwalking.Orbwalk(EnemyTarget, 0, 0, false, true);
                        }
                        else
                        {
                            Orbwalking.Attack(EnemyTarget, true);
                        }
                        Utils.Sleep(200, "ChaseZip");
                    }
                }
                else
                {
                    if (aboutToHit)
                    {
                        if (Utils.SleepCheck("ChaseOrbZip"))
                        {
                            if (zip.CanBeCasted() && !zip.IsInAbilityPhase && !inUltimate)
                            {
                                PowerTread.changePowerTread();
                                zip.UseAbility(ZipZap.ChaseToEnemy(_me, EnemyTarget));
                            }
                            else if (Orbwalking.AttackOnCooldown())
                            {
                                Orbwalking.Orbwalk(EnemyTarget, 0, 0, false, true);
                            }
                            else
                            {
                                Orbwalking.Attack(EnemyTarget, true);
                            }
                            Utils.Sleep(200, "ChaseOrbZip");
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private static void Game_OnUpdate_SelfZip(EventArgs args)
        {                     
            if (!_loaded)
            {
                if (!Game.IsInGame || _me == null)
                {
                    return;
                }
                if (_me.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit)
                {
                    _loaded = true;
                    PrintSuccess("> Storm Annihilation loaded! v" + Ver);

                }
            }

            if (!Game.IsInGame || _me == null)
            {
                _loaded = false;
                PrintInfo("> Storm Annihilation unLoaded");
                return;
            }

            if (zip == null) return;
            var zipLvl = zip.Level;


            try
            {
                _me = ObjectManager.LocalHero;
                inUltimate = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
                inPassive = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload");
                myProjectiles = ObjectManager.TrackingProjectiles.Where(x => x.Source.Name == _me.Name && x.Source.Team != _me.GetEnemyTeam());
                inVortex = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_electric_vortex_self_slow");
                var inPassive0 = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload_passive");
                var inPassive1 = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload");

                if (!Menu.Item("SelfZip").GetValue<KeyBind>().Active)
                {
                    myProjectiles = null;
                    EnemyTargetHero = null;
                    myAttackInAir = false;
                    return;
                }
                EnemyTargetHero = _me.ClosestToMouseTarget(1000);
                //if (EnemyTargetHero == null) return;
                myAttackInAir = myProjectiles.ToList().Count() == 1;
                var AnyHeroInRemnantRange =
                    ObjectManager.GetEntities<Hero>()
                        .Any(
                            x =>
                                x.Team == _me.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                                && x.Distance2D(_me.Position) <= 100);
                var PowerTread = new TreadSwitch();
                maxManaEfficiency();
                if (EnemyTargetHero == null)
                {
                    if (Utils.SleepCheck("SelfZip") && zip.CanBeCasted())
                    {
                        PowerTread.changePowerTread();
                        zip.UseAbility(ZipZap.ZiptoSelf(_me));
                        Orbwalking.Attack(EnemyTargetHero, true);
                        Utils.Sleep(1000, "SelfZip");
                    }
                }
                else if (!myAttackInAir)
                {
                    if (Utils.SleepCheck("SelfZip"))
                    {
                        if (zip.CanBeCasted() && !zip.IsInAbilityPhase && !inUltimate
                            && !inPassive)
                        {
                            PowerTread.changePowerTread();
                            zip.UseAbility(ZipZap.ZiptoSelf(_me));

                        }
                        else if (Orbwalking.AttackOnCooldown())
                        {
                            Orbwalking.Orbwalk(EnemyTargetHero);
                        }
                        else
                        {
                            Orbwalking.Attack(EnemyTargetHero, true);
                        }
                        Utils.Sleep(200, "SelfZip");
                    }
                }
                else
                {
                    if (aboutToHit)
                    {
                        if (Utils.SleepCheck("SelfZip"))
                        {
                            if (zip.CanBeCasted() && !zip.IsInAbilityPhase && !inUltimate)
                            {
                                PowerTread.changePowerTread();
                                zip.UseAbility(ZipZap.ZiptoSelf(_me));
                            }
                            else if (Orbwalking.AttackOnCooldown())
                            {
                                Orbwalking.Orbwalk(EnemyTargetHero);
                            }
                            else
                            {
                                Orbwalking.Attack(EnemyTargetHero, true);
                            }
                            Utils.Sleep(200, "SelfZip");
                        }
                    }
                }

            }
            catch
            {

            }
        }        

        private static void Game_OnUpdate_ShowMana(EventArgs args)
        {
            
            if (!_loaded)
            {
                if (!Game.IsInGame || _me == null)
                {
                    return;
                }
                if (_me.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit)
                {
                    _loaded = true;
                    PrintSuccess("Show Mana loaded! v" + Ver);
                }
            }

            if (!Game.IsInGame || _me == null)
            {
                _loaded = false;
                PrintInfo("Show Mana unLoaded");
                return;
            }

            try {
                _me = ObjectManager.LocalHero;
                zip = _me.Spellbook.SpellR;
                var zipLvl = zip.Level;
                if (zipLvl == 0)
                {
                    return;
                }
                if (Menu.Item("Hotkey Setting").GetValue<bool>())
                {    // legacy hotkey
                    if (Game.IsKeyDown(Key.G) && !Game.IsChatOpen)
                    {
                        ManaDisplayEn = true;
                    }
                }
                else // qwer hotkey
                {
                    if (Game.IsKeyDown(Key.R) && !Game.IsChatOpen)
                    {
                        ManaDisplayEn = true;
                    }
                }
                if (ManaDisplayEn)
                {
                    var mousePos = Ensage.Game.MousePosition;
                    var heroPos = _me.Position;
                    int distance = (int)SharpDX.Vector3.Distance(mousePos, heroPos);
                    var travelSpeed = travelSpeeds[zipLvl - 1];
                    var damagePerUnit = dmgPerUnit[zipLvl - 1];
                    var startManaCost = 30 + _me.MaximumMana * 0.08;
                    var costPerUnit = (12 + _me.MaximumMana * 0.007) / 100.0;
                    int totalCost = (int)(startManaCost + costPerUnit * (int)Math.Floor((decimal)distance / 100) * 100);
                    var travelTime = distance / travelSpeed;
                    totalDamage = (int)(damagePerUnit * distance);
                    remainingMana = (int)(_me.Mana - totalCost + (_me.ManaRegeneration * (travelTime + 1)));

                    if (remainingMana > _me.MaximumMana) return;
                    bDraw = true;
                }
                else
                {
                    bDraw = false;
                }
            }
            catch
            {

            }
        }

        private static void Game_OnUpdate_BallLightningDodge(EventArgs args)
        {
            //update EnemySpellListInGame once we have the ability menu

            var me = ObjectManager.LocalHero;
            var PowerTread = new TreadSwitch();
            HeroDodge Dodge = new HeroDodge();
            if (!_loaded)
            {

                if (!Game.IsInGame || me == null)
                {
                    return;
                }
                if (me.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit)
                {
                    _loaded = true;
                    
                    PrintSuccess("> Zip Dodge loaded! v" + Ver);
                }
            }

            if (!Game.IsInGame || me == null)
            {
                _loaded = false;
                PrintInfo("> Zip Dodge unLoaded");
                return;
            }        

            var zip = me.Spellbook.Spell4;
            if (zip == null)
                return;
            var zipLvl = zip.Level;
            float zipSpeed = 1250 + 625 * (Convert.ToInt32(zipLvl) - 1);
            var inUltimate = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
            var inPassive = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload");
            var NearbyEnemyHeroes =
                ObjectManager.GetEntities<Hero>()
                    .Where(
                        x =>
                            x.Team == me.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                            && x.Distance2D(me.Position) <= 1000).ToList();
            if (NearbyEnemyHeroes.Count() == 0)
                return;


            var AllEnemyHeroes =
            ObjectManager.GetEntities<Hero>()
                .Where(
                    x =>
                        x.Team == me.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                        );

            EnemyCasting = false;
            foreach(var enemy in AllEnemyHeroes)
            {
                if(enemy.Spellbook.Spells.Any<Ability>(x => (x.IsInAbilityPhase == true))){
                    EnemyCasting = true;
                }
            }

            if (!EnemyCasting)
                return;

            #region CastDodge

            foreach (var enemy in NearbyEnemyHeroes)
            {
                // conditions of spells that being dodged upon spell's cast, if spells in SpellsList_StartDodgeWhenInAbilityPhase

                if (enemy.Spellbook.Spells.Any<Ability>(x => (x.IsInAbilityPhase == true)))
                {
                    var CastingSpell = enemy.Spellbook.Spells.Where<Ability>(x => (x.IsInAbilityPhase == true));
                    var EnemyFacingto = ObjectManager.GetEntities<Hero>()
                                    .Where(x => x.Distance2D(enemy) < CastingSpell.FirstOrDefault().CastRange + 150 && x.Team == me.Team) // Valid targets within range (over slightly to account for movement during abilityphase)
                                    .OrderBy(x => RadiansToFace(enemy, x))  // Orders targets based on radians required to be facing exactly
                                    .FirstOrDefault();

                    if (EnemyFacingto != null)
                    {
                        foreach (var Cspell in CastingSpell)
                        {
                            SpellCastedInSpellsList_StartDodgeWhenInAbilityPhase = false;
                            if (SpellsList_StartDodgeWhenInAbilityPhase.Exists(x => x == Cspell.Name))
                            {

                                SpellCastedInSpellsList_StartDodgeWhenInAbilityPhase = true;
                            }

                            if ((EnemyFacingto == me || IsFacing(enemy, me))//enemy facing me when casting
                                                                            //&& Math.Ceiling(Cspell.CooldownLength)  - Cspell.CooldownLength > 0 //Cspell casted have to add this later
                                && SpellCastedInSpellsList_StartDodgeWhenInAbilityPhase // one of the spells that needs to be dodged when spell start casting
                                && Utils.SleepCheck("zipDodge")
                                && zip.CanBeCasted()
                                 && IsFacing(enemy, me)
                                )
                            {
                                var zipTarget = ZipZap.StartDodgeWhenInAbilityPhase_ZipTo(me, enemy, Cspell);
                                PowerTread.changePowerTread();
                                zip.UseAbility(zipTarget);
                                Utils.Sleep(Game.Ping + 500, "zipDodge");
                            }
                        }
                    }

                }
            }
            #endregion


            Dodge.Doom();
            Dodge.BeastMaster();
            Dodge.Riki();
            Dodge.DeathProphet();       
            Dodge.Bane();
            Dodge.Centaur();

            Dodge.ShadowFiend();
            Dodge.Lina();
            Dodge.Lion();
            if (Menu.Item("AutoAttack Dodge").GetValue<bool>())
            {
                Dodge.AutoAttackDodge_OD_Enchantress();
            }



        }

        private static void Game_OnUpdate_Initiate(EventArgs args)
        {
            #region loaded
            _me = ObjectManager.LocalHero;
            if (!_loaded)
            {

                if (!Game.IsInGame || _me == null)
                {
                    return;
                }
                if (_me.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit)
                {
                    _loaded = true;
                    PrintSuccess("Initiate loaded! v" + Ver);
                }
            }

            if (!Game.IsInGame || _me == null)
            {
                _loaded = false;
                PrintInfo("> Storm Annihilation unLoaded");
                return;
            }


            if (_me.Spellbook.SpellR == null) return;
            zip = _me.Spellbook.SpellR;
            var zipLvl = _me.Spellbook.SpellR.Level;
            vortex = _me.Spellbook.SpellW;
            remnant = _me.Spellbook.SpellQ;

            #endregion
            try
            {
                
                inUltimate = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
                inPassive = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload");
                myProjectiles = ObjectManager.TrackingProjectiles.Where(x => x.Source.Name == _me.Name && x.Source.Team != _me.GetEnemyTeam());
                inVortex = _me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_electric_vortex_self_slow");
                myAttackInAir = myProjectiles.ToList().Count() == 1;
                
                var PowerTread = new TreadSwitch();

                if (!InitiateZipMenu.GetValue<KeyBind>().Active)
                {
                    InitiateTarget = null;
                    return;
                }              
                if (InitiateTarget == null || !InitiateTarget.IsValid || !Menu.Item("LockTarget").GetValue<bool>())
                {
                    InitiateTarget = _me.ClosestToMouseTarget(1000);
                }
                if (InitiateTarget == null || !InitiateTarget.IsValid || !InitiateTarget.IsAlive) return;
                var EnemyTarget = InitiateTarget;
                if (EnemyTarget == null) return;

                if (myAttackInAir && EnemyTarget != null)
                {
                    aboutToHit = EnemyTarget.Distance2D(myProjectiles.FirstOrDefault().Position) <= 100;
                }
                maxManaEfficiency();
                var AnyHeroInRemnantRange =
                    ObjectManager.GetEntities<Hero>()
                        .Any(
                            x =>
                                x.Team == _me.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                                && x.Distance2D(_me.Position) <= 100);
                if (Utils.SleepCheck("initiate"))
                {
                    //initiate: zip and pull
                    if (_me.Distance2D(EnemyTarget) > vortex.CastRange && vortex.CanBeCasted() && vortex.Cooldown == 0 && !zip.IsInAbilityPhase && !inUltimate && zip.CanBeCasted())
                    {
                        PowerTread.changePowerTread();
                        zip.CastSkillShot(EnemyTarget);
                        //Orbwalking.Attack(EnemyTargetHero, true);
                    }
                    //else if (((!inPassive || (inPassive && myAttackInAir))) && vortex.CanBeCasted()) //within vortex range
                    if (!inPassive && vortex.CanBeCasted())
                    {
                        vortex.UseAbility(EnemyTarget);
                        Orbwalking.Attack(EnemyTarget, true);
                    }
                    Utils.Sleep(200, "initiate");
                }
                
                //force attack once pulled
                if (inVortex && Utils.SleepCheck("pullattack"))
                {
                    
                    //first remnant after pull
                    if (!inPassive && remnant.CanBeCasted() && !_me.IsInvisible())
                    {
                        remnant.UseAbility();
                        Orbwalking.Attack(EnemyTarget, true);
                    }
                    else if(Orbwalking.AttackOnCooldown())
                        {
                        Orbwalking.Orbwalk(EnemyTarget, 0, 0, false, true);
                    }
                        else
                        {
                        Orbwalking.Attack(EnemyTarget, true);
                    }
                    
                    Utils.Sleep(100, "pullattack");
                }

                // following zip attack
                if (!myAttackInAir)
                {
                    if (Utils.SleepCheck("ChaseZip")){
                        if (zip.CanBeCasted() && !zip.IsInAbilityPhase && !inUltimate
                            && ((!inPassive) || (inPassive && !_me.IsAttacking() && _me.Distance2D(EnemyTarget) > attackRange))
                            && vortex.Cooldown <= 21 - vortex.Level
                            && vortex.Cooldown > 0)
                        {
                            PowerTread.changePowerTread();
                            zip.UseAbility(ZipZap.ChaseToEnemy(_me, EnemyTarget));
                            
                        }
                        else if (Orbwalking.AttackOnCooldown())
                        {
                            Orbwalking.Orbwalk(EnemyTarget, 0, 0, false, true);
                        }
                        else
                        {
                            Orbwalking.Attack(EnemyTarget, true);
                        }
                        Utils.Sleep(200, "ChaseZip");
                    }
                        
                }
                else // myattack in air
                {
                    if (aboutToHit)
                    {
                        if (Utils.SleepCheck("ChaseOrbZip"))
                        {
                            if (zip.CanBeCasted() && !zip.IsInAbilityPhase && !inUltimate
                                && vortex.Cooldown <= 21 - vortex.Level
                                && vortex.Cooldown > 0)
                            {
                                PowerTread.changePowerTread();
                                zip.UseAbility(ZipZap.ChaseToEnemy(_me, EnemyTarget));
                            }
                            else if (Orbwalking.AttackOnCooldown())
                            {
                                Orbwalking.Orbwalk(EnemyTarget, 0, 0, false, true);
                            }
                            else
                            {
                                Orbwalking.Attack(EnemyTarget, true);
                            }
                            Utils.Sleep(200, "ChaseOrbZip");
                        }
                    }
                }

                //Following zip when after pull-attack-remnant


                //following 2nd remnant after combo
                if (!inUltimate && vortex.Cooldown != 0 && vortex.Cooldown <= 20 - vortex.Level && remnant.CanBeCasted() && Utils.SleepCheck("Remnant") && AnyHeroInRemnantRange)
                {
                    remnant.UseAbility();
                    Orbwalking.Attack(EnemyTarget, true);
                    Utils.Sleep(100, "Remnant");
                }
                else
                {

                }
            }
            catch
            {

            } 
        }

        private static void DropItems(Hero me)
        {
            var items = me.Inventory.Items;
            foreach (var item in items.Where(x => !x.Equals("null") && ManaStackItem.Exists(y => y == x.Name)))
            {
                SaveItemSlot(item);
                me.DropItem(item, me.NetworkPosition, true);
            }
        }

        private static void SaveItemSlot(Item item)
        {
            var me = ObjectManager.LocalHero;
            for (var i = 0; i < 6; i++)
            {
                var currentSlot = (ItemSlot)i;
                var currentItem = me.Inventory.GetItem(currentSlot);
                if (currentItem == null || !currentItem.Equals(item) || ItemSlots.ContainsKey(currentSlot)) continue;
                ItemSlots.Add(currentSlot, item);
                break;
            }
        }

        private static void PickUpItemsOnMove(ExecuteOrderEventArgs args)
        {
            args.Process = false;
            PickUpItems();
            //Utils.Sleep(1000, "DropManaItem");
        }

        private static void PickUpItems()
        {
            var droppedItems =
                ObjectManager.GetEntities<PhysicalItem>().Where(x => x.Distance2D(_me) < 250).Reverse().ToList();

            var count = droppedItems.Count;

            //if (Utils.SleepCheck("DropManaItem")) {
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                    _me.PickUpItem(droppedItems[i], i != 0);

                foreach (var itemSlot in ItemSlots)
                    itemSlot.Value.MoveItem(itemSlot.Key);

                ItemSlots.Clear();
            }
        }

        private static double GetProjectileDistanceToMe(TrackingProjectile P, Hero me)
        {
            double distance;
            distance = Math.Sqrt(Math.Pow(P.Position.X - me.Position.X, 2) + Math.Pow(P.Position.Y - me.Position.Y, 2) + Math.Pow(P.Position.Z - me.Position.Z, 2));
            return distance;
        }

        static float TimeToTurn(Unit StartUnit, dynamic Target)
        {
            if (!(Target is Unit || Target is Vector3)) throw new ArgumentException("TimeToTurn => INVALID PARAMETERS!", "Target");
            if (Target is Unit) Target = Target.Position;

            double TurnRate = 0.5; //Game.FindKeyValues(string.Format("{0}/MovementTurnRate", StartUnit.Name), KeyValueSource.Hero).FloatValue; // (Only works in lobby)

            float deltaY = StartUnit.Position.Y - Target.Y;
            float deltaX = StartUnit.Position.X - Target.X;
            float angle = (float)(Math.Atan2(deltaY, deltaX));

            float n1 = (float)Math.Sin(StartUnit.RotationRad - angle);
            float n2 = (float)Math.Cos(StartUnit.RotationRad - angle);

            float Calc = (float)(Math.PI - Math.Abs(Math.Atan2(n1, n2)));

            if (Calc < 0.1 && Calc > -0.1) return 0;

            return (float)(Calc * (0.03 / TurnRate));
        }        

        private static void PrintInfo(string text, params object[] arguments)
        {
            PrintEncolored(text, ConsoleColor.White, arguments);
        }

        private static void PrintSuccess(string text, params object[] arguments)
        {
            PrintEncolored(text, ConsoleColor.Green, arguments);
        }

        // ReSharper disable once UnusedMember.Local
        private static void PrintError(string text, params object[] arguments)
        {
            PrintEncolored(text, ConsoleColor.Red, arguments);
        }

        private static void PrintEncolored(string text, ConsoleColor color, params object[] arguments)
        {
            var clr = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.ForegroundColor = clr;
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            _Font.Dispose();
        }

        static bool IsFacing(Unit StartUnit, dynamic Target)
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

        static float RadiansToFace(Unit StartUnit, dynamic Target)
        {
            if (!(Target is Unit || Target is Vector3)) throw new ArgumentException("RadiansToFace -> INVALID PARAMETERS!", "Target");
            if (Target is Unit) Target = Target.Position;

            float deltaY = StartUnit.Position.Y - Target.Y;
            float deltaX = StartUnit.Position.X - Target.X;
            float angle = (float)(Math.Atan2(deltaY, deltaX));

            return (float)(Math.PI - Math.Abs(Math.Atan2(Math.Sin(StartUnit.RotationRad - angle), Math.Cos(StartUnit.RotationRad - angle))));
        }

        

        static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
                return;
            //if (!Utils.SleepCheck("ShowMana")) {
            if (!bDraw) return;
            //
            _Font.DrawText(null, (remainingMana < 0 ? 0 : remainingMana).ToString(), (int)Game.MouseScreenPosition.X + 25, (int)Game.MouseScreenPosition.Y - 20, Color.Red);
        }

        static void Drawing_OnPostReset(EventArgs args)
        {
            _Font.OnResetDevice();
        }

        static void Drawing_OnPreReset(EventArgs args)
        {
            _Font.OnLostDevice();
        }
    }
}
