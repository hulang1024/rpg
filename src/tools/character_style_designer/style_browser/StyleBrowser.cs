using System;
using Godot;
using Characters;

namespace Tools.CharacterStyleDesigner
{
    public class StyleBrowser : MarginContainer
    {
        public Action<CharacterStyleInfo> OnSelect;

        private CharacterStyleManager styleManager = new CharacterStyleManager();
        private CharacterStyleSearchParam searchParam = new CharacterStyleSearchParam();
        private CharacterStyleInfo lastMenuTargetItem;

        private GridContainer gridContainer;
        private PopupMenu menu;
        private WindowDialog styleInfoDialog;

        public override void _Ready()
        {
            var searchBar = GetNode<Control>("VBoxContainer/SearchBar");
            var genderRadioGroup = searchBar.GetNode<RadioGroup>("GenderRadioGroup");
            genderRadioGroup.OnChanged += (gender) =>
            {
                if (gender == "All")
                    searchParam.Gender = null;
                else
                    searchParam.Gender = (Gender)Enum.Parse(typeof(Gender), gender);
                Search();
            };

            var isKidRadioGroup = searchBar.GetNode<RadioGroup>("IsKidRadioGroup");
            isKidRadioGroup.OnChanged += (type) =>
            {
                if (type == "All")
                    searchParam.IsKid = null;
                else
                    searchParam.IsKid = type == "Y";
                Search();
            };

            gridContainer = GetNode<GridContainer>("VBoxContainer/ScrollContainer/GridContainer");
            menu = GetNode<PopupMenu>("PopupMenu");
            styleInfoDialog = GetNode<WindowDialog>("StyleInfoDialog");
        }

        public async void Search()
        {
            foreach (Node it in gridContainer.GetChildren())
            {
                gridContainer.RemoveChild(it);
            }
            foreach (CharacterStyleInfo styleInfo in styleManager.Search(searchParam))
            {
                var item = StyleSelectItem.Instance();
                item.LoadFrom(await CharacterStyleImageGenerator.GenerateImageTexture(styleInfo.Styles), null);
                item.OnSelect += (selected) =>
                {
                    foreach (var it in gridContainer.GetChildren())
                    {
                        if (item != it)
                            (it as StyleSelectItem).Selected = false;
                    }
                    OnSelect.Invoke(styleInfo);
                };
                item.OnRequestPopupMenu += (position) =>
                {
                    lastMenuTargetItem = styleInfo;
                    menu.SetPosition(position);
                    menu.Visible = true;
                };
                gridContainer.AddChild(item);
            }
        }

        private void _on_TagsLineEdit_text_changed(string text)
        {
            searchParam.Tags = text;
        }

        private void _on_SearchButton_pressed()
        {
            Search();
        }

        private void _on_PopupMenu_popup_hide()
        {
            lastMenuTargetItem = null;
        }

        private void _on_PopupMenu_id_pressed(int id)
        {
            switch (id)
            {
                case 0:
                    menu.Visible = false;
                    break;
                case 1:
                    OnOpenStyleInfo();
                    break;
                case 2:
                    OnDeleteStyleInfo();
                    break;
            }
        }

        public void OnOpenStyleInfo()
        {
            styleInfoDialog.SetPosition(menu.RectPosition);
            var form = styleInfoDialog.GetNode<Control>("MarginContainer/VBoxContainer");
            form.GetNode<LineEdit>("IdItem/LineEdit").Text = lastMenuTargetItem.Id;
            form.GetNode<Label>("IsKidItem/IsKid").Text = lastMenuTargetItem.Styles.IsKid ? "是" : "否";
            form.GetNode<Label>("GenderItem/Gender").Text =
                lastMenuTargetItem.Gender == Gender.Male ? "男" : "女";
            form.GetNode<TextEdit>("TagsItem/TextEdit").Text = lastMenuTargetItem.Tags;
            form.GetNode<TextEdit>("RemarkItem/TextEdit").Text = lastMenuTargetItem.Remark;

            styleInfoDialog.Visible = true;
        }

        private void OnDeleteStyleInfo()
        {
            var result = styleManager.Delete(lastMenuTargetItem.Id);
            if (result == 0)
            {
                Search();
            }
        }
    }
}
