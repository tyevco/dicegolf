using Godot;
using System;

public partial class GoalZone : Node3D
{
    [Export(PropertyHint.NodeType)] public Area3D Collision { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Collision.BodyEntered += OnBodyEntered;
        Collision.BodyExited += OnBodyExited;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        //Targets.
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is Dice dice)
        {
            dice.EnteredGoal();
            GD.Print(dice);
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (body is Dice dice)
        {
            GD.Print(dice);            
            dice.ExitedGoal();
        }
    }
}