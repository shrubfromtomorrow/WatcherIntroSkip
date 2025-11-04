using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using System.Collections.Generic;
using UnityEngine;


namespace WatcherIntroSkip
{
    public class UI : RectangularMenuObject
    {
        public SimpleButton warp;
        public MenuTabWrapper wrapper;

        public OpComboBox selectedRegion;

        //public OpComboBox stomachItem;

        public OpCheckBox karmaReinforced;

        public OpCheckBox rotSpread;

        public Configurable<string> regionString;
        //public Configurable<string> stomachItemString;
        public Configurable<bool> karmaReinforcedBool;
        public Configurable<bool> rotSpreadBool;

        public MenuLabel regionSelectLabel;
        //public MenuLabel stomachItemLabel;
        public MenuLabel karmaLabel;
        public MenuLabel rotLabel;

        public List<ListItem> regions;
        //public List<ListItem> stomachItems;


        private float selectedRegionWidth = 150f;

        private Vector2 anchor;

        public UI(Menu.Menu menu, MenuObject owner, Vector2 pos, Vector2 size) : base(menu, owner, pos, size)
        {
            anchor = new Vector2(145f, Plugin.screenDims.y - Plugin.screenDims.y * 0.825f);

            regionString = new Configurable<string>("Sunbaked Alley");
            //stomachItemString = new Configurable<string>("None");
            karmaReinforcedBool = new Configurable<bool>(true);
            rotSpreadBool = new Configurable<bool>(true);

            regions = new List<ListItem>();
            //stomachItems = new List<ListItem>();

            regions.Add(new ListItem("WSKB", "Sunbaked Alley", 0));
            regions.Add(new ListItem("WRFA", "Coral Caves", 1));
            regions.Add(new ListItem("WSKA", "Torrential Railways", 2));

            //stomachItems.Add(new ListItem("Rock", "Rock", 0));
            //stomachItems.Add(new ListItem("DataPearl", "Pearl", 1));
            //stomachItems.Add(new ListItem("PuffBall", "Spore Puff", 2));
            //stomachItems.Add(new ListItem("ScavengerBomb", "Bomb", 3));
            //stomachItems.Add(new ListItem("FlareBomb", "Flashbang", 4));
            //stomachItems.Add(new ListItem("FirecrackerPlant", "Cherry Bomb", 5));

            wrapper = new MenuTabWrapper(menu, this);
            subObjects.Add(wrapper);


            selectedRegion = new OpComboBox(regionString, anchor, selectedRegionWidth, regions);
            var selectedRegionWrapper = new UIelementWrapper(wrapper, selectedRegion);

            regionSelectLabel = new MenuLabel(menu, this, "Select a region:", new Vector2(selectedRegion.GetPos().x - 9f, selectedRegion.GetPos().y + 12f), new Vector2(), false);
            regionSelectLabel.label.alignment = FLabelAlignment.Left;
            subObjects.Add(regionSelectLabel);


            //stomachItem = new OpComboBox(stomachItemString, anchor + new Vector2(310f, 0f), selectedRegionWidth - 25f, stomachItems);
            //var stomachItemWrapper = new UIelementWrapper(wrapper, stomachItem);

            //stomachItemLabel = new MenuLabel(menu, this, "Select a stomach item:", new Vector2(stomachItem.GetPos().x - 136f, stomachItem.GetPos().y + 12f), new Vector2(), false);
            //stomachItemLabel.label.alignment = FLabelAlignment.Left;
            //subObjects.Add(stomachItemLabel);


            karmaReinforced = new OpCheckBox(karmaReinforcedBool, anchor + new Vector2(0f, 28f));
            var karmaReinforcedWrapper = new UIelementWrapper(wrapper, karmaReinforced);

            karmaLabel = new MenuLabel(menu, this, "Reinforced karma:", new Vector2(karmaReinforced.GetPos().x - 110f, karmaReinforced.GetPos().y + 12f), new Vector2(), false);
            karmaLabel.label.alignment = FLabelAlignment.Left;
            subObjects.Add(karmaLabel);


            rotSpread = new OpCheckBox(rotSpreadBool, anchor + new Vector2(0f, 56f));
            var rotSpreaddWrapper = new UIelementWrapper(wrapper, rotSpread);

            rotLabel = new MenuLabel(menu, this, "Spread rot:", new Vector2(rotSpread.GetPos().x - 110f, rotSpread.GetPos().y + 12f), new Vector2(), false);
            rotLabel.label.alignment = FLabelAlignment.Left;
            subObjects.Add(rotLabel);


            float thing = (Plugin.screenDims.x + (1366f - Plugin.screenDims.x) * 0.5f - 320.2f);
            warp = new SimpleButton(menu, this, "WARP", "warp", new Vector2(thing, 55f), new Vector2(110f, 30f));
            subObjects.Add(warp);
        }

        public override void Update()
        {
            base.Update();
            if (selectedRegion.value == "WRFA")
            {
                rotSpreadBool.Value = true;
                rotSpread.SetValueBool(true);
                rotSpread.greyedOut = true;
            }
            else
            {
                rotSpread.greyedOut = false;
            }
        }

        public override void Singal(MenuObject sender, string message)
        {
            base.Singal(sender, message);

            if (message == "warp")
            {
                if (selectedRegion.value != null)
                {
                    Hooks.SpawnWarp(selectedRegion.value, karmaReinforced.value, rotSpread.value);
                }
            }
        }
    }
}

//LF last is absolutely certain to result in the first region being rotted, no way around it
//CC last is technically possible to result in being unrotted, but tbh I think it could be worth testing practically to see if there's any way to do that
//SH last is a coin flip and definitely needs a toggle