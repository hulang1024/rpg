using Godot;
using Characters;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Tools.CharacterStyleDesigner
{
    public class StyleComponentTabs : TabContainer
    {
        public Action<CharacterStyleComponentType, string, bool> OnStyleComponentToggle;

        private bool isLoadKids = false;
        private CharacterFrameSize frameSize = CharacterFrameSize.Small;

        class TabInfo
        {
            public string Name;
            public StyleComponentTabPanel Panel;
            public CharacterStyleComponentType ComponentType;
            public string ComponentName;
            public bool HasKidCharacter = true;
            public bool Loaded;
        }

        private TabInfo[] tabs = new TabInfo[]
        {
            new TabInfo {
                Name = "身体",
                ComponentType = CharacterStyleComponentType.Body,
                ComponentName = "Bodies"
            },
            new TabInfo {
                Name = "装束",
                ComponentType = CharacterStyleComponentType.Outfit,
                ComponentName = "Outfits"
            },
            new TabInfo {
                Name = "发型",
                ComponentType = CharacterStyleComponentType.Hairstyle,
                ComponentName = "Hairstyles"
            },
            new TabInfo {
                Name = "眼睛",
                ComponentType = CharacterStyleComponentType.Eyes,
                ComponentName = "Eyes"
            },
            new TabInfo {
                Name = "手机",
                ComponentType = CharacterStyleComponentType.Smartphones,
                ComponentName = "Smartphones",
                HasKidCharacter = false
            },
            new TabInfo {
                Name = "配饰",
                ComponentType = CharacterStyleComponentType.Accessories,
                ComponentName = "Accessories",
                HasKidCharacter = false
            }
        };

        public override void _Ready()
        {
            Connect("tab_selected", this, "OnTabSelected");
        }

        public void Load(bool isKid, CharacterFrameSize frameSize, bool isReload = false)
        {
            this.isLoadKids = isKid;
            this.frameSize = frameSize;

            if (isReload)
            {
                tabs.ToList().ForEach(tab =>
                {
                    tab.Loaded = false;
                });
                onTabSelected(CurrentTab);
                return;
            }

            var TabPanelPackedScene = GD.Load<PackedScene>(
                "res://src/tools/character_style_designer/style_components/StyleComponentTabPanel.tscn");
            foreach (TabInfo tab in tabs)
            {
                tab.Panel = TabPanelPackedScene.Instance<StyleComponentTabPanel>();
                tab.Panel.Name = tab.Name;
                AddChild(tab.Panel);
            }

            OnTabSelected(0);
        }

        public void ClearSelected()
        {
            tabs.ToList().ForEach(tab =>
            {
                foreach (var item in tab.Panel.GetItems())
                    (item as StyleSelectItem).Selected = false;
            });
        }

        private void OnTabSelected(int tabIndex)
        {
            var tab = tabs[tabIndex];
            if (tab.Loaded)
                return;
            new Task(() =>
            {
                LoadStyleComponents(tab);
            }).Start();
            tab.Loaded = true;
        }

        private void LoadStyleComponents(TabInfo tab)
        {
            var sizeName = CharacterStyleImageGenerator.GetFrameSizeName(frameSize);
            var suffix = isLoadKids && tab.HasKidCharacter ? "_kids" : "";
            var dirPath = $"{OS.GetUserDataDir()}/res/character/{tab.ComponentName}{suffix}/{sizeName}";
            string[] filePaths = System.IO.Directory.GetFiles(dirPath, "*.png");

            tab.Panel.Clear();
            foreach (string filePath in filePaths)
            {
                var item = StyleSelectItem.Instance();
                item.LoadFrom($"{filePath}", tab.ComponentType);

                item.OnSelect = (selected) =>
                {
                    foreach (var it in tab.Panel.GetItems())
                    {
                        if (item != it)
                            (it as StyleSelectItem).Selected = false;
                    }
                    OnStyleComponentToggle.Invoke(tab.ComponentType, item.filePath, selected);
                };
                tab.Panel.AddItem(item);
            }
        }
    }
}