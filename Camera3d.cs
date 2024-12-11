using Godot;
using System;

public partial class Camera3d : Camera3D
{
	// Horizontal angle of the camera (movement along the xz-plane)
	float _angle;

	// Vertical angle of the camera (movement along the y-axis)
	float _yangle;

	// Distance from the center
	float DISTANCE = 5;

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
		}

		if (@event is InputEventMouseMotion mouseEvent)
		{
			// Rotate camera
			_angle += mouseEvent.Relative.X * 0.005f;
			_yangle -= mouseEvent.Relative.Y * 0.005f;

			// Clamp vertical angle
			_yangle = Mathf.Clamp(_yangle, -Mathf.Pi / 6, Mathf.Pi / 6);
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

		// Move camera
		var centre = new Vector3(0, 1, 0);
		Position = centre + new Vector3(
			DISTANCE * Mathf.Cos(_angle),
			DISTANCE * Mathf.Tan(_yangle),
			DISTANCE * Mathf.Sin(_angle)
		);

		// Look at center
		LookAt(centre);
	}
}