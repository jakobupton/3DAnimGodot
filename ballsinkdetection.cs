using Godot;
using System;

public partial class ballsinkdetection : Area3D
{
	 private int counter = 0;
	 //spawn position for prizeball
	 public Vector3 SpawnPosition = new Vector3(1, 5, 0);

	 public RigidBody3D prizeBall;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		prizeBall = GetNode<RigidBody3D>("../../PrizeBall");

		// Connect the signal to the function
        this.BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
    {
        // Check if the body is the PrizeBall
        if (body == prizeBall)
        {
            counter++;
            GD.Print("Counter: " + counter);

			RespawnPrizeBall(prizeBall);
        }
    }

	private void RespawnPrizeBall(RigidBody3D prizeBall)
    {
        // Reset position
        prizeBall.GlobalTransform = new Transform3D(prizeBall.GlobalTransform.Basis, SpawnPosition);

		//Reset velocity other than vertical
        Vector3 currentVelocity = prizeBall.LinearVelocity;
		prizeBall.LinearVelocity = new Vector3(0, currentVelocity.Y, 0);

        GD.Print("PrizeBall respawned at: " + SpawnPosition);
    }
}
