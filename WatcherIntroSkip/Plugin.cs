using System;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Logging;
using Menu;
using UnityEngine;

#pragma warning disable CS0618
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace WatcherIntroSkip
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_VERSION = "1.05";
        public const string PLUGIN_NAME = "Watcher Intro Skip";
        public const string PLUGIN_GUID = "shrubfromtomorrow.watcherIntroSkip";
        internal static ManualLogSource logger;
        public static Plugin instance;
        public static bool init;

        public static readonly ProcessManager.ProcessID RegionMenuID = new ProcessManager.ProcessID("RegionMenu", true);
        public static readonly MenuScene.SceneID MainRegionSelect = new MenuScene.SceneID("MainRegionSelect", false);
        public static String CCWarp = "SpinningTopSpot><933.9406><2132.439><18~77~Watcher~WSKB~wskb_c17~490~510~15";
        public static String LFWarp = "SpinningTopSpot><591.3841><1316.087><37~40~Watcher~WRFA~wrfa_sk04~550~450~16";
        public static String SHWarp = "SpinningTopSpot><647.808><357.6914><44~106~Watcher~WSKA~wska_d27~470~410~17";

        public static string titleFont;
        public static string mainFont;

        public static Vector2 screenDims;

        public static UI container;

        public static RainWorldGame game;

        public bool warped = false;

        public void OnEnable()
        {
            instance = this;
            logger = Logger;
            On.RainWorld.OnModsInit += OnModsInit;
        }

        public void OnDisable()
        {
            logger = null;
            instance = null;
            init = false;
        }

        public static void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld game)
        {
            orig(game);
            if (!init)
            {
                init = true;
                Hooks.Apply();
                screenDims = RWCustom.Custom.rainWorld.options.ScreenSize;
            }
        }

        public void Update()
        {
            if (Plugin.instance.warped) return;

            var player = Plugin.game?.Players.FirstOrDefault();
            if (player == null) return;

            var room = player.Room;

            if (room != null)
            {
                foreach (var shortcut in room.realizedRoom.shortcutsIndex)
                {
                    room.realizedRoom.lockedShortcuts.Add(shortcut);
                }
            }
        }
    }
}
