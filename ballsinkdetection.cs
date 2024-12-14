using Godot;
using System;

public partial class ballsinkdetection : Area3D
{
    private int counter = 0;
    public Vector3 SpawnPosition = new Vector3(1, 5, 0);
    public RigidBody3D prizeBall;

    // Reference to the score label in the UI
    private Label scoreLabel;

    public override void _Ready()
    {
        // Get the PrizeBall node
        prizeBall = GetNode<RigidBody3D>("../../PrizeBall");

        // Connect the signal to the function
        this.BodyEntered += OnBodyEntered;

        // Get the score label node
        scoreLabel = GetNode<Label>("../../CanvasLayer/ScoreLabel");
        UpdateScoreLabel();
    }

    private void OnBodyEntered(Node body)
    {
        // Check if the body is the PrizeBall
        if (body == prizeBall)
        {
            counter++;
            GD.Print("Counter: " + counter);

            // Update the UI label
            UpdateScoreLabel();

            // Respawn the PrizeBall
            RespawnPrizeBall(prizeBall);
        }
    }

    private async void RespawnPrizeBall(RigidBody3D prizeBall)
    {
        // Wait for 3 seconds
        await ToSignal(GetTree().CreateTimer(1.5), "timeout");

        // Reset position
        prizeBall.GlobalTransform = new Transform3D(prizeBall.GlobalTransform.Basis, SpawnPosition);

        // Reset velocity other than vertical
        Vector3 currentVelocity = prizeBall.LinearVelocity;
        prizeBall.LinearVelocity = new Vector3(0, currentVelocity.Y, 0);

        GD.Print("PrizeBall respawned at: " + SpawnPosition);
    }

    private void UpdateScoreLabel()
    {
        // Update the score label text
        scoreLabel.Text = $"Score: {counter}";
    }
}
