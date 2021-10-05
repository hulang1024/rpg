using Godot;
using Characters;
using System;
using System.Collections.Generic;
using CharacterState = Characters.State;

namespace Tools.CharacterStyleDesigner
{
    public class StyleDesigner : Control
    {
        private List<Character> previewCharacters = new List<Character>();
        private CharacterStyleManager styleManager = new CharacterStyleManager();

        private StyleInfoForm styleInfoForm;

        private TabContainer mainTabs;
        private StyleComponentTabs styleComponentTabs;
        private StyleBrowser styleBrowser;

        public override void _Ready()
        {
            styleInfoForm = GetNode<StyleInfoForm>("MarginContainer/HBoxContainer/Left/StyleInfoForm");
            styleInfoForm.OnNew += onNewStyle;
            styleInfoForm.OnSave += onSaveStyle;
            styleInfoForm.OnDelete += onDeleteStyle;
            styleInfoForm.OnModelChanged += onStyleChanged;
            styleInfoForm.OnStyleDepsChanged += onStyleDepsChanged;

            mainTabs = GetNode<TabContainer>("MarginContainer/HBoxContainer/TabContainer");
        
            styleComponentTabs = mainTabs.GetNode<StyleComponentTabs>("StyleComponentTabs");
            styleComponentTabs.Name = "角色样式设计";
            styleComponentTabs.OnStyleComponentToggle = onStyleComponentToggle;
            styleComponentTabs.Load(false, CharacterFrameSize.Small);

            styleBrowser = mainTabs.GetNode<StyleBrowser>("StyleBrowser");
            styleBrowser.Name = "预制角色样式库";
            styleBrowser.OnSelect = onStyleBrowserItemSelect;

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
                if (styleInfoForm.StyleInfo.Styles.IsKid)
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
            previewCharacters.ForEach(ch => ch.UpdateCharacterStyle(styleInfoForm.StyleInfo.Styles));
        }

        private void onStyleBrowserItemSelect(CharacterStyleInfo styleInfo)
        {
            styleInfoForm.StyleInfo = styleInfo;
        }

        private void onStyleChanged()
        {
            updatePreviewCharacterStyle();
        }

        private void onStyleDepsChanged()
        {
            var styles = styleInfoForm.StyleInfo.Styles;
            createPreviewCharacters();
            updatePreviewCharacterStyle();
            styleComponentTabs.Load(styles.IsKid, styles.FrameSize, true);
        }

        private void onStyleComponentToggle(CharacterStyleComponentType type, string filePath, bool selected)
        {
            string typeName = null;
            if (selected)
            {
                var filePathParts = filePath.Split("/");
                var itemFileName = filePathParts[filePathParts.Length - 1];
                typeName = itemFileName.Substring(0, itemFileName.Length - 4);
            }
            var styles = styleInfoForm.StyleInfo.Styles;
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
            styleInfoForm.Message.Text = "";
        }

        private void onNewStyle()
        {
            styleComponentTabs.ClearSelected();
            mainTabs.CurrentTab = 0;
        }

        private void onSaveStyle(CharacterStyleInfo styleInfo, string styleId)
        {
            var isAdd = styleId == null;
            
            var result = isAdd
                ? styleManager.Add(styleInfo)
                : styleManager.Update(styleId, styleInfo);
            switch (result)
            {
                case 0:
                    styleInfoForm.Message.Text = isAdd ? "增加成功！" : "修改成功！";
                    styleInfoForm.StyleId = styleInfo.Id;
                    styleBrowser.Search();
                    break;
                case 1:
                    styleInfoForm.Message.Text = "保存失败！";
                    break;
                case 2:
                    styleInfoForm.Message.Text = "此角色配置已存在！";
                    break;
                case 3:
                    styleInfoForm.Message.Text = "ID重复！";
                    break;
                case 4:
                    styleInfoForm.Message.Text = "ID不能包含逗号！";
                    break;
            }
        }

        private void onDeleteStyle(string styleId)
        {
            var result = styleManager.Delete(styleId);
            if (result == 0)
            {
                styleInfoForm.Message.Text = "删除成功！";
                styleInfoForm.StyleId = null;
                styleInfoForm.StyleInfo.OldIds = null;
                styleBrowser.Search();
            }
            else
            {
                styleInfoForm.Message.Text = "删除失败！";
            }
        }

        private void _on_TabContainer_tab_selected(int tabIndex)
        {
            if (tabIndex == 1)
            {
                styleBrowser.Search();
            }
        }
    }
}