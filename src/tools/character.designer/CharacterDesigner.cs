using Godot;
using Characters;
using System;
using System.Collections.Generic;
using CharacterState = Characters.State;

public class CharacterDesigner : Control
{
    private List<Character> previewCharacters = new List<Character>();
    private PremadeCharacterManager premadeCharacterManager = new PremadeCharacterManager();

    private PremadeForm premadeForm;

    private TabContainer mainTabs;
    private CharacterComponentTabs characterComponentTabs;
    private PremadeBrowser premadeBrowser;

    public override void _Ready()
    {
        premadeForm = GetNode<PremadeForm>("MarginContainer/HBoxContainer/Left/PremadeForm");
        premadeForm.OnNew += onNewPremade;
        premadeForm.OnSave += onSavePremade;
        premadeForm.OnDelete += onDeletePremade;
        premadeForm.OnPremadeChanged += onPremadeChanged;
        premadeForm.OnStyleDepsChanged += onStyleDepsChanged;

        mainTabs = GetNode<TabContainer>("MarginContainer/HBoxContainer/TabContainer");
    
        characterComponentTabs = mainTabs.GetNode<CharacterComponentTabs>("CharacterComponentTabs");
        characterComponentTabs.Name = "角色设计";
        characterComponentTabs.OnCharacterComponentToggle = onCharacterComponentToggle;
        characterComponentTabs.Load(false, CharacterFrameSize.Small);

        premadeBrowser = mainTabs.GetNode<PremadeBrowser>("PremadeBrowser");
        premadeBrowser.Name = "预制角色库";
        premadeBrowser.OnSelect = onPremadeBrowserItemSelect;

        createPreviewCharacters();
    }

    private void createPreviewCharacters()
    {
        previewCharacters.Clear();

        var container = GetNode<Control>(
            "MarginContainer/HBoxContainer/Left/ScrollContainer/MarginContainer/CharacterView");

        foreach (Node child in container.GetChildren())
            container.RemoveChild(child);

        var kidStates = new CharacterState[]
        {
            CharacterState.Idle, CharacterState.Walk, CharacterState.Sleep,
        };
        foreach (CharacterState state in Enum.GetValues(typeof(CharacterState)))
        {
            if (premadeForm.Premade.Styles.IsKid)
            {
                if (!Array.Exists(kidStates, ks => ks == state))
                    continue;
            }
            else if (state == CharacterState.Sleep)
            {
                continue;
            }

            var group = new VBoxContainer();
            int x = 20;
            foreach (Characters.Dir dir in Enum.GetValues(typeof(Characters.Dir)))
            {
                var character = CharacterGenerator.Generate();
                character.Preview = true;
                character.InitialState = state;
                character.InitialDir = dir;
                character.Set("position", new Vector2(x, 0));
                x += 40;
                if (state != CharacterState.Sit
                    || (state == CharacterState.Sit && (dir == Dir.Left || dir == Dir.Right)))
                {
                    group.AddChild(character);
                    previewCharacters.Add(character);
                }
            }
            container.AddChild(group);
        }
    }

    private void updatePreviewCharacterStyle()
    {
        previewCharacters.ForEach(ch => ch.UpdateCharacterStyle(premadeForm.Premade.Styles));
    }

    private void onPremadeBrowserItemSelect(PremadeCharacterInfo premade)
    {
        premadeForm.Premade = premade;
    }

    private void onPremadeChanged()
    {
        updatePreviewCharacterStyle();
    }

    private void onStyleDepsChanged()
    {
        var styles = premadeForm.Premade.Styles;
        createPreviewCharacters();
        updatePreviewCharacterStyle();
        characterComponentTabs.Load(styles.IsKid, styles.FrameSize, true);
    }

    private void onCharacterComponentToggle(CharacterStyleComponentType type, string filePath, bool selected)
    {
        string typeName = null;
        if (selected)
        {
            var filePathParts = filePath.Split("/");
            var itemFileName = filePathParts[filePathParts.Length - 1];
            typeName = itemFileName.Substring(0, itemFileName.Length - 4);
        }
        var styles = premadeForm.Premade.Styles;
        switch (type)
        {
            case CharacterStyleComponentType.Body:
                styles.Body = typeName;
                break;
            case CharacterStyleComponentType.Outfit:
                styles.Outfit = typeName;
                break;
            case CharacterStyleComponentType.Hairstyle:
                styles.Hairstyle = typeName;
                break;
            case CharacterStyleComponentType.Eyes:
                styles.Eyes = typeName;
                break;
            case CharacterStyleComponentType.Smartphones:
                styles.Smartphones = typeName;
                break;
            case CharacterStyleComponentType.Accessories:
                styles.Accessories = typeName;
                break;
        }
        updatePreviewCharacterStyle();
        premadeForm.Message.Text = "";
    }

    private void onNewPremade()
    {
        characterComponentTabs.ClearSelected();
        mainTabs.CurrentTab = 0;
    }

    private void onSavePremade(PremadeCharacterInfo premade, string premadeId)
    {
        var isAdd = premadeId == null;
        
        var result = isAdd
            ? premadeCharacterManager.Add(premade)
            : premadeCharacterManager.Update(premadeId, premade);
        switch (result)
        {
            case 0:
                premadeForm.Message.Text = isAdd ? "增加成功！" : "修改成功！";
                premadeForm.PremadeId = premade.Id;
                premadeBrowser.Search();
                break;
            case 1:
                premadeForm.Message.Text = "保存失败！";
                break;
            case 2:
                premadeForm.Message.Text = "此角色配置已存在！";
                break;
            case 3:
                premadeForm.Message.Text = "ID重复！";
                break;
            case 4:
                premadeForm.Message.Text = "ID不能包含逗号！";
                break;
        }
    }

    private void onDeletePremade(string premadeId)
    {
        var result = premadeCharacterManager.Delete(premadeId);
        if (result == 0)
        {
            premadeForm.Message.Text = "删除成功！";
            premadeForm.PremadeId = null;
            premadeForm.Premade.OldIds = null;
            premadeBrowser.Search();
        }
        else
        {
            premadeForm.Message.Text = "删除失败！";
        }
    }

    private void _on_TabContainer_tab_selected(int tabIndex)
    {
        if (tabIndex == 1)
        {
            premadeBrowser.Search();
        }
    }
}
