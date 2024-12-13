using Godot;

public partial class ArmControl : Node3D
{
    private AnimationPlayer _animationPlayer;
    private Skeleton3D _skeleton;
    private Node3D _ball;
    private Camera3d _camera;
    private Area3D _grabArea;
    private bool _isHoldingBall = false;
    private bool _canGrabBall = false;

    // For movement
    private Vector3 _velocity = new Vector3();
    private const float MAX_VELOCITY = 0.05f;
    private const float FRICTION = 0.3f;
    private const float ACCELERATION = 0.3f;

    // Rotation variables
    private float _rotationSpeed = 2.0f; // Rotation speed
    private float _minRotation = -1.0f; // Minimum rotation limit (in radians)
    private float _maxRotation = 1.0f;  // Maximum rotation limit (in radians)

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _skeleton = GetNode<Skeleton3D>("Armature/Skeleton3D");
        _grabArea = GetNode<Area3D>("Armature/Skeleton3D/BoneAttachment3D/Area3D");
        _ball = GetNode<Node3D>("../PrizeBall");
        _camera = (Camera3d)GetNode<Node3D>("../Camera3D");

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
        if (Input.IsKeyPressed(Key.Q))
        {
            RotateArm(-_rotationSpeed * (float)delta);
        }

        if (Input.IsKeyPressed(Key.E))
        {
            RotateArm(_rotationSpeed * (float)delta);
        }

        var acceleration = new Vector3(0, 0, 0);
        if (Input.IsKeyPressed(Key.W))
        {
            //acceleration.Z = 1;
            acceleration -= new Vector3(Mathf.Cos(_camera.GetAngle()), 0, Mathf.Sin(_camera.GetAngle()));
        }
        if (Input.IsKeyPressed(Key.S))
        {
            acceleration += new Vector3(Mathf.Cos(_camera.GetAngle()), 0, Mathf.Sin(_camera.GetAngle()));
        }
        if (Input.IsKeyPressed(Key.A))
        {
            acceleration += new Vector3(Mathf.Cos(_camera.GetAngle() + (Mathf.Pi / 2)), 0, Mathf.Sin(_camera.GetAngle() + (Mathf.Pi / 2)));
        }
        if (Input.IsKeyPressed(Key.D))
        {
            acceleration += new Vector3(Mathf.Cos(_camera.GetAngle() - (Mathf.Pi / 2)), 0, Mathf.Sin(_camera.GetAngle() - (Mathf.Pi / 2)));
        }

        // Movement physics
        bool activeFriction = false;
        acceleration = acceleration.Normalized() * (float)delta * ACCELERATION;
        if (acceleration.Length() == 0) {
            acceleration = -_velocity.Normalized() * FRICTION * (float)delta;
            activeFriction = true;
        }
        var prevVelocity = new Vector3(_velocity.X, _velocity.Y, _velocity.Z);
        _velocity += acceleration;
        _velocity.X = Mathf.Clamp(_velocity.X, -MAX_VELOCITY, MAX_VELOCITY);
        _velocity.Z = Mathf.Clamp(_velocity.Z, -MAX_VELOCITY, MAX_VELOCITY);
        if (activeFriction && prevVelocity.Sign() != _velocity.Sign()) {
            _velocity = new Vector3(0, 0, 0);
        }
        GlobalPosition += _velocity;

        // Update ball position if held
        if (_isHoldingBall && _ball != null)
        {
            //Get the armend bone's global transform
            Transform3D armEndTransform = _skeleton.GetBoneGlobalPose(_skeleton.FindBone("armend"));

            //transform ball's position relative to armend
            Vector3 transformedPosition = GlobalTransform * armEndTransform.Origin;

            //offset for ball to be slightly below armend
            _ball.GlobalPosition = transformedPosition + new Vector3(0, -0.5f, 0);
        }
    }

    private void RotateArm(float deltaRotation)
    {
        // Calculate new rotation
        float newRotation = Rotation.Y + deltaRotation;

        // Clamp rotation to defined limits
        //newRotation = Mathf.Clamp(newRotation, _minRotation, _maxRotation);

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

