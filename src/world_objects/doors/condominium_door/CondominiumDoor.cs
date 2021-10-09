using Godot;
using static Godot.GD;

namespace OrdinaryObjects
{
    [Tool]
    public class CondominiumDoor : Door
    {
        protected override void PreloadSpriteTextures()
        {
            string pathPrefix = "user://res/ordinary_objects/animated_door_condominium";
            sprite.Texture = Textures.From($"{pathPrefix}_{styleId}_32x32.png");
        }
    }
}