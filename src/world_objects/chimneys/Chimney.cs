using Godot;

public class Chimney : StaticBody2D
{
    public static readonly PackedScene PackedScene = GD.Load<PackedScene>(
        "res://src/world_objects/chimneys/Chimney.tscn");
}
