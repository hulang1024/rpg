using Godot;
using System;
using Characters;

namespace Tools.CharacterStyleDesigner
{
    public class StyleSelectItem : Control
    {
        public Action<bool> OnSelect;
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

        public static StyleSelectItem Instance()
        {
            if (packedScene == null)
            {
                packedScene = GD.Load<PackedScene>("res://src/tools/character_style_designer/StyleSelectItem.tscn");
            }
            return packedScene.Instance<StyleSelectItem>();
        }

        public override void _Ready()
        {
            Connect("gui_input", this, "onGuiInput");
            Connect("mouse_entered", this, "onMouseHover", new Godot.Collections.Array(true));
            Connect("mouse_exited", this, "onMouseHover", new Godot.Collections.Array(false));
        }

        public void LoadFrom(string filePath, CharacterStyleComponentType? type)
        {
            this.filePath = filePath;
            var image = new Image();
            image.Load(filePath);
            LoadFrom(CharacterStyleTexture.From(image), type);
        }

        public void LoadFrom(ImageTexture texture, CharacterStyleComponentType? type)
        {
            textureRect = GetNode<TextureRect>("TextureRect");
            var t = (Texture)textureRect.Texture.Duplicate();
            t.Set("atlas", texture);
            t.Set("region", type != CharacterStyleComponentType.Smartphones
                ? new Rect2(new Vector2(1, 16), new Vector2(128, 50))
                : new Rect2(new Vector2(1, 288), new Vector2(128, 50)));
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
                OnSelect.Invoke(selected);
            }
            else if (buttonEvent.ButtonIndex == (int)ButtonList.Right)
            {
                OnRequestPopupMenu?.Invoke(buttonEvent.GlobalPosition);
            }
        }
    }
}
