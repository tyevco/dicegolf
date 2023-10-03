using Godot;
using System;

public partial class Status : RichTextLabel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void _on_d_20_dice_rolled(int result, Dice dice)
    {
        GD.Print($"rolled {result}");
        this.Text = $"{DateTime.Now.ToShortTimeString()} : rolled {result}{(dice.InGoal ? " - finished" : "")}\n"
	        + this.Text;
    }

    public void _on_golf_swing_swing(Vector3 direction, float power)
    {
	    this.Text = $"{DateTime.Now.ToShortTimeString()} : swing {direction} @ {power}\n"
	                + this.Text;
    }
}
