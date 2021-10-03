using Godot;
using System;

public class RadioGroup : HBoxContainer
{
    public Action<string> OnChanged;

    private ButtonGroup group;

    public string Value
    {
        get
        {
            return group.GetPressedButton().Name;
        }
        set
        {
            foreach (Button btn in group.GetButtons())
            {
                btn.Pressed = (btn.Name == value);
            }
        }
    }

    public override void _Ready()
    {
        group = new ButtonGroup();
        foreach (Button btn in GetChildren())
        {
            btn.Group = group;
            btn.Connect("pressed", this, "onRadioCheck", new Godot.Collections.Array(btn));
        }
    }

    private void onRadioCheck(Button btn)
    {
        OnChanged.Invoke(btn.Name);
    }
}