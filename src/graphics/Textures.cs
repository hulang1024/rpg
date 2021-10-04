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

    public static ImageTexture From(string path)
    {
        var image = new Image();
        image.Load(path);
        return From(image);
    }
}