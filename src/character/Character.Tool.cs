using Godot;
using System.Collections.Generic;

namespace Characters
{
    [Tool]
    public partial class Character
    {
        public bool Preview;

        private Dir initialDir = Dir.Down;
        [Export]
        public Dir InitialDir
        {
            get { return initialDir; }
            set
            {
                initialDir = value;
                dir = value;
                if (Engine.EditorHint)
                {
                    playAnimation();
                }
            }
        }

        private State initialState;
        [Export]
        public State InitialState
        {
            get { return initialState; }
            set
            {
                initialState = value;
                State = value;
                if (Engine.EditorHint)
                {
                    playAnimation();
                }
            }
        }

        private string styleId;
        [Export]
        public string StyleId
        {
            get { return styleId; }
            set
            {
                styleId = value;
                LoadCharacterStyle();
            }
        }

        private CharacterStyleSpecifics styleSpecifics = new CharacterStyleSpecifics();

        public void LoadCharacterStyle()
        {
            if (string.IsNullOrEmpty(styleId))
            {
                return;
            }
            var styleManager = new CharacterStyleManager();
            var styleInfo = styleManager.FindById(styleId);
            if (styleInfo != null)
            {
                UpdateCharacterStyle(styleInfo.Styles);
            }
            else
            {
                UpdateCharacterStyle(new CharacterStyleSpecifics());
                GD.PrintErr($"未找到Id为{styleId}的角色样式");
            }
        }

        public async void UpdateCharacterStyle(CharacterStyleSpecifics styleSpecificsToOverride = null)
        {
            if (sprite == null) return;
            if (styleSpecificsToOverride != null)
            {
                this.styleSpecifics = styleSpecificsToOverride;
            }
            
            var sheetTexture = await CharacterStyleImageGenerator.GenerateImageTexture(styleSpecifics);
            animationPlayer.Stop(false);
            sprite.Texture = sheetTexture;
            playAnimation();
        }
    }
}