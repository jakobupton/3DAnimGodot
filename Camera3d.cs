using Godot;
using System;

public partial class Camera3d : Camera3D
{
	// Horizontal angle of the camera (movement along the xz-plane)
	float _angle = Mathf.Pi / 4;

	// For interpolation
	float _targetAngle = Mathf.Pi / 4;

	// Distance from the center
	float DISTANCE = 6;

	public float GetAngle() {
		return _angle;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent)
		{
			if (keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
			{
				GetTree().Quit();
			}
		} else if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed) {
			if (mouseEvent.ButtonIndex == MouseButton.Left) {
				_targetAngle += Mathf.Pi / 2;
			} else if (mouseEvent.ButtonIndex == MouseButton.Right) {
				_targetAngle -= Mathf.Pi / 2;
			}
		}

		// Zoom in and out implementations
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.WheelUp)
        	{
            	DISTANCE -= 0.2f;
				DISTANCE = Mathf.Clamp(DISTANCE, 1, 10);
        	}
    		else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
        	{
            	DISTANCE += 0.2f;
				DISTANCE = Mathf.Clamp(DISTANCE, 1, 10);
        	}
		}

		// this should be for touch pads
		if (@event is InputEventPanGesture panEvent)
		{
			
			DISTANCE += panEvent.Delta.Y * 0.1f;
			DISTANCE = Mathf.Clamp(DISTANCE, 1, 10);
		}
	}
	
	public override void _Process(double delta) {
		// Interpolate camera movement
		_angle += (_targetAngle - _angle) * 2.5f * (float)delta;

		// Move camera
		var centre = new Vector3(0, 1, 0);
		Position = centre + new Vector3(
			DISTANCE * Mathf.Cos(_angle),
			5,
			DISTANCE * Mathf.Sin(_angle)
		);

		// Look at center
		LookAt(centre - new Vector3(0, 2, 0));
	}
}