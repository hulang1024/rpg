using Godot;

public sealed class CharacterStyleTexture
{
    public static ImageTexture From(Image image)
    {
        return Textures.From(image);
    }
}