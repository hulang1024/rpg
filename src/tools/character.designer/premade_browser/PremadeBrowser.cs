using System;
using Godot;
using Characters;

public class PremadeBrowser : MarginContainer
{
    public Action<PremadeCharacterInfo> OnSelect;

    private PremadeCharacterManager premadeCharacterManager = new PremadeCharacterManager();
    private PremadeSearchParam searchParam = new PremadeSearchParam();
    private PremadeCharacterInfo lastMenuTargetPremade;

    private GridContainer gridContainer;
    private PopupMenu menu;
    private WindowDialog premadeInfoDialog;

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
        premadeInfoDialog = GetNode<WindowDialog>("PremadeInfoDialog");
    }

    public async void Search()
    {
        foreach (Node it in gridContainer.GetChildren())
        {
            gridContainer.RemoveChild(it);
        }
        foreach (PremadeCharacterInfo premade in premadeCharacterManager.Search(searchParam))
        {
            var item = CharacterSelectItem.Instance();
            item.LoadFrom(await CharacterStyleGenerator.GenerateImageTexture(premade.Styles), null);
            item.onSelect += (selected) =>
            {
                foreach (var it in gridContainer.GetChildren())
                {
                    if (item != it)
                        (it as CharacterSelectItem).Selected = false;
                }
                OnSelect.Invoke(premade);
            };
            item.OnRequestPopupMenu += (position) =>
            {
                lastMenuTargetPremade = premade;
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
        lastMenuTargetPremade = null;
    }

    private void _on_PopupMenu_id_pressed(int id)
    {
        switch (id)
        {
            case 0:
                menu.Visible = false;
                break;
            case 1:
                onOpenPremadeInfo();
                break;
            case 2:
                onDeletePremade();
                break;
        }
    }

    public void onOpenPremadeInfo()
    {
        premadeInfoDialog.SetPosition(menu.RectPosition);
        var form = premadeInfoDialog.GetNode<Control>("MarginContainer/VBoxContainer");
        form.GetNode<LineEdit>("IdItem/LineEdit").Text = lastMenuTargetPremade.Id;
        form.GetNode<Label>("IsKidItem/IsKid").Text = lastMenuTargetPremade.Styles.IsKid ? "是" : "否";
        form.GetNode<Label>("GenderItem/Gender").Text =
            lastMenuTargetPremade.Gender == Gender.Male ? "男" : "女";
        form.GetNode<TextEdit>("TagsItem/TextEdit").Text = lastMenuTargetPremade.Tags;
        form.GetNode<TextEdit>("RemarkItem/TextEdit").Text = lastMenuTargetPremade.Remark;

        premadeInfoDialog.Visible = true;
    }

    private void onDeletePremade()
    {
        var result = premadeCharacterManager.Delete(lastMenuTargetPremade.Id);
        if (result == 0)
        {
            Search();
        }
    }
}
