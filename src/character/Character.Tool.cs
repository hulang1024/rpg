using Godot;
using System.Collections.Generic;

namespace Characters
{
    [Tool]
    public partial class Character
    {
        public bool Preview;

        private Dir initialDir;
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
        public State InitialState
        {
            get { return initialState; }
            set
            {
                initialState = value;
                NowState = value;
                if (Engine.EditorHint)
                {
                    playAnimation();
                }
            }
        }

        private string styleId;
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
        

        public override Godot.Collections.Array _GetPropertyList()
        {
            return new Godot.Collections.Array(new Dictionary<string, dynamic>[] {
                new Dictionary<string, dynamic> {
                    { "name", "角色设计" },
                    { "type", Variant.Type.Nil },
                    { "usage", PropertyUsageFlags.Category | PropertyUsageFlags.ScriptVariable }
                },

                new Dictionary<string, dynamic> {
                    { "name", "InitialDir" },
                    { "type", Variant.Type.Int }
                },
                new Dictionary<string, dynamic> {
                    { "name", "InitialState" },
                    { "type", Variant.Type.Int }
                },
                new Dictionary<string, dynamic> {
                    { "name", "StyleId" },
                    { "type", Variant.Type.String }
                },
            });
        }

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
                GD.Print($"未找到Id为{styleId}的角色样式");
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