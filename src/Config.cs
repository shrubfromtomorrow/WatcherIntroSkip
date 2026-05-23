using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace WatcherIntroSkip
{
    public class Config : OptionInterface
    {
        private OpTab mainTab;
        private UIelement[] mainTabOptions;

        public readonly Configurable<bool> WatcherIntroSkip;
        public readonly Configurable<string> WISRegionString;
        public List<ListItem> WISRegionList;
        public readonly Configurable<bool> WISReinforcedKarma;
        public readonly Configurable<bool> WISSpreadRot;

        public Config()
        {
            WatcherIntroSkip = config.Bind<bool>("WatcherIntroSkip", true);
            WISRegionString = config.Bind<string>("WISRegionString", "Sunbaked Alley");
            WISReinforcedKarma = config.Bind<bool>("WISReinforcedKarma", true);
            WISSpreadRot = config.Bind<bool>("WISSpreadRot", true);

            WISRegionList = new List<ListItem>
            {
                new ListItem("Sunbaked Alley", "Sunbaked Alley", 0),
                new ListItem("Coral Caves", "Coral Caves", 1),
                new ListItem("Torrential Railways", "Torrential Railways", 2)
            };
        }

        public override void Initialize()
        {
            base.Initialize();

            mainTab = new OpTab(this, "Main");

            Tabs = new[] { mainTab };

            mainTabOptions = new UIelement[]
            {
                 new OpCheckBox(WatcherIntroSkip, 5f, 527f) { description = "Beginning Watcher's campaign will start Watcher in the selected starting region with the selected options"},
                new OpLabel(37f, 530f, "Watcher Intro Skip") {alignment = FLabelAlignment.Left, description = "Beginning Watcher's campaign will start Watcher in the selected starting region with the selected options", color = RainWorld.RippleColor},
                new OpComboBox(WISRegionString, new Vector2(153f, 527f), 150f, WISRegionList) { description = "Starting region", colorEdge = RainWorld.RippleColor},

                new OpCheckBox(WISReinforcedKarma, 5f, 492f) { description = "The Watcher starts their campaign with reinforced karma (karma flower effect)"},
                new OpLabel(37f, 495f, "Reinforced karma") {alignment = FLabelAlignment.Left, description = "The Watcher starts their campaign with reinforced karma (karma flower effect)", color = RainWorld.RippleColor},

                new OpCheckBox(WISSpreadRot, 5f, 457f) { description = "The Watcher starts spreads rot to starting region (forced for Coral Caves to match game behavior)"},
                new OpLabel(37f, 460f, "Spread rot") {alignment = FLabelAlignment.Left, description = "The Watcher starts spreads rot to starting region (forced for Coral Caves to match game behavior)", color = RainWorld.RippleColor},
            };

            mainTab.AddItems(mainTabOptions);
        }

        public override void Update()
        {
            base.Update();

            bool WISValue = false;

            bool WISRegionCoral = false;

            foreach (var item in Tabs[0].items)
            {
                if (item is OpCheckBox b)
                {
                    if (b.cfgEntry == WatcherIntroSkip) WISValue = b.GetValueBool();
                }
                if (item is OpComboBox b2)
                {
                    if (b2.cfgEntry == WISRegionString)
                    {
                        if (b2.value == "Coral Caves")
                        {
                            WISRegionCoral = true;
                        }
                        else WISRegionCoral = false;
                    }
                }
            }

            foreach (var item in Tabs[0].items)
            {
                if (item is OpComboBox c && c.cfgEntry == WISRegionString)
                {
                    c.greyedOut = !WISValue;
                }
                if (item is OpCheckBox b && (b.cfgEntry == WISReinforcedKarma || b.cfgEntry == WISSpreadRot))
                {
                    b.greyedOut = !WISValue;
                }
                if (item is OpCheckBox b2 && b2.cfgEntry == WISSpreadRot && WISRegionCoral)
                {
                    if (WISRegionCoral)
                    {
                        b2.SetValueBool(true);
                        b2.greyedOut = true;
                    }
                    else b2.greyedOut = !WISValue;
                }
            }
        }
    }
}
