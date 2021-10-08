using Godot;

public class House1 : YSort
{
    public override void _Ready()
    {
        NodeTileHelper.ReplaceChild(GetNode<TileMap>("TileMap1"), "chimney", Chimney.PackedScene);
    }
}
