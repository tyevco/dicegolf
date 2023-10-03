using Godot;
using System;

[Tool]
public partial class DiceSideLines : Dice
{
    // Called when the node enters the scene tree for the first time.

    public override async void _Ready()
    {
        await new SignalAwaiter(GetTree(), "process_frame", this);

        if (!IsInsideTree())
            return;

        base._Ready();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            foreach (var side in Sides)
            {
                DebugDraw3D.DrawLine(Position, Position + side.Value * 2, Color.FromHsv(side.Key / (float)Sides.Count, 1f, 1f));
            }
        }

        base._Process(delta);
    }
}