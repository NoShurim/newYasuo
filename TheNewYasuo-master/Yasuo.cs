using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using YasuoBuddy.EvadePlus;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace YasuoBuddy
{
    internal class Yasuo
    {
        public static Menu Menu, ComboMenu, HarassMenu, FarmMenu, FleeMenu, DrawMenu, MiscSettings;
        public static Spell.Targeted E;
        public static Spell.Skillshot E2;
        private static int _cleanUpTime;
        private static AIHeroClient Yasou => Player.Instance;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Yasou.Hero != Champion.Yasuo) return;

            Menu = MainMenu.AddMenu("TheNewYasuo", "yasuobuddyfluxy");

            ComboMenu = Menu.AddSubMenu("Combo", "yasuCombo");
            ComboMenu.AddGroupLabel("Combo");
            ComboMenu.Add("combo.Q", new CheckBox("Use Q"));
            ComboMenu.Add("combo.E", new CheckBox("Use E"));
            //ComboMenu.Add("combo.ToMouse", new CheckBox("Dash to Mouse", false));
            ComboMenu.Add("combo.stack", new CheckBox("Stack/Stackar Q"));
            ComboMenu.Add("combo.leftclickRape", new CheckBox("Left Click Rape"));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("Ultimate");
            ComboMenu.Add("combo.R", new CheckBox("Use R"));
            ComboMenu.Add("combo.RTarget", new CheckBox("Use R always on Selected TargetUsar R sempre no alvo selecionado"));
            ComboMenu.Add("combo.RKillable", new CheckBox("Use R KS"));
            ComboMenu.Add("combo.MinTargetsR", new Slider("Use R Min Targets/Alvos Minimos para ultar", 2, 1, 5));

            HarassMenu = Menu.AddSubMenu("Harass", "yasuHarass");
            HarassMenu.AddGroupLabel("Harass/Incomodar");
            HarassMenu.Add("harass.Q", new CheckBox("Use Q"));
            HarassMenu.Add("harass.E", new CheckBox("Use E"));
            HarassMenu.Add("harass.stack", new CheckBox("Stack/Stackar Q"));

            FarmMenu = Menu.AddSubMenu("Farming", "yasuoFarm");
            FarmMenu.AddGroupLabel("Farming");
            FarmMenu.AddLabel("Last Hit");
            FarmMenu.Add("LH.Q", new CheckBox("Use Q"));
            FarmMenu.Add("LH.E", new CheckBox("Use E"));
            FarmMenu.Add("LH.EUnderTower", new CheckBox("Use E Under Tower/Debaixo da Torre", false));

            FarmMenu.AddLabel("WaveClear");
            FarmMenu.Add("WC.Q", new CheckBox("Use Q"));
            FarmMenu.Add("WC.E", new CheckBox("Use E"));
            FarmMenu.Add("WC.EUnderTower", new CheckBox("Use E Under Tower/Debaixo da Torre", false));

            FarmMenu.AddLabel("Jungle");
            FarmMenu.Add("JNG.Q", new CheckBox("Use Q"));
            FarmMenu.Add("JNG.E", new CheckBox("Use E"));

            FleeMenu = Menu.AddSubMenu("Flee/Evade", "yasuoFlee");
            FleeMenu.AddGroupLabel("Flee");
            FleeMenu.Add("Flee.E", new CheckBox("Use E"));
            FleeMenu.Add("Flee.stack", new CheckBox("Stack/Stackar Q"));
            FleeMenu.AddGroupLabel("Evade");
            FleeMenu.Add("Evade.E", new CheckBox("Use E para desviar/to Evade"));
            FleeMenu.Add("Evade.W", new CheckBox("Use  W para desviar/to Evade"));
            FleeMenu.Add("Evade.WDelay", new Slider("Humanizer Delay (ms)", 0, 0, 1000));
            //
            FleeMenu.AddGroupLabel("WallJump");
            FleeMenu.Add("WJ", new KeyBind("Walljump Key:", false, KeyBind.BindTypes.HoldActive, 'G'));
            FleeMenu.Add("DrawSpots", new CheckBox("Draw Walljump spots"));
            //
            MiscSettings = Menu.AddSubMenu("Diversas/Misc");
            MiscSettings.AddGroupLabel("KS");
            MiscSettings.Add("KS.Q", new CheckBox("Use Q"));
            MiscSettings.Add("KS.E", new CheckBox("Use E"));
            MiscSettings.AddGroupLabel("Auto Q");
            MiscSettings.Add("Auto.Q3", new CheckBox("Use Q3"));
            MiscSettings.Add("Auto.Active", new KeyBind("Auto Q Inimigo/Enemy", true, KeyBind.BindTypes.PressToggle, 'M'));

            Program.Main(null);

            DrawMenu = Menu.AddSubMenu("Draw", "yasuoDraw");
            DrawMenu.AddGroupLabel("Draw Config");

            DrawMenu.Add("Draw.Q", new CheckBox("Draw Q", true));
            DrawMenu.AddColourItem("Draw.Q.Colour");
            DrawMenu.AddSeparator();

            DrawMenu.Add("Draw.E", new CheckBox("Draw E", false));
            DrawMenu.AddColourItem("Draw.E.Colour");
            DrawMenu.AddSeparator();

            DrawMenu.Add("Draw.R", new CheckBox("Draw R", false));
            DrawMenu.AddColourItem("Draw.R.Colour");
            DrawMenu.AddSeparator();

            DrawMenu.AddLabel("CoolDown Colour/Cor:", 4);
            DrawMenu.AddColourItem("Draw.Down", 7);
            
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            EEvader.Init();

            //

            E = new Spell.Targeted(SpellSlot.E, 475);
            E2 = new Spell.Skillshot(SpellSlot.E, 475, EloBuddy.SDK.Enumerations.SkillShotType.Linear);


            //
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["Draw.Q"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(
                    SpellManager.Q.IsReady() ? DrawMenu.GetColour("Draw.Q.Colour") : DrawMenu.GetColour("Draw.Down"),
                    SpellManager.Q.Range, Player.Instance.Position);
            }
            if (DrawMenu["Draw.R"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(
                    SpellManager.R.IsReady() ? DrawMenu.GetColour("Draw.R.Colour") : DrawMenu.GetColour("Draw.Down"),
                    SpellManager.R.Range, Player.Instance.Position);
            }
            if (DrawMenu["Draw.E"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(
                    SpellManager.E.IsReady() ? DrawMenu.GetColour("Draw.E.Colour") : DrawMenu.GetColour("Draw.Down"),
                    SpellManager.E.Range, Player.Instance.Position);
            }
            if (FleeMenu["DrawSpots"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(YasuoWall.spotA.To3D(), 100, System.Drawing.Color.HotPink);//BlueVermelho*
                //Drawing.DrawCircle(YasuoWall.spotB.To3D(), 100, System.Drawing.Color.HotPink);//RedAzul*
                Drawing.DrawCircle(YasuoWall.spotC.To3D(), 100, System.Drawing.Color.HotPink);//SapoAzul*
                Drawing.DrawCircle(YasuoWall.spotD.To3D(), 130, System.Drawing.Color.HotPink);//PassaroAzul
                Drawing.DrawCircle(YasuoWall.spotP.To3D(), 150, System.Drawing.Color.HotPink);//PassaroAzul2
                Drawing.DrawCircle(YasuoWall.spotE.To3D(), 100, System.Drawing.Color.HotPink);//PedraAzul*
                //Drawing.DrawCircle(YasuoWall.spotF.To3D(), 100, System.Drawing.Color.HotPink);//BlueAzul*
                Drawing.DrawCircle(YasuoWall.spotG.To3D(), 100, System.Drawing.Color.HotPink);//LoboAzulBaixo*
                Drawing.DrawCircle(YasuoWall.spotH.To3D(), 100, System.Drawing.Color.HotPink);//SapoVermelho*
                Drawing.DrawCircle(YasuoWall.spotI.To3D(), 60, System.Drawing.Color.HotPink);//LoboVermelhoBaixo*
                Drawing.DrawCircle(YasuoWall.spotJ.To3D(), 60, System.Drawing.Color.HotPink);//LoboVermelhoCima*
                //Drawing.DrawCircle(YasuoWall.spotK.To3D(), 60, System.Drawing.Color.HotPink);//LoboAzulCima
                Drawing.DrawCircle(YasuoWall.spotL.To3D(), 100, System.Drawing.Color.HotPink);//PedraVermelho*
                //Drawing.DrawCircle(YasuoWall.spotM.To3D(), 60, System.Drawing.Color.HotPink);//RedVermelhoBaixo*
                Drawing.DrawCircle(YasuoWall.spotN.To3D(), 100, System.Drawing.Color.HotPink);//PassaroVermelho*
                //Drawing.DrawCircle(YasuoWall.spotO.To3D(), 60, System.Drawing.Color.HotPink);//RedVermelhoCima*
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (_cleanUpTime < Environment.TickCount)
            {
                GC.Collect();
                _cleanUpTime = Environment.TickCount + 1000000;
            }
            StateManager.KillSteal();
            if (MiscSettings["Auto.Active"].Cast<KeyBind>().CurrentValue)
            {
                StateManager.AutoQ();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                StateManager.Flee();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                StateManager.Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                StateManager.Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                StateManager.LastHit();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                StateManager.WaveClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                StateManager.Jungle();
            }
            if (FleeMenu["WJ"].Cast<KeyBind>().CurrentValue)
            {
                YasuoWall.WallDash();
            }
        }
    }
}