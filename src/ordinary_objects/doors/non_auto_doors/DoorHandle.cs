using Godot;
using OrdinaryObjects;

/// <summary>
/// 可操作开关门Area
/// </summary>
public class DoorHandle : Area2D, ActionableObject
{
    private Door door;

    public override void _Ready()
    {
        door = GetParent<Door>();
    }

    public void Toggle()
    {
        door.Toggle();
    }

    public void ToggleLock(LockKey key)
    {
        door.Toggle(key);
    }
}
