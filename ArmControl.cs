using Godot;
using System;

public partial class ArmControl : Node3D
{
    private AnimationPlayer _animationPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("idle");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Space))
        {
            _animationPlayer.Play("grab");
            _animationPlayer.Queue("pickup");

        }

        if (Input.IsKeyPressed(Key.Enter))
        {
            _animationPlayer.Play("letgo");
        }

    }
}