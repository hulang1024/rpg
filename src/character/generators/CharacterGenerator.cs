using Godot;
using static Godot.GD;
using System.Threading.Tasks;

namespace Characters
{
    public class CharacterGenerator
    {
        private static readonly PackedScene characterPackedScene = Load<PackedScene>("res://src/character/Character.tscn");

        public static Character Generate()
        {
            return characterPackedScene.Instance<Character>();
        }

        public static async Task<Character> Generate(CharacterStyleSpecifics specifics)
        {
            var character = Generate();
            var characterSprite = character.GetNode<Sprite>("Sprite");
            characterSprite.Texture = await CharacterStyleImageGenerator.GenerateImageTexture(specifics);

            return character;
        }
    }
}