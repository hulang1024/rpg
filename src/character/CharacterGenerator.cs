using Godot;
using static Godot.GD;
using System.Threading.Tasks;

namespace Characters
{
    public class CharacterGenerator
    {
        private static PackedScene characterTemplate = null;

        public static Character Generate()
        {
            if (characterTemplate == null)
            {
                characterTemplate = Load<PackedScene>("res://src/character/Character.tscn");
            }

            return characterTemplate.Instance<Character>();
        }

        public static async Task<Character> Generate(CharacterStyleSpecifics specifics)
        {
            var character = Generate();
            var characterSprite = character.GetNode<Sprite>("Sprite");
            characterSprite.Texture = await CharacterStyleGenerator.GenerateImageTexture(specifics);

            return character;
        }
    }
}