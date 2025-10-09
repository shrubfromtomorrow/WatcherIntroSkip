using System;
using System.Text.RegularExpressions;
using MonoMod.Cil;
using UnityEngine;
using Watcher;


namespace WatcherIntroSkip
{
    public class Hooks
    {

        public static void Apply()
        {
            On.RainWorldGame.ctor += RainWorldGame_ctor;
            On.Menu.SleepAndDeathScreen.ctor += SleepAndDeathScreen_ctor;
            On.Menu.PauseMenu.ctor += PauseMenu_ctor;

            IL.Menu.SlugcatSelectMenu.StartGame += SlugcatSelectMenu_StartGame;
        }

        private static void PauseMenu_ctor(On.Menu.PauseMenu.orig_ctor orig, Menu.PauseMenu self, ProcessManager manager, RainWorldGame game)
        {
            orig(self, manager, game);

            if (!Plugin.instance.warped && game.IsStorySession && game.GetStorySession.saveStateNumber == WatcherEnums.SlugcatStatsName.Watcher)
            {
                Plugin.container = new UI(self, self.pages[0], self.pages[0].pos, new Vector2());
                self.pages[0].subObjects.Add(Plugin.container);
            }
        }

        private static void SlugcatSelectMenu_StartGame(ILContext il)
        {
            var c = new ILCursor(il);

            try
            {
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdarg(0),
                    i => i.MatchLdfld(typeof(MainLoopProcess).GetField("manager")),
                    i => i.MatchLdsfld(typeof(ProcessManager.ProcessID).GetField("Game")),
                    i => i.MatchCallvirt(typeof(ProcessManager).GetMethod("RequestMainProcessSwitch", new[] { typeof(ProcessManager.ProcessID) }))))
                {
                    c.EmitDelegate<Action>(() =>
                    {
                        if (Plugin.instance != null)
                            Plugin.instance.warped = false;
                    });
                }
                else
                {
                    Plugin.logger?.LogError("Failed to find IL pattern in SlugcatSelectMenu.StartGame");
                }
            }
            catch (Exception ex)
            {
                Plugin.logger?.LogError($"Exception in IL hook for SlugcatSelectMenu.StartGame: {ex}");
            }
        }


        private static void SleepAndDeathScreen_ctor(On.Menu.SleepAndDeathScreen.orig_ctor orig, Menu.SleepAndDeathScreen self, ProcessManager manager, ProcessManager.ProcessID ID)
        {
            orig(self, manager, ID);
            if (Plugin.instance.warped && RainWorld.lockGameTimer)
            {
                RainWorld.lockGameTimer = false;
            }
        }

        private static void RainWorldGame_ctor(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
        {
            orig(self, manager);
            Plugin.game = self;
            if (!Plugin.instance.warped)
            {
                RainWorld.lockGameTimer = true;
            }
        }

        public static void SpawnWarp(string region, string karmaReinforced)
        {
            Plugin.game.ContinuePaused();

            var storySession = Plugin.game.session as StoryGameSession;
            var rippleData = storySession.saveState.deathPersistentSaveData;
            rippleData.minimumRippleLevel = 1f;
            rippleData.rippleLevel = 1f;
            rippleData.maximumRippleLevel = 1f;

            if (karmaReinforced == "true")
            {
                storySession.saveState.deathPersistentSaveData.reinforcedKarma = true;
            }

            string warpDest;
            switch (region)
            {
                case "WSKB":
                    warpDest = Plugin.CCWarp;
                    break;
                case "WRFA":
                    warpDest = Plugin.LFWarp;
                    break;
                case "WSKA":
                    warpDest = Plugin.SHWarp;
                    break;
                default:
                    warpDest = "WSKB";
                    break;
            }

            var spinningTopData = new PlacedObject(PlacedObject.Type.None, null);
            spinningTopData.FromString(Regex.Split(warpDest, "><"));

            var player = Plugin.game.Players[0];
            var currentRoom = player.Room.realizedRoom;
            var warpData = (spinningTopData.data as SpinningTopData).CreateWarpPointData(currentRoom);

            var placedWarp = new PlacedObject(PlacedObject.Type.WarpPoint, warpData)
            {
                pos = player.realizedCreature.bodyChunks[0].pos
            };

            var warpPoint = currentRoom.ForceSpawnWarpPoint(placedWarp, true);
            if (warpPoint != null && !warpData.rippleWarp)
            {
                warpPoint.triggerTime = warpPoint.triggerActivationTime - 1f;
                warpPoint.strongPull = true;
                warpPoint.guaranteeTrigger = true;
            }

            Plugin.game.GetStorySession.spinningTopWarpsLeadingToRippleScreen.Add(warpPoint.MyIdentifyingString());
            Plugin.container = null;
            Plugin.instance.warped = true;
        }
    }
}
