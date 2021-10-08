using Godot;
using System;

public class ElevatorDoor : StaticBody2D
{
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    private void _on_TestTimer_timeout()
    {
        animationPlayer.PlayBackwards(animationPlayer.CurrentAnimation == "closed" ? "opened" : "closed");
    }
}
