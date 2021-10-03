using Godot;
using Characters;
using System;
using System.Threading.Tasks;
using System.Linq;

public class CharacterComponentTabs : TabContainer
{
    public Action<CharacterStyleComponentType, string, bool> OnCharacterComponentToggle;

    private bool isLoadKids = false;
    private CharacterFrameSize frameSize = CharacterFrameSize.Small;

    class TabInfo
    {
        public string Name;

        public CharacterComponentTabPanel Panel;

        public CharacterStyleComponentType ComponentType;

        public string CharacterComponentName;

        public bool hasKidCharacter = true;

        public bool Loaded;
    }

    private TabInfo[] tabs = new TabInfo[]
    {
        new TabInfo {
            Name = "身体",
            ComponentType = CharacterStyleComponentType.Body,
            CharacterComponentName = "Bodies"
        },
        new TabInfo {
            Name = "装束",
            ComponentType = CharacterStyleComponentType.Outfit,
            CharacterComponentName = "Outfits"
        },
        new TabInfo {
            Name = "发型",
            ComponentType = CharacterStyleComponentType.Hairstyle,
            CharacterComponentName = "Hairstyles"
        },
        new TabInfo {
            Name = "眼睛",
            ComponentType = CharacterStyleComponentType.Eyes,
            CharacterComponentName = "Eyes"
        },
        new TabInfo {
            Name = "手机",
            ComponentType = CharacterStyleComponentType.Smartphones,
            CharacterComponentName = "Smartphones",
            hasKidCharacter = false
        },
        new TabInfo {
            Name = "配饰",
            ComponentType = CharacterStyleComponentType.Accessories,
            CharacterComponentName = "Accessories",
            hasKidCharacter = false
        }
    };

    public override void _Ready()
    {
        Connect("tab_selected", this, "onTabSelected");
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
            "res://src/tools/character.designer/character_components/CharacterComponentTabPanel.tscn");
        foreach (TabInfo tab in tabs)
        {
            tab.Panel = TabPanelPackedScene.Instance<CharacterComponentTabPanel>();
            tab.Panel.Name = tab.Name;
            AddChild(tab.Panel);
        }

        onTabSelected(0);
    }

    public void ClearSelected()
    {
        tabs.ToList().ForEach(tab =>
        {
            foreach (var item in tab.Panel.GetItems())
                (item as CharacterSelectItem).Selected = false;
        });
    }

    private void onTabSelected(int tabIndex)
    {
        var tab = tabs[tabIndex];
        if (tab.Loaded)
            return;
        new Task(() =>
        {
            loadCharacterComponents(tab);
        }).Start();
        tab.Loaded = true;
    }

    private void loadCharacterComponents(TabInfo tab)
    {
        var sizeName = CharacterStyleGenerator.GetFrameSizeName(frameSize);
        var suffix = isLoadKids && tab.hasKidCharacter ? "_kids" : "";
        var dirPath = $"{OS.GetUserDataDir()}/res/character/{tab.CharacterComponentName}{suffix}/{sizeName}";
        string[] filePaths = System.IO.Directory.GetFiles(dirPath, "*.png");

        tab.Panel.Clear();
        foreach (string filePath in filePaths)
        {
            var item = CharacterSelectItem.Instance();
            item.LoadFrom($"{filePath}");

            item.onSelect = (selected) =>
            {
                foreach (var it in tab.Panel.GetItems())
                {
                    if (item != it)
                        (it as CharacterSelectItem).Selected = false;
                }
                OnCharacterComponentToggle.Invoke(tab.ComponentType, item.filePath, selected);
            };
            tab.Panel.AddItem(item);
        }
    }
}
