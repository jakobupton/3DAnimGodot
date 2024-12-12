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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _skeleton = GetNode<Skeleton3D>("Armature/Skeleton3D");
        _grabArea = GetNode<Area3D>("Armature/Skeleton3D/BoneAttachment3D/Area3D");
        _ball = GetNode<Node3D>("../PrizeBall");

        // get signal entry and exit signal from area3d of armend
        _grabArea.BodyEntered += OnBodyEntered;
        _grabArea.BodyExited += OnBodyExited;

        _animationPlayer.Connect("animation_finished", new Callable(this, nameof(OnAnimationFinished)));
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

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        if (Input.IsKeyPressed(Key.Space))
        {
            _animationPlayer.Play("grab");
        }

        if (Input.IsKeyPressed(Key.Enter))
        {
            _animationPlayer.Play("letgo");
        }

        if (_isHoldingBall && _ball != null)
        {
            // Update ball position relative to the armend
            Vector3 armEndPosition = _skeleton.GetBoneGlobalPose(_skeleton.FindBone("armend")).Origin;
            _ball.GlobalPosition = armEndPosition + new Vector3(1, -0.5f, 0); 
            //offseting 1 to the right because the ball automatically goes off 1 to the left??
        }
    }
    private void OnAnimationFinished(string animName)
    {
        if (animName == "grab")
        {
            if (_canGrabBall && !_isHoldingBall)
            {
                // Set _isHoldingBall to true if the ball is in the grab area
                _isHoldingBall = true;
                GD.Print("Grab animation finished and ball is held.");
            }
        
            //play pickup animation
            _animationPlayer.Play("pickup");
        }

        else if (animName == "letgo" && _isHoldingBall)
        {
            _isHoldingBall = false;
            GD.Print("Letgo animation finished, ball released.");

            // Drop the ball
            Vector3 bonePosition = _skeleton.GetBoneGlobalPose(_skeleton.FindBone("armend")).Origin;
            _ball.GlobalPosition = bonePosition + new Vector3(1, -1, 0);
            // offsetting for same reason as above
        }
    }
}