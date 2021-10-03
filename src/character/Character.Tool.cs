using Godot;
using System.Collections.Generic;

namespace Characters
{
    [Tool]
    public partial class Character
    {
        public bool Preview;

        public Dir InitialDir
        {
            get { return dir; }
            set
            {
                dir = value;
                
                if (Engine.EditorHint)
                {
                    playAnimation();
                    PropertyListChangedNotify();
                }
            }
        }

        public State InitialState
        {
            get { return NowState; }
            set
            {
                NowState = value;
            }
        }

        private string premadeId;
        public string PremadeId
        {
            get { return premadeId; }
            set
            {
                premadeId = value;
                LoadPremadeCharacterStyle();
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
                    { "name", "PremadeId" },
                    { "type", Variant.Type.String }
                },
            });
        }

        public void LoadPremadeCharacterStyle()
        {
            if (string.IsNullOrEmpty(premadeId))
            {
                return;
            }
            var premadeCharacterManager = new PremadeCharacterManager();
            var premade = premadeCharacterManager.FindById(premadeId);
            if (premade != null)
            {
                UpdateCharacterStyle(premade.Styles);
            }
            else
            {
                UpdateCharacterStyle(new CharacterStyleSpecifics());
                GD.Print($"未找到Id为{premadeId}的预制角色");
            }
        }

        public async void UpdateCharacterStyle(CharacterStyleSpecifics styleSpecificsToOverride = null)
        {
            if (sprite == null) return;
            if (styleSpecificsToOverride != null)
            {
                this.styleSpecifics = styleSpecificsToOverride;
            }
            
            var sheetTexture = await CharacterStyleGenerator.GenerateImageTexture(styleSpecifics);
            animationPlayer.Stop(false);
            sprite.Texture = sheetTexture;
            playAnimation();
        }
    }
}