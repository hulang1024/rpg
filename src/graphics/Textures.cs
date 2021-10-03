using Godot;

public sealed class Textures
{
    public static ImageTexture From(Image image)
    {
        var texture = new ImageTexture();
        texture.CreateFromImage(image);
        texture.Flags = (int)Texture.FlagsEnum.Mipmaps;
        return texture;
    }
}