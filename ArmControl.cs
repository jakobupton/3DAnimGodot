using Godot;
using System;

public partial class ArmControl : Node3D
{
    private AnimationPlayer _animationPlayer;
    private Skeleton3D _skeleton;
    private Node3D _ball;
    private Area3D _grabArea;
    private bool _isHoldingBall = false;
    private bool _canGrabBall = false;

    // Rotation variables
    private float _rotationSpeed = 1.0f; // Rotation speed
    private float _minRotation = -1.0f; // Minimum rotation limit (in radians)
    private float _maxRotation = 1.0f;  // Maximum rotation limit (in radians)

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _skeleton = GetNode<Skeleton3D>("Armature/Skeleton3D");
        _grabArea = GetNode<Area3D>("Armature/Skeleton3D/BoneAttachment3D/Area3D");
        _ball = GetNode<Node3D>("../ball");

        // Connect signals for grab area
        _grabArea.BodyEntered += OnBodyEntered;
        _grabArea.BodyExited += OnBodyExited;

        // Connect animation finished signal
        _animationPlayer.Connect("animation_finished", new Callable(this, nameof(OnAnimationFinished)));

        // Play idle animation initially
        _animationPlayer.Play("idle");
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body == _ball)
        {
            GD.Print("Ball entered the grabbing area!");
            _canGrabBall = true;
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (body == _ball)
        {
            GD.Print("Ball exited the grabbing area!");
            _canGrabBall = false;
        }
    }

    public override void _Process(double delta)
    {
        // Handle grabbing and letting go
        if (Input.IsKeyPressed(Key.Space))
        {
            _animationPlayer.Play("grab");
        }

        if (Input.IsKeyPressed(Key.Enter))
        {
            _animationPlayer.Play("letgo");
        }

        // Handle arm swiveling
        if (Input.IsKeyPressed(Key.A))
        {
            RotateArm(-_rotationSpeed * (float)delta);
        }

        if (Input.IsKeyPressed(Key.D))
        {
            RotateArm(_rotationSpeed * (float)delta);
        }

        // Update ball position if held
        if (_isHoldingBall && _ball != null)
        {
            Vector3 armEndPosition = _skeleton.GetBoneGlobalPose(_skeleton.FindBone("armend")).Origin;
            _ball.GlobalPosition = armEndPosition + new Vector3(1, -0.5f, 0);
        }
    }

    private void RotateArm(float deltaRotation)
    {
        // Calculate new rotation
        float newRotation = Rotation.Y + deltaRotation;

        // Clamp rotation to defined limits
        newRotation = Mathf.Clamp(newRotation, _minRotation, _maxRotation);

        // Apply rotation
        Rotation = new Vector3(Rotation.X, newRotation, Rotation.Z);
    }

    private void OnAnimationFinished(string animName)
    {
        if (animName == "grab")
        {
            if (_canGrabBall && !_isHoldingBall)
            {
                _isHoldingBall = true;
                GD.Print("Grab animation finished and ball is held.");
            }

            // Play the pickup animation
            _animationPlayer.Play("pickup");
        }
        else if (animName == "letgo" && _isHoldingBall)
        {
            _isHoldingBall = false;
            GD.Print("Letgo animation finished, ball released.");

            // Drop the ball at the current claw position
            Vector3 bonePosition = _skeleton.GetBoneGlobalPose(_skeleton.FindBone("armend")).Origin;
            _ball.GlobalPosition = bonePosition + new Vector3(1, -1, 0);
        }
    }
}

