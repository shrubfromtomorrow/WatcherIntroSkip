using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using System.Collections.Generic;
using UnityEngine;


namespace WatcherIntroSkip
{
    public class UI : RectangularMenuObject
    {
        public SimpleButton warp;
        public MenuTabWrapper wrapper;

        public OpComboBox selectedRegion;

        public OpCheckBox karmaReinforced;

        public Configurable<string> regionString;
        public Configurable<bool> karmaReinforcedBool;

        public MenuLabel regionSelectLabel;
        public MenuLabel karmaLabel;

        public List<ListItem> regions = new List<ListItem>();


        private float selectedRegionWidth = 150f;

        public UI(Menu.Menu menu, MenuObject owner, Vector2 pos, Vector2 size) : base(menu, owner, pos, size)
        {
            regionString = new Configurable<string>("Sunbaked Alley");
            karmaReinforcedBool = new Configurable<bool>(false);

            regions.Add(new ListItem("WSKB", "Sunbaked Alley", 0));
            regions.Add(new ListItem("WRFA", "Coral Caves", 1));
            regions.Add(new ListItem("WSKA", "Torrential Railways", 2));

            wrapper = new MenuTabWrapper(menu, this);
            subObjects.Add(wrapper);

            selectedRegion = new OpComboBox(regionString, new Vector2(145f, Plugin.screenDims.y - 135f), selectedRegionWidth, regions);
            var selectedRegionWrapper = new UIelementWrapper(wrapper, selectedRegion);

            regionSelectLabel = new MenuLabel(menu, this, "Select a region:", new Vector2(selectedRegion.GetPos().x - 95f, selectedRegion.GetPos().y + 12f), new Vector2(), false);
            regionSelectLabel.label.alignment = FLabelAlignment.Left;
            subObjects.Add(regionSelectLabel);

            karmaReinforced = new OpCheckBox(karmaReinforcedBool, new Vector2(160f, Plugin.screenDims.y - 100f));
            var karmaReinforcedWrapper = new UIelementWrapper(wrapper, karmaReinforced);

            karmaLabel = new MenuLabel(menu, this, "Reinforced karma:", new Vector2(karmaReinforced.GetPos().x - 110f, karmaReinforced.GetPos().y + 12f), new Vector2(), false);
            karmaLabel.label.alignment = FLabelAlignment.Left;
            subObjects.Add(karmaLabel);

            float thing = (Plugin.screenDims.x + (1366f - Plugin.screenDims.x) * 0.5f) - 180.2f;
            warp = new SimpleButton(menu, this, "WARP", "warp", new Vector2(thing, 55f), new Vector2(110f, 30f));
            subObjects.Add(warp);

        }

        public override void Singal(MenuObject sender, string message)
        {
            base.Singal(sender, message);

            if (message == "warp")
            {
                if (selectedRegion.value != null)
                {
                    Hooks.SpawnWarp(selectedRegion.value, karmaReinforced.value);
                }
            }
        }
    }
}