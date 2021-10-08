using Godot;
using System;
using Characters;

namespace Tools.CharacterStyleDesigner
{
    public class StyleInfoForm : Panel
    {
        private const string form_path = "MarginContainer/VBoxContainer";

        public Action OnNew;
        public Action<CharacterStyleInfo, string> OnSave;
        public Action<string> OnDelete;
        public Action OnModelChanged;
        public Action OnStyleDepsChanged;
        
        public string StyleId;

        private CharacterStyleInfo styleInfo = new CharacterStyleInfo();
        public CharacterStyleInfo StyleInfo
        {
            get { return styleInfo; }
            set
            {
                styleInfo = value;
                StyleId = styleInfo.Id;
                OnModelChanged.Invoke();
                SyncFormItems();
            }
        }

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
            idLineEdit.Text = styleInfo.Id;
            isKidCheckbox.Pressed = styleInfo.Styles.IsKid;
            genderRadioGroup.Value = Enum.GetName(typeof(Gender), styleInfo.Gender);
            tagsLineEdit.Text = styleInfo.Tags;
            remarkTextEdit.Text = styleInfo.Remark;
            Message.Text = "";
        }

        private void _on_IsKidCheckBox_toggled(bool pressed)
        {
            styleInfo.Styles = new CharacterStyleSpecifics(pressed);
            styleInfo.Styles.IsKid = pressed;
            OnStyleDepsChanged.Invoke();
        }

        private void _on_FrameSizeOptionButton_item_selected(int index)
        {
            var frameSize = new CharacterFrameSize[]
            {
                CharacterFrameSize.Medium,
                CharacterFrameSize.Small,
                CharacterFrameSize.Mini
            }[index];
            styleInfo.Styles = new CharacterStyleSpecifics(styleInfo.Styles.IsKid);
            styleInfo.Styles.FrameSize = frameSize;
            OnStyleDepsChanged.Invoke();
        }

        private void _on_NewButton_pressed()
        {
            StyleInfo = new CharacterStyleInfo();
            OnNew.Invoke();
        }

        private void _on_SaveButton_pressed()
        {
            styleInfo.Id = idLineEdit.Text.Trim();
            styleInfo.Gender = (Gender)Enum.Parse(typeof(Gender), genderRadioGroup.Value);
            styleInfo.Tags = tagsLineEdit.Text.Trim();
            styleInfo.Remark = remarkTextEdit.Text.Trim();
            styleInfo.Styles.IsKid = isKidCheckbox.Pressed;

            Message.Text = "";
            OnSave.Invoke(styleInfo, StyleId);
        }

        private void _on_DeleteButton_pressed()
        {
            Message.Text = "";
            if (StyleId == null)
            {
                StyleInfo = new CharacterStyleInfo();
                return;
            }
            OnDelete.Invoke(StyleId);
        }
    }
}
