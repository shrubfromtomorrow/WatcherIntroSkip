using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Watcher;


namespace WatcherIntroSkip
{
    // Stolen from rwsqol
    public class Hooks
    {
        public static bool Toggled => Plugin.Instance.options.WatcherIntroSkip.Value;
        public static string EntryRegion => Plugin.Instance.options.WISRegionString.Value;
        public static bool KarmaReinforced => Plugin.Instance.options.WISReinforcedKarma.Value;
        public static bool SpreadRot => Plugin.Instance.options.WISSpreadRot.Value;

        public static readonly Dictionary<string, string> regionToPO = new Dictionary<string, string>
        {
            { "Sunbaked Alley", "SpinningTopSpot><933.9406><2132.439><18~77~Watcher~WSKB~wskb_c17~490~510~15"},
            { "Coral Caves", "SpinningTopSpot><591.3841><1316.087><37~40~Watcher~WRFA~wrfa_sk04~550~450~16"},
            { "Torrential Railways", "SpinningTopSpot><647.808><357.6914><44~106~Watcher~WSKA~wska_d27~470~410~17"}
        };

        public static void Apply()
        {
            On.StoryGameSession.ctor += StoryGameSession_ctor;
            On.Watcher.WarpPoint.NewWorldLoaded_Room += WarpPoint_NewWorldLoaded_Room;
        }

        private static void StoryGameSession_ctor(On.StoryGameSession.orig_ctor orig, StoryGameSession self, SlugcatStats.Name saveStateNumber, RainWorldGame game)
        {
            orig(self, saveStateNumber, game);
            if (!ModManager.Watcher || saveStateNumber != WatcherEnums.SlugcatStatsName.Watcher || !Toggled) return;

            if (self.game.manager.menuSetup.startGameCondition == ProcessManager.MenuSetup.StoryGameInitCondition.New)
            {

                WarpPoint.WarpPointData wpData = CreateSpecialWarpData();

                // The meat
                self.saveState.warpPointTargetAfterWarpPointSave = wpData;
                self.saveState.denPosition = wpData.destRoom.ToUpperInvariant();

                // The vegetables
                self.saveState.deathPersistentSaveData.minimumRippleLevel = 1f;
                self.saveState.deathPersistentSaveData.rippleLevel = 1f;
                self.saveState.deathPersistentSaveData.maximumRippleLevel = 1f;

                self.warpsTraversedThisCycle++;
                self.saveState.preserveWarpFatigueAfterWarpPointSave = 0;
                self.saveState.miscWorldSaveData.hasSkippedFirstWarpFatigueTransfer = 1;

                self.saveState.deathPersistentSaveData.reinforcedKarma = KarmaReinforced;

            }
        }

        private static void WarpPoint_NewWorldLoaded_Room(On.Watcher.WarpPoint.orig_NewWorldLoaded_Room orig, WarpPoint self, Room newRoom)
        {

            if (!ModManager.Watcher || newRoom.game.GetStorySession.saveStateNumber != WatcherEnums.SlugcatStatsName.Watcher || !Toggled)
            {
                orig(self, newRoom);
            }
            else
            {
                if (newRoom.abstractRoom.name.ToLowerInvariant() == Regex.Split(Regex.Split(regionToPO[EntryRegion], "><")[3], "~")[4] && // EEEWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
                    newRoom.game.manager.menuSetup.startGameCondition == ProcessManager.MenuSetup.StoryGameInitCondition.New)
                {

                    newRoom.world.game.GetStorySession.pendingSentientRotInfectionFromWarp = SpreadRot;
                    orig(self, newRoom);

                    WarpPoint.WarpPointData wpData = CreateSpecialWarpData();

                    newRoom.world.game.GetStorySession.saveState.warpPointTargetAfterWarpPointSave = wpData;
                    newRoom.world.game.GetStorySession.pendingSentientRotInfectionFromWarp = false;
                    newRoom.world.game.rainWorld.progression.SaveWorldStateAndProgression(false);
                    newRoom.world.game.GetStorySession.saveState.warpPointTargetAfterWarpPointSave = null; // gotta keep this null in memory in case the player dies (ripplmode karma ladder screen)
                    newRoom.world.game.manager.menuSetup.startGameCondition = ProcessManager.MenuSetup.StoryGameInitCondition.Load; // this is so goddamn important I it took me 6 days to find
                }
                else
                {
                    orig(self, newRoom);
                }
            }
        }

        private static WarpPoint.WarpPointData CreateSpecialWarpData()
        {
            string[] POString = Regex.Split(regionToPO[EntryRegion], "><");

            PlacedObject po = new PlacedObject(PlacedObject.Type.None, null);
            po.FromString(POString);
            SpinningTopData stData = po.data as SpinningTopData;

            WarpPoint.WarpPointData warpPointData = new WarpPoint.WarpPointData(null);
            warpPointData.destPos = stData.destPos;
            warpPointData.RegionString = stData.RegionString;
            warpPointData.destRegion = stData.destRegion.ToUpperInvariant();
            warpPointData.sourceTimeline = stData.destTimeline;
            warpPointData.destRoom = stData.destRoom;
            warpPointData.destTimeline = stData.destTimeline;
            warpPointData.panelPos = stData.panelPos;
            warpPointData.deathPersistentWarpPoint = true;
            warpPointData.rippleWarp = stData.rippleWarp;
            warpPointData.oneWay = true;
            if (warpPointData.oneWay)
            {
                warpPointData.oneWayEntrance = true;
                warpPointData.oneWayEntranceIdentified = true;
            }
            warpPointData.cycleSpawnedOn = 0;
            warpPointData.destCam = WarpPoint.GetDestCam(warpPointData);
            warpPointData.uuidPair = "25683a5f-a972-4c6b-99bc-5860fe654b42"; // idk I just picked one. Needs to be constant for some warpdeferid checks

            return warpPointData;
        }
    }
}
