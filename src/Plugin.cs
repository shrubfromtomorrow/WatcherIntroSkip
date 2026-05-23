using System;
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
        public const string PLUGIN_VERSION = "1.1";
        public const string PLUGIN_NAME = "Watcher Intro Skip";
        public const string PLUGIN_GUID = "shrubfromtomorrow.watcherIntroSkip";
        internal static new ManualLogSource Logger;
        public static Plugin Instance;
        public Config options;
        public static bool init;

        public void OnEnable()
        {
            Instance = this;
            Logger = base.Logger;
            options = new Config();
            On.RainWorld.OnModsInit += OnModsInit;
        }

        public void OnDisable()
        {
            Logger = null;
            Instance = null;
            init = false;
            On.RainWorld.OnModsInit -= OnModsInit;
        }

        public void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld game)
        {
            orig(game);
            if (!init)
            {
                init = true;
                Hooks.Apply();
            }
            MachineConnector.SetRegisteredOI("shrubfromtomorrow.watcherIntroSkip", options);
        }
    }
}
