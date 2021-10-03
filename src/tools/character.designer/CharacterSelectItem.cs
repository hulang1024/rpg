using Godot;
using System;

public class CharacterSelectItem : Control
{
    public Action<bool> onSelect;

    public Action<Vector2> OnRequestPopupMenu;

    public string filePath;

    private static PackedScene packedScene = null;

    private TextureRect textureRect;

    private bool selected;
    public bool Selected
    {
        get { return selected; }
        set
        {
            if (value != selected)
            {
              selected = value;
              drawSelected(value);
            }
        }
    }

    public static CharacterSelectItem Instance()
    {
        if (packedScene == null)
        {
            packedScene = GD.Load<PackedScene>("res://src/tools/character.designer/CharacterSelectItem.tscn");
        }
        return packedScene.Instance<CharacterSelectItem>();
    }

    public override void _Ready()
    {
        Connect("gui_input", this, "onGuiInput");
        Connect("mouse_entered", this, "onMouseHover", new Godot.Collections.Array(true));
        Connect("mouse_exited", this, "onMouseHover", new Godot.Collections.Array(false));
    }

    public void LoadFrom(string filePath)
    {
        this.filePath = filePath;
        var image = new Image();
        image.Load(filePath);
        LoadFrom(CharacterStyleTexture.From(image));
    }

    public void LoadFrom(ImageTexture texture)
    {
        textureRect = GetNode<TextureRect>("TextureRect");
        var t = (Texture)textureRect.Texture.Duplicate();
        t.Set("atlas", texture);
        textureRect.Texture = t;
    }

    public void drawSelected(bool selected) {
        var style = (StyleBox)GetStylebox("panel").Duplicate();
        style.Set("border_color", selected ? new Color(0x5a5a5af) : new Color(0x3a3a4f));
        AddStyleboxOverride("panel", style);
    }

    private void onMouseHover(bool entered)
    {
        drawSelected(entered);
    }

    private void onGuiInput(InputEvent @event)
    {
        if (!(@event is InputEventMouseButton))
            return;
        var buttonEvent = (@event as InputEventMouseButton);
        if (!buttonEvent.IsPressed())
            return;
        if (buttonEvent.ButtonIndex == (int)ButtonList.Left)
        {
            selected = !selected;
            drawSelected(selected);
            onSelect.Invoke(selected);
        }
        else if (buttonEvent.ButtonIndex == (int)ButtonList.Right)
        {
            OnRequestPopupMenu?.Invoke(buttonEvent.GlobalPosition);
        }
    }
}
