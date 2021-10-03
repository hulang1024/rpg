using Godot;
using System;
using Characters;

public class PremadeForm : Panel
{
    private const string form_path = "MarginContainer/VBoxContainer";

    public Action OnNew;
    public Action<PremadeCharacterInfo, string> OnSave;
    public Action<string> OnDelete;
    public Action OnPremadeChanged;
    public Action OnStyleDepsChanged;
    

    private PremadeCharacterInfo premade = new PremadeCharacterInfo();
    public PremadeCharacterInfo Premade
    {
        get { return premade; }
        set
        {
            premade = value;
            PremadeId = premade.Id;
            OnPremadeChanged.Invoke();
            SyncFormItems();
        }
    }

    public string PremadeId;

    public Label Message;
    private LineEdit idLineEdit;
    private CheckBox isKidCheckbox;
    private RadioGroup genderRadioGroup;
    private TextEdit tagsLineEdit;
    private TextEdit remarkTextEdit;

    public override void _Ready()
    {
        idLineEdit = GetNode<LineEdit>($"{form_path}/ItemId/LineEdit");
        isKidCheckbox = GetNode<CheckBox>($"{form_path}/ItemKid/CheckBox");
        genderRadioGroup = GetNode<RadioGroup>($"{form_path}/ItemGender/RadioGroup");
        tagsLineEdit = GetNode<TextEdit>($"{form_path}/ItemTags/TextEdit");
        remarkTextEdit = GetNode<TextEdit>($"{form_path}/ItemRemark/TextEdit");
        Message = GetNode<Label>($"{form_path}/Message");
    }

    private void SyncFormItems()
    {
        idLineEdit.Text = premade.Id;
        isKidCheckbox.Pressed = premade.Styles.IsKid;
        genderRadioGroup.Value = Enum.GetName(typeof(Gender), premade.Gender);
        tagsLineEdit.Text = premade.Tags;
        remarkTextEdit.Text = premade.Remark;
        Message.Text = "";
    }

    private void _on_IsKidCheckBox_toggled(bool pressed)
    {
        premade.Styles = new CharacterStyleSpecifics(pressed);
        premade.Styles.IsKid = pressed;
        OnStyleDepsChanged.Invoke();
    }

    private void _on_FrameSizeOptionButton_item_selected(int index)
    {
        var frameSize = new CharacterFrameSize[]
        {
            CharacterFrameSize.Medium, CharacterFrameSize.Small, CharacterFrameSize.Mini
        }[index];
        premade.Styles = new CharacterStyleSpecifics(premade.Styles.IsKid);
        premade.Styles.FrameSize = frameSize;
        OnStyleDepsChanged.Invoke();
    }

    private void _on_NewButton_pressed()
    {
        Premade = new PremadeCharacterInfo();
        OnNew.Invoke();
    }

    private void _on_SaveButton_pressed()
    {
        premade.Id = idLineEdit.Text.Trim();
        premade.Gender = (Gender)Enum.Parse(typeof(Gender), genderRadioGroup.Value);
        premade.Tags = tagsLineEdit.Text.Trim();
        premade.Remark = remarkTextEdit.Text.Trim();
        premade.Styles.IsKid = isKidCheckbox.Pressed;

        Message.Text = "";
        OnSave.Invoke(premade, PremadeId);
    }

    private void _on_DeleteButton_pressed()
    {
        Message.Text = "";
        if (PremadeId == null)
        {
            Premade = new PremadeCharacterInfo();
            return;
        }
        OnDelete.Invoke(PremadeId);
    }
}
